using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.Video;

public class RuleSelectView : Photon.PunBehaviour {

    //static
    [SerializeField] private int selectableRuleNum;

    [SerializeField] private GameObject RuleSubjectButton;
    [SerializeField] private GameObject DecideButton;

    public List<RuleSubjectButton> buttonsList = new List<RuleSubjectButton>();
    private int currentSelectRuleId;

    [SerializeField] private VideoPlayer openingVideo, closingVideo;

    private ViewManager viewManager;

    public async UniTask Set(bool isSelector) {
        //TODO:これ2回呼ばれちゃってんのよ
        Debug.Log("rule select view set");
        /*foreach (var rsb in buttonsList) {
            Destroy(rsb.gameObject);
        }*/
        //buttonsList.Clear();
        for (int i = 0; i < selectableRuleNum; i++) {
            //TODO:randomize
            /*var subject = Instantiate(RuleSubjectButton, RuleSubjectRoot.transform);
            subject.GetComponent<RuleSubjectButton>().SetInfomation(i, this);
            subject.GetComponent<Button>().enabled = isSelector;
            buttonsList.Add(subject.GetComponent<RuleSubjectButton>());
            Debug.Log("rule select view add");
            */
        }

        //TODO:for 1007
        foreach (var subject in buttonsList) {
            subject.GetComponent<RuleSubjectButton>().SetInfomation(this);
            subject.GetComponent<Button>().enabled = isSelector;
        }

        DecideButton.SetActive(isSelector);
        DecideButton.GetComponent<Button>().enabled = false;

        if (viewManager == null) {
            viewManager = GameObject.Find("ViewManager").GetComponent<ViewManager>();
        }

        openingVideo.gameObject.GetComponent<RawImage>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        openingVideo.Play();
        closingVideo.Prepare();
        closingVideo.time = 0f;
        closingVideo.gameObject.GetComponent<RawImage>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        GameManager.Instance.canOperateUI = false;

        await UniTask.Delay(3200);

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
            closingVideo.gameObject.GetComponent<RawImage>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            closingVideo.time = 0f;
            closingVideo.Play();
            openingVideo.gameObject.GetComponent<RawImage>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            openingVideo.time = 0f;
            Decide().Forget();
            GameManager.Instance.canOperateUI = false;
        }
    }

    public async UniTask Decide() { //TODO Archive
        await UniTask.Delay(3200);
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
    }
}