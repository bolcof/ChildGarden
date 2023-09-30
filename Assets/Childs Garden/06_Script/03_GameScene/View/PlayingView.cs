using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayingView : Photon.PunBehaviour {
    [SerializeField] Image background;
    [SerializeField] TMP_Text purposeLabel;

    [SerializeField] List<Image> roundResults;
    [SerializeField] List<Sprite> roundResultImage;/*0:lose 1:win*/

    [SerializeField] TMP_Text timerLabel;
    [SerializeField] Image progressBar;

    [SerializeField] GameObject winObject, loseObject;
    private bool hasWin;
    [SerializeField] GameObject toRuleSelectButton;

    private ViewManager viewManager;

    public void RoundStart(int round, RuleManager.Rule currentRule) {
        purposeLabel.text = currentRule.explainText;
        for (int i = 0; i < 4; i++) {
            roundResults[i].enabled = false;
        }
        for (int i = 0; i < round - 1; i++) {
            roundResults[i].enabled = true;
            if (RoundManager.Instance.isWin[i]) {
                roundResults[i].sprite = roundResultImage[0];
            } else {
                roundResults[i].sprite = roundResultImage[1];
            }
        }

        winObject.SetActive(false);
        loseObject.SetActive(false);
        hasWin = false;
        toRuleSelectButton.SetActive(false);

        if (viewManager == null) {
            viewManager = GameObject.Find("ViewManager").GetComponent<ViewManager>();
        }
    }

    public void AppearWinObject() {
        winObject.SetActive(true);
        hasWin = true;
        toRuleSelectButton.SetActive(true);
    }

    public void AppearLoseObject() {
        loseObject.SetActive(true);
        hasWin = false;
        toRuleSelectButton.SetActive(false);
    }

    public void PushToRuleSelect() {
        if (RoundManager.Instance.currentRound != RoundManager.Instance.RoundNum) {
            photonView.RPC(nameof(ToZizouView), PhotonTargets.AllBuffered);
        } else {
            photonView.RPC(nameof(ToEndingView), PhotonTargets.AllBuffered);
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
    public void ToZizouView() {
        gameObject.SetActive(false);
        viewManager.zizouViewObj.SetActive(true);
        viewManager.zizouView.GetComponent<ZizouView>().Set(hasWin);
    }

    [PunRPC]
    public void ToEndingView() {
        gameObject.SetActive(false);
        viewManager.endingViewObj.SetActive(true);
        viewManager.endingView.GetComponent<EndingView>().Set(1);
    }
}
