﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

public class PlayingView : Photon.PunBehaviour {
    [SerializeField] Image background;

    [SerializeField] TextMeshProUGUI countDownLabel;

    [SerializeField] TMP_Text purposeLabel;

    [SerializeField] List<Image> roundResults;
    [SerializeField] List<Sprite> roundResultImage; /* 0:win 1:lose 2:draw */

    [SerializeField] TMP_Text timerLabel;

    [SerializeField] RectTransform progressBar;
    private Vector2 progressBarDefaultSize;

    [SerializeField] GameObject finishLabel;
    [SerializeField] GameObject winObject, loseObject, drawObject;
    private int hasWin;
    [SerializeField] GameObject test_toZizouButton;

    private ViewManager viewManager;

    private void Awake() {
        progressBarDefaultSize = progressBar.sizeDelta;
        ApplyProgressBar(0.0f);
    }

    public void RoundStart(int round, RuleManager.Rule currentRule) {
        purposeLabel.text = currentRule.explainText;
        for (int i = 0; i < 4; i++) {
            roundResults[i].enabled = false;
        }
        for (int i = 0; i < round - 1; i++) {
            roundResults[i].enabled = true;
            roundResults[i].sprite = roundResultImage[RoundManager.Instance.isWin[i]];
        }

        winObject.SetActive(false);
        loseObject.SetActive(false);
        drawObject.SetActive(false);
        hasWin = -1;
        test_toZizouButton.SetActive(false);

        if (viewManager == null) {
            viewManager = GameObject.Find("ViewManager").GetComponent<ViewManager>();
        }
        progressBar.GetComponent<RectTransform>().sizeDelta = new Vector2(progressBarDefaultSize.x, 0.0f);
    }

    public void BeginningCountDown(int sec) {
        if (sec != 0) {
            countDownLabel.gameObject.SetActive(true);
            countDownLabel.text = sec.ToString();
        } else {
            countDownLabel.gameObject.SetActive(false);
        }
    }

    public void ApplyTimeLimit(int sec) {
        timerLabel.text = (sec / 60).ToString() + ":" + (sec % 60).ToString("D2");
    }

    public void ApplyProgressBar(float progress) {
        progressBar.GetComponent<RectTransform>().sizeDelta = new Vector2(progressBarDefaultSize.x, progressBarDefaultSize.y * progress);
    }

    public async UniTask RoundFinish(int result) {
        finishLabel.SetActive(true);

        await UniTask.Delay(4000);

        finishLabel.SetActive(false);

        hasWin = result;

        switch (result) {
            case 0:
                AppearWinObject().Forget();
                break;
            case 1:
                AppearLoseObject().Forget();
                break;
            case 2:
                AppearDrawObject().Forget();
                break;
            default:
                break;
        }
    }
    public async UniTask AppearWinObject() {
        winObject.SetActive(true);
        test_toZizouButton.SetActive(false);

        await UniTask.Delay(5000);
        PushToZizou();
    }

    public async UniTask AppearLoseObject() {
        loseObject.SetActive(true);
        test_toZizouButton.SetActive(false);

        await UniTask.Delay(5000);
        PushToZizou();
    }

    public async UniTask AppearDrawObject() {
        drawObject.SetActive(true);
        test_toZizouButton.SetActive(false);

        await UniTask.Delay(5000);
        PushToZizou();
    }

    public void PushToZizou() {
        Debug.Log("to zizou");
        photonView.RPC(nameof(ToZizouView), PhotonTargets.AllBuffered);
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
}
