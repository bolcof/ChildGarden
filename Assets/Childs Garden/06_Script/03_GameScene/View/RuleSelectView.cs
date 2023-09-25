using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class RuleSelectView : Photon.PunBehaviour {

    //static
    [SerializeField] private int selectableRuleNum;

    [SerializeField] private GameObject RuleSubjectRoot;
    [SerializeField] private GameObject RuleSubjectButton;
    [SerializeField] private GameObject DecideButton;

    private List<RuleSubjectButton> buttonsList = new List<RuleSubjectButton>();
    private int currentSelectRuleId;

    public void Set(bool isWinner) {
        for (int i = 0; i < selectableRuleNum; i++) {
            var subject = Instantiate(RuleSubjectButton, RuleSubjectRoot.transform);
            //TODO:randomize
            subject.GetComponent<RuleSubjectButton>().SetInfomation(i, this);
            buttonsList.Add(subject.GetComponent<RuleSubjectButton>());
        }
        DecideButton.SetActive(isWinner);
    }

    public void PushRule(int index) {
        foreach(var rsb in buttonsList) {
            rsb.SetHighlight(false);
        }
        buttonsList[index].SetHighlight(true);
        currentSelectRuleId = buttonsList[index].thisButtonsRuleId;
        photonView.RPC(nameof(ChangeOthersHighlight), PhotonTargets.OthersBuffered, currentSelectRuleId);
    }

    public void RepushRule() {
        foreach (var rsb in buttonsList) {
            rsb.SetHighlight(false);
        }
        currentSelectRuleId = -1;
        photonView.RPC(nameof(ChangeOthersHighlight), PhotonTargets.OthersBuffered, currentSelectRuleId);
    }

    public void PushDecide() {
        photonView.RPC(nameof(ToNextRound), PhotonTargets.AllBuffered);
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
    public void ChangeOthersHighlight(int ruleId) {
        foreach(var rsb in buttonsList) {
            rsb.SetHighlight(false);
        }
        switch(ruleId) {
            case -1:
                break;
            default:
                buttonsList[ruleId].SetHighlight(true);
                break;
        }
    }

    [PunRPC]
    public void ToNextRound() {
        gameObject.SetActive(false);
        GameManager.Instance.NextRoundStart();
    }
}