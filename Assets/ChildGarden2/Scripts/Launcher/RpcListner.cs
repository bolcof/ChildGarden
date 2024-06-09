using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cysharp.Threading.Tasks;

public class RpcListner : NetworkBehaviour {
    public override void Spawned() {
        RoomConector.Instance.rpcListner = this;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]

    public void RPC_RuleViewAppear() {
        Debug.Log("MyDebug fusion appear");
        ViewManager.Instance.ruleExplainViewObj.SetActive(true);
        ViewManager.Instance.ruleExplainView.ResetView();
        ViewManager.Instance.launcherViewObj.SetActive(false);
        ViewManager.Instance.matchingView.Disappear().Forget();
    }
}