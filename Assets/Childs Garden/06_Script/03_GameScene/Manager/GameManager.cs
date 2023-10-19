using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Photon.PunBehaviour {

    public static GameManager Instance;

    [SerializeField] RoundManager roundManager;
    [SerializeField] RuleManager ruleManager;
    [SerializeField] StageManager stageManager;

    [SerializeField] CreateRayPoint createRayPoint;

    [SerializeField] PlayingView playingVew;

    public bool canPutOnbutsu;
    public bool canOperateUI;
    private bool isPlaying;

    public int winnerIsMine; /* -1:not yet 0:other 1:me 2:draw */

    [SerializeField] float BeginningCountDownTime;
    public float timeLimit, remainingTimeLimit;

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

    private void Start() {
        FirstRoundStart();
    }

    private void FirstRoundStart() {
        CountDownStart(BeginningCountDownTime).Forget();

        stageManager.SetStage();

        roundManager.currentRound = 1;
        ruleManager.SetFirstRound();

        playingVew.RoundStart(1, ruleManager.currentRule);
        remainingTimeLimit = timeLimit;
        winnerIsMine = -1;

        SoundManager.Instance.PlayBgm(SoundManager.Instance.BGM_GameScene[0]);
    }

    public void NextRoundStart() {
        Debug.Log("NextRound!");
        ViewManager.Instance.playingView.ApplyTimeLimit((int)timeLimit);
        CountDownStart(BeginningCountDownTime).Forget();

        stageManager.AppearMyPlayerPin().Forget();

        ResetWorld();
        ruleManager.ResetCount();

        roundManager.currentRound++;
        SoundManager.Instance.PlayBgm(SoundManager.Instance.BGM_GameScene[roundManager.currentRound - 1]);

        playingVew.gameObject.SetActive(true);
        playingVew.RoundStart(roundManager.currentRound, ruleManager.currentRule);

        remainingTimeLimit = timeLimit;

        winnerIsMine = -1;
    }

    private async UniTask CountDownStart(float sec) {
        float remainingTime = sec;

        while (remainingTime > 0.0f) {
            ViewManager.Instance.playingView.BeginningCountDown((int)remainingTime);
            await UniTask.Delay(1000);
            remainingTime -= 1.0f;
        }

        ViewManager.Instance.playingView.BeginningCountDown(0);

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

    private void ResetWorld() {
        foreach (var obj in GameObject.FindGameObjectsWithTag("Onbutu")) {
            Destroy(obj);
        }
    }

    public void MyPlayerWin() {
        photonView.RPC(nameof(OtherPlayerWin), PhotonTargets.OthersBuffered, MatchingStateManager.instance.MyPlayerId());
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