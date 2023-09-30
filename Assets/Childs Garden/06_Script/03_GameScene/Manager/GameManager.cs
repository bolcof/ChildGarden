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

    [SerializeField] PlayingView playingVew;

    public bool canPutOnbutsu;
    public bool canOperateUI;
    private bool isPlaying;

    public int winnerIsMine; /* -1:not yet 0:me 1:other*/

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
        winnerIsMine = -1;
    }

    public void NextRoundStart() {
        Debug.Log("NextRound!");
        ResetWorld();
        ruleManager.ResetCount();

        roundManager.currentRound++;

        playingVew.gameObject.SetActive(true);
        playingVew.RoundStart(roundManager.currentRound, ruleManager.currentRule);

        canPutOnbutsu = true;
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
        if(isPlaying) {
            timeLimit -= Time.deltaTime;
            ViewManager.Instance.playingView.ApplyTimeLimit((int)timeLimit);
            if (timeLimit < 0.0f) {
                TimeOver();
            }
        }
    }

    private void ResetWorld() {
        foreach(var obj in GameObject.FindGameObjectsWithTag("Onbutu")) {
            Destroy(obj);
        }
    }

    public void MyPlayerWin() {
        photonView.RPC(nameof(OtherPlayerWin), PhotonTargets.OthersBuffered, MatchingStateManager.instance.MyPlayerId());
        winnerIsMine = 0;
        roundManager.FinishRound(true);
        DecideWinner();
    }

    public void TimeOver() {

    }

    public void DecideWinner() {
        canPutOnbutsu = false;
        switch (winnerIsMine) {
            case 0:
                playingVew.AppearWinObject();
                break;
            case 1:
                playingVew.AppearLoseObject();
                break;
            case 2:
                playingVew.AppearDrawObject();
                break;
            default:
                break;
        }
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
        winnerIsMine = 1;
        roundManager.FinishRound(false);
        DecideWinner();
    }
}