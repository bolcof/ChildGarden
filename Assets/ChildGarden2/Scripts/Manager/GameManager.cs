using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GameManager : Photon.PunBehaviour {

    public static GameManager Instance;

    [SerializeField] RoundManager roundManager;
    [SerializeField] RuleManager ruleManager;
    [SerializeField] UtsuwaManager stageManager;

    [SerializeField] CreateRayPoint createRayPoint;

    [SerializeField] PlayingView playingVew;

    public bool canPutOnbutsu;
    public bool canOperateUI;
    private bool isPlaying;

    public int winnerIsMine; /* -1:not yet 0:other 1:me 2:draw */

    public float timeLimit, remainingTimeLimit;

    //TODO:ここじゃないんだよな～～～
    [SerializeField] private List<GameObject> backgroundObject;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        canPutOnbutsu = false;
        canOperateUI = false;
        isPlaying = false;
    }

    public void GameStart() {
        if (PhotonNetwork.isMasterClient) {
            photonView.RPC(nameof(FirstRoundStart), PhotonTargets.AllBuffered);
        }
    }

    [PunRPC]
    private void FirstRoundStart() {
        CountDownStart().Forget();

        stageManager.SetStage();

        roundManager.currentRound = 1;
        ruleManager.SetFirstRound();

        playingVew.RoundStart(1, ruleManager.currentRule);
        remainingTimeLimit = timeLimit;
        winnerIsMine = -1;

        SoundManager.Instance.PlayBgm(SoundManager.Instance.BGM_GameScene[0]);

        foreach (var bgo in backgroundObject) {
            bgo.SetActive(false);
        }
        backgroundObject[0].SetActive(true);
        backgroundObject[0].GetComponent<BackgroundSlider>().StartSlider();
    }

    public void NextRoundStart() {
        Debug.Log("NextRound!");
        ViewManager.Instance.playingView.ApplyTimeLimit((int)timeLimit);
        CountDownStart().Forget();

        stageManager.AppearMyPlayerPin();
        ruleManager.ResetCount();

        roundManager.currentRound++;
        SoundManager.Instance.PlayBgm(SoundManager.Instance.BGM_GameScene[roundManager.currentRound - 1]);

        playingVew.gameObject.SetActive(true);
        playingVew.RoundStart(roundManager.currentRound, ruleManager.currentRule);

        remainingTimeLimit = timeLimit;

        winnerIsMine = -1;
    }

    public void BackGroundVideoStart() {
        backgroundObject[roundManager.currentRound].GetComponent<VideoPlayer>().Play();
        backgroundObject[roundManager.currentRound].GetComponent<BackgroundSlider>().StartSlider();
    }

    private async UniTask CountDownStart() {

        await UniTask.Delay(2000);
        ViewManager.Instance.playingView.countDownObject.SetActive(true);
        GameObject.Find("Cursor").GetComponent<CursorBehaviour>().displayed = false;
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_CountDown);
        await UniTask.Delay(3600);
        ViewManager.Instance.playingView.countDownObject.SetActive(false);
        GameObject.Find("Cursor").GetComponent<CursorBehaviour>().displayed = true;

        canPutOnbutsu = true;
        isPlaying = true;
    }

    private void Update() {
        if (isPlaying) {
            remainingTimeLimit -= Time.deltaTime;
            ViewManager.Instance.playingView.ApplyTimeLimit((int)remainingTimeLimit);
            if (remainingTimeLimit < 0.0f) {
                isPlaying = false;
                ViewManager.Instance.playingView.ApplyTimeLimit(0);
                TimeOver();
            }
        }
    }

    public void ResetWorld() {
        foreach (var obj in GameObject.FindGameObjectsWithTag("Onbutu")) {
            Destroy(obj);
        }

        foreach (var bgo in backgroundObject) {
            bgo.SetActive(false);
        }
        backgroundObject[roundManager.currentRound].SetActive(true);
    }

    public void MyPlayerWin() {
        photonView.RPC(nameof(OtherPlayerWin), PhotonTargets.OthersBuffered, RoomConector.Instance.MyPlayerId());
        winnerIsMine = 1;
        roundManager.FinishRound(1);
        DecideWinner();
    }

    public void TimeOver() {
        canPutOnbutsu = false;
        photonView.RPC(nameof(Draw), PhotonTargets.All);
    }

    public void DecideWinner() {
        canPutOnbutsu = false;
        isPlaying = false;
        ViewManager.Instance.playingView.RoundFinish(winnerIsMine).Forget();
        SoundManager.Instance.BgmSource.Stop();
        createRayPoint.DisappearGauge();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        //これが無いと動くけどエラーが出る
        if (stream.isWriting) {
            // ここにオブジェクトの状態を送信するコードを書きます
        } else {
            // ここにオブジェクトの状態を受信して更新するコードを書きます
        }
    }

    [PunRPC]
    public void OtherPlayerWin(int winnerID) {
        winnerIsMine = 0;
        roundManager.FinishRound(0);
        DecideWinner();
    }

    [PunRPC]
    public void Draw() {
        winnerIsMine = 2;
        roundManager.FinishRound(2);
        DecideWinner();
    }
}