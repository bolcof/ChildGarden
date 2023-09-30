using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class RuleSelectView : Photon.PunBehaviour {

    //static
    [SerializeField] private int selectableRuleNum;

    [SerializeField] private GameObject RuleSubjectRoot;
    [SerializeField] private GameObject RuleSubjectButton;
    [SerializeField] private GameObject DecideButton;

    public List<RuleSubjectButton> buttonsList = new List<RuleSubjectButton>();
    private int currentSelectRuleId;

    private ViewManager viewManager;

    public async UniTask Set(bool isWinner) {
        Debug.Log("rule select view set");
        for (int i = 0; i < selectableRuleNum; i++) {
            var subject = Instantiate(RuleSubjectButton, RuleSubjectRoot.transform);
            //TODO:randomize
            subject.GetComponent<RuleSubjectButton>().SetInfomation(i, this);
            subject.GetComponent<Button>().enabled = isWinner;
            buttonsList.Add(subject.GetComponent<RuleSubjectButton>());
            Debug.Log("rule select view add");
        }
        DecideButton.SetActive(isWinner);
        DecideButton.GetComponent<Button>().enabled = false;

        if (viewManager == null) {
            viewManager = GameObject.Find("ViewManager").GetComponent<ViewManager>();
        }

        GameManager.Instance.canOperateUI = false;

        await UniTask.Delay(5000);

        GameManager.Instance.canOperateUI = true;
    }

    public void PushRule(int index) {
        foreach (var rsb in buttonsList) {
            rsb.SetHighlight(false);
        }
        buttonsList[index].SetHighlight(true);
        currentSelectRuleId = buttonsList[index].thisButtonsRuleId;
        photonView.RPC(nameof(ChangeOthersHighlight), PhotonTargets.OthersBuffered, currentSelectRuleId);

        DecideButton.GetComponent<Button>().enabled = true;
    }

    public void RepushRule() {
        foreach (var rsb in buttonsList) {
            rsb.SetHighlight(false);
        }
        currentSelectRuleId = -1;
        photonView.RPC(nameof(ChangeOthersHighlight), PhotonTargets.OthersBuffered, currentSelectRuleId);

        DecideButton.GetComponent<Button>().enabled = false;
    }

    public void PushDecide() {
        if (GameManager.Instance.canOperateUI) {
            photonView.RPC(nameof(ToNextRound), PhotonTargets.AllBuffered);
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
    public void ChangeOthersHighlight(int ruleId) {
        foreach (var rsb in buttonsList) {
            rsb.SetHighlight(false);
        }
        currentSelectRuleId = ruleId;
        switch (ruleId) {
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
        RuleManager.instance.SetRule(currentSelectRuleId);
        GameManager.Instance.NextRoundStart();
        foreach (var rsb in buttonsList) {
            Destroy(rsb.gameObject);
        }
        buttonsList.Clear();
    }
}