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
        ViewManager.Instance.ruleExplainViewObj.SetActive(true);
        ViewManager.Instance.ruleExplainView.ResetView();
        ViewManager.Instance.launcherViewObj.SetActive(false);
        ViewManager.Instance.matchingView.Disappear().Forget();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_PlayingView_ApplyOtherProgressGuages(int playerId, float progress, RpcInfo info = default) {
        ViewManager.Instance.playingView.ApplyOtherProgressGuages(playerId, progress, info);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_PlayingView_ToRuleSelectFromPlayingView() {
        ViewManager.Instance.playingView.ToRuleSelectFromPlayingView();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayingView_ToEndingView() {
        ViewManager.Instance.playingView.ToEndingView();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayingView_ApplyTimeLimit(int sec) {
        ViewManager.Instance.playingView.ApplyTimeLimit(sec);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ZizouMovie_SetZizouMovieId(int id) {
        ViewManager.Instance.playingView.zizowMovie.SetZizouMovieId(id);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_RuleSelectView_ChangeOthersHighlight(int ruleIndex, RpcInfo info = default) {
        if (info.Source != RoomConector.Instance.networkRunner.LocalPlayer) {
            ViewManager.Instance.ruleSelectView.ChangeOthersHighlight(ruleIndex);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_RuleSelectView_CloseRuleSelectPanel() {
        ViewManager.Instance.ruleSelectView.CloseRuleSelectPanel();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_RuleSelectView_OpenGate() {
        ViewManager.Instance.ruleSelectView.CloseRuleSelectPanel();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_RuleSelectView_ToNextRound() {
        ViewManager.Instance.ruleSelectView.ToNextRound();
    }
}