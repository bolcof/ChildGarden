using Cysharp.Threading.Tasks;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GameManager : NetworkBehaviour {

    public static GameManager Instance;

    [SerializeField] RuleManager ruleManager;
    [SerializeField] UtsuwaManager stageManager;

    [SerializeField] CreateRayPoint createRayPoint;

    public bool canOperateUI;
    private bool isPlaying;

    public int winnerIsMine; /* -1:not yet 0:other 1:me 2:draw */

    public float timeLimit;
    [Networked] public float remainingTimeLimit { get; set; }

    [SerializeField] private BackgroundRoot backgroundRoot;

    public override void Spawned() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        canOperateUI = false;
        isPlaying = false;

        createRayPoint = GameObject.Find("MainCamera").GetComponent<CreateRayPoint>();
        backgroundRoot = GameObject.Find("BackgroundRoot").GetComponent<BackgroundRoot>();

        Debug.Log("MyDebug GameManager Spawned");
    }

    public void GameStart() {
        Debug.Log("MyDebug GameManager GameStart");
        if (RoomConector.Instance.networkRunner.IsSharedModeMasterClient) {
            RPC_FirstRoundStart();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_FirstRoundStart() {
        CountDownStart().Forget();

        stageManager.SetStage();

        RoundManager.instance.currentRound = 1;
        ruleManager.SetFirstRound();

        ViewManager.Instance.playingView.RoundStart(1, ruleManager.currentRule);
        remainingTimeLimit = timeLimit;
        winnerIsMine = -1;

        SoundManager.Instance.PlayBgm(SoundManager.Instance.BGM_GameScene[0]);

        foreach (var bgo in backgroundRoot.backgrounds) {
            bgo.SetActive(false);
        }
        backgroundRoot.backgrounds[0].SetActive(true);
        backgroundRoot.backgrounds[0].GetComponent<BackgroundSlider>().StartSlider();
    }

    public void NextRoundStart() {
        Debug.Log("MyDebug NextRound! " + RoomConector.Instance.MyPlayerId().ToString());
        //TODO:kore naito sorezore kara 2kai yobaretyau nandeya.
        //if (RoomConector.Instance.HasStateAuthority) {
        //  RoomConector.Instance.rpcListner.RPC_PlayingView_ApplyTimeLimit((int)timeLimit);
        //}
        RoomConector.Instance.rpcListner.RPC_PlayingView_ApplyTimeLimit((int)timeLimit);
        CountDownStart().Forget();

        stageManager.AppearMyPlayerPin();
        ruleManager.ResetCount();

        RoundManager.instance.currentRound++;
        SoundManager.Instance.PlayBgm(SoundManager.Instance.BGM_GameScene[RoundManager.instance.currentRound - 1]);

        ViewManager.Instance.playingView.gameObject.SetActive(true);
        ViewManager.Instance.playingView.RoundStart(RoundManager.instance.currentRound, ruleManager.currentRule);

        remainingTimeLimit = timeLimit;

        winnerIsMine = -1;
    }

    public void BackGroundVideoStart() {
        backgroundRoot.backgrounds[RoundManager.instance.currentRound].GetComponent<VideoPlayer>().Play();
        backgroundRoot.backgrounds[RoundManager.instance.currentRound].GetComponent<BackgroundSlider>().StartSlider();
    }

    private async UniTask CountDownStart() {

        await UniTask.Delay(2000);
        ViewManager.Instance.playingView.countDownObject.SetActive(true);
        GameObject.Find("Cursor").GetComponent<CursorBehaviour>().displayed = false;
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_CountDown);
        await UniTask.Delay(3600);
        ViewManager.Instance.playingView.countDownObject.SetActive(false);
        GameObject.Find("Cursor").GetComponent<CursorBehaviour>().displayed = true;

        LocalStateManager.Instance.canPutOnbutsu = true;
        isPlaying = true;
    }

    public override void FixedUpdateNetwork() {
        if (isPlaying) {
            remainingTimeLimit -= Runner.DeltaTime;
            RoomConector.Instance.rpcListner.RPC_PlayingView_ApplyTimeLimit((int)remainingTimeLimit);
            if (remainingTimeLimit < 0.0f) {
                isPlaying = false;
                RoomConector.Instance.rpcListner.RPC_PlayingView_ApplyTimeLimit(0);
                RPC_TimeOverAndDraw();
            }
        }
    }

    public void ResetWorld() {
        foreach (var obj in GameObject.FindGameObjectsWithTag("Onbutu")) {
            Destroy(obj);
        }

        foreach (var bgo in backgroundRoot.backgrounds) {
            bgo.SetActive(false);
        }
        if (RoundManager.instance.currentRound <= 3) {
            backgroundRoot.backgrounds[RoundManager.instance.currentRound].SetActive(true);
        }
    }

    public void MyPlayerWin() {
        RPC_OtherPlayerWin(RoomConector.Instance.MyPlayerId());
        winnerIsMine = 1;
        RoundManager.instance.FinishRound(1);
        DecideWinner();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_OtherPlayerWin(int winnerID, RpcInfo info = default) {
        if (info.Source != Runner.LocalPlayer) {
            winnerIsMine = 0;
            RoundManager.instance.FinishRound(0);
            DecideWinner();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_TimeOverAndDraw() {
        winnerIsMine = 2;
        RoundManager.instance.FinishRound(2);
        DecideWinner();
    }

    public void DecideWinner() {
        LocalStateManager.Instance.canPutOnbutsu = false;
        isPlaying = false;
        SoundManager.Instance.BgmSource.Stop();
        createRayPoint.DisappearGauge();
        ViewManager.Instance.playingView.RoundFinish(winnerIsMine).Forget();
    }
}