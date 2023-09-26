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

    public bool isPlaying;
    public int winnerIsMine; /* -1:not yet 0:me 1:other*/

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        isPlaying = false;
    }

    private void Start() {
        FirstRoundStart();
    }

    private void FirstRoundStart() {
        stageManager.SetStage();
        roundManager.currentRound = 1;
        ruleManager.SetFirstRound();

        playingVew.RoundStart(1, ruleManager.currentRule);

        isPlaying = true;
        winnerIsMine = -1;
    }

    public void NextRoundStart() {
        Debug.Log("NextRound!");
        ResetWorld();
        ruleManager.ResetCount();

        roundManager.currentRound++;

        playingVew.gameObject.SetActive(true);
        playingVew.RoundStart(roundManager.currentRound, ruleManager.currentRule);

        isPlaying = true;
        winnerIsMine = -1;
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

    public void DecideWinner() {
        isPlaying = false;
        switch (winnerIsMine) {
            case 0:
                playingVew.AppearWinObject();
                break;
            case 1:
                playingVew.AppearLoseObject();
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