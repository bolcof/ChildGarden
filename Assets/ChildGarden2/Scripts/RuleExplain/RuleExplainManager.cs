using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fusion;

public class RuleExplainManager : NetworkBehaviour {
    public static RuleExplainManager Instance;
    [Networked]
    public int finishRuleReadCount { get; set; } = 0;

    private bool completed;

    public override void Spawned() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        completed = false;
    }

    public override void FixedUpdateNetwork() {
        if (finishRuleReadCount == RoomConector.Instance.PlayerNum && !completed) {
            if (RoomConector.Instance.networkRunner.IsSharedModeMasterClient) {
                Debug.Log("MyDebug go game");
                completed = true;
                GoGameDelayed(900).Forget();
                RPC_WhiteOut();
            }
        }
    }

    public void PushHasRead() {
        RPC_IncreaseCount();
    }

    private async UniTask GoGameDelayed(int delay) {
        await UniTask.Delay(delay);
        RPC_GameViewAppear();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_WhiteOut() {
        GameObject.Find("FaderCanvas").GetComponent<Fader>().Transit(2.7f);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_GameViewAppear() {
        ViewManager.Instance.playingViewObj.SetActive(true);
        GameManager.Instance.GameStart();
        ViewManager.Instance.ruleExplainViewObj.SetActive(false);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_IncreaseCount() {
        Debug.Log("MyDebug Increase");
        finishRuleReadCount++;
    }
}