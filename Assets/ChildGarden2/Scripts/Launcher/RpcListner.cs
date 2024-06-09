using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cysharp.Threading.Tasks;

public class RpcListner : NetworkBehaviour {
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]

    /*public override void Spawned() {
        RoomConector.Instance.rpcListner = this;
    }*/
    public void RPC_RuleViewAppear() {
        Debug.Log("MyDebug fusion appear");
        ViewManager.Instance.ruleExplainViewObj.SetActive(true);
        ViewManager.Instance.ruleExplainView.ResetView();
        ViewManager.Instance.launcherViewObj.SetActive(false);
        ViewManager.Instance.matchingView.Disappear().Forget();
    }
}