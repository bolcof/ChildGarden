using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZizouView : Photon.PunBehaviour {
    [SerializeField] GameObject oldZizouObject;
    [SerializeField] GameObject winZizouObject, loseZizouObject;
    [SerializeField] GameObject toRuleSelectButton;

    private bool hasWin;

    private ViewManager viewManager;

    public void Set(bool isWinner) {
        Instantiate(oldZizouObject, new Vector3(0, 0, -0.1f), Quaternion.identity);
        winZizouObject.SetActive(isWinner);
        loseZizouObject.SetActive(!isWinner);
        //TODO:Unitask timing
        toRuleSelectButton.SetActive(isWinner);

        if (viewManager == null) {
            viewManager = GameObject.Find("ViewManager").GetComponent<ViewManager>();
        }
        hasWin = isWinner;
    }

    //これが無いと動くけどエラーが出る
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            // ここにオブジェクトの状態を送信するコードを書きます
        } else {
            // ここにオブジェクトの状態を受信して更新するコードを書きます
        }
    }

    public void PushToRuleSelect() {
        photonView.RPC(nameof(ToRuleSelect), PhotonTargets.AllBuffered);
    }

    [PunRPC]
    public void ToRuleSelect() {
        Debug.Log("Rule Select");
        gameObject.SetActive(false);
        viewManager.ruleSelectViewObj.SetActive(true);
        viewManager.ruleSelectView.GetComponent<RuleSelectView>().Set(hasWin);
    }
}