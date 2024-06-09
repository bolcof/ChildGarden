using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fusion;

public class RuleExplainManager : NetworkBehaviour {
    public static RuleExplainManager Instance;
    [SerializeField] private int finishRuleReadCount;
    private bool completed;

    public override void Spawned() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        completed = false;
        Debug.Log("MyDebug RuleExplainManager Spawned");
    }

    public override void FixedUpdateNetwork() {
        if (RoomConector.Instance.networkRunner.IsSharedModeMasterClient) {
            if (finishRuleReadCount == RoomConector.Instance.PlayerNum && !completed) {
                Debug.Log("go game");
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
        Debug.Log("white");
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
        Debug.Log("Increase");
        finishRuleReadCount++;
    }
}