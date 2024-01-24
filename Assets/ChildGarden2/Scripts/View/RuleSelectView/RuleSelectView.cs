using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.Video;
using static RuleManager;

public class RuleSelectView : Photon.PunBehaviour {

    //static
    [SerializeField] private int selectableRuleNum;

    [SerializeField] private Image selectorLabel, waiterLabel;
    [SerializeField] private GameObject RuleSubjectButton;
    [SerializeField] private GameObject DecideButton;

    public List<RuleSubjectButton> buttonsList = new List<RuleSubjectButton>();
    private int ruleIndex;

    [SerializeField] private Animator _animator;

    private ViewManager viewManager;

    public async UniTask Set(bool isSelector) {
        //TODO: fuckin code

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
        foreach (var subject in buttonsList) {
            subject.GetComponent<RuleSubjectButton>().SetInfomation(this);
            subject.GetComponent<Button>().enabled = isSelector;
        }
        RepushRule();

        DecideButton.SetActive(isSelector);
        if (isSelector) {
            waiterLabel.gameObject.SetActive(false);
            selectorLabel.gameObject.SetActive(true);
            DecideButton.SetActive(true);
            GameObject.Find("Cursor").GetComponent<CursorBehaviour>().isRuleSelectView = true;
            GameObject.Find("Cursor").GetComponent<CursorBehaviour>().isSelector = true;
        } else {
            waiterLabel.gameObject.SetActive(true);
            selectorLabel.gameObject.SetActive(false);
            DecideButton.SetActive(false);
            GameObject.Find("Cursor").GetComponent<CursorBehaviour>().isRuleSelectView = true;
            GameObject.Find("Cursor").GetComponent<CursorBehaviour>().isSelector = false;
        }
        DecideButton.GetComponent<Button>().enabled = false;

        if (viewManager == null) {
            viewManager = GameObject.Find("ViewManager").GetComponent<ViewManager>();
        }

        SoundManager.Instance.PlayBgm(SoundManager.Instance.BGM_RuleSelect);

        GameManager.Instance.canOperateUI = false;
        _animator.SetBool("Close", false);

        await UniTask.Delay(3200);

        GameManager.Instance.canOperateUI = true;
    }

    public void PushRule(int ruleId) {
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_SelectRule);
        foreach (var rsb in buttonsList) {
            rsb.SetHighlight(false);
        }
        buttonsList.Find(r => r.thisButtonsRuleId == ruleId).SetHighlight(true);
        ruleIndex = ruleId;
        photonView.RPC(nameof(ChangeOthersHighlight), PhotonTargets.OthersBuffered, ruleIndex);

        DecideButton.GetComponent<Button>().enabled = true;
    }

    public void RepushRule() {
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_SelectRule);
        foreach (var rsb in buttonsList) {
            rsb.SetHighlight(false);
        }
        ruleIndex = -1;
        photonView.RPC(nameof(ChangeOthersHighlight), PhotonTargets.OthersBuffered, ruleIndex);

        DecideButton.GetComponent<Button>().enabled = false;
    }

    public void PushDecide() {
        if (GameManager.Instance.canOperateUI) {
            Decide().Forget();
            SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_ClickGo);
            GameManager.Instance.canOperateUI = false;
        }
    }

    public void PushBack() {
        if (GameObject.Find("Cursor").GetComponent<CursorBehaviour>().isSelector) {
            SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_WholeClick);
        }
    }

    public async UniTask Decide() { //TODO Archive
        photonView.RPC(nameof(CloseRuleSelectPanel), PhotonTargets.AllBuffered);
        await UniTask.Delay(3200);
        photonView.RPC(nameof(OpenGate), PhotonTargets.AllBuffered);
        await UniTask.Delay(500);
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
        ruleIndex = ruleId;
        switch (ruleId) {
            case -1:
                break;
            default:
                //TODO FIX AppearedId
                buttonsList.Find(r => r.thisButtonsRuleId == ruleId).SetHighlight(true);
                break;
        }
    }
    [PunRPC]
    public void CloseRuleSelectPanel() {
        _animator.SetBool("Close", true);
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_RuleSelectViewClosing);
    }

    [PunRPC]
    public void OpenGate() {
        ViewManager.Instance.playingView.OpenGateToNext().Forget();
    }

    [PunRPC]
    public void ToNextRound() {
        gameObject.SetActive(false);
        RuleManager.instance.SetRule(ruleIndex);
        GameObject.Find("Cursor").GetComponent<CursorBehaviour>().isRuleSelectView = false;
        GameManager.Instance.NextRoundStart();
    }
}