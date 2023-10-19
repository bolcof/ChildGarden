using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class PlayingView : Photon.PunBehaviour {
    [SerializeField] Image background;

    [SerializeField] TextMeshProUGUI countDownLabel;

    [SerializeField] TMP_Text purposeLabel;

    [SerializeField] List<Image> roundResults;
    [SerializeField] List<Sprite> roundResultImage; /* 0:win 1:lose 2:draw */

    [SerializeField] TMP_Text timerLabel;

    [SerializeField] RectTransform progressBar;
    private Vector2 progressBarDefaultSize;

    [SerializeField] private GameObject finishLabel;
    [SerializeField] private Image gateBack;
    [SerializeField] private RectTransform gateR, gateL;
    [SerializeField] private Image gateLabel;
    [SerializeField] private List<Sprite> gateR_Images = new List<Sprite>();
    [SerializeField] private List<Sprite> gateL_Images = new List<Sprite>();
    [SerializeField] private List<Sprite> gateC_Images = new List<Sprite>();

    private int hasWin;
    [SerializeField] private ZizouMovie zizowMovie;

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
        hasWin = -1;

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
        AppearGate(result).Forget();
    }

    public async UniTask AppearGate(int winner /* -1:not yet 0:other 1:me 2:draw */) {
        gateR.gameObject.GetComponent<Image>().sprite = gateR_Images[winner];
        gateL.gameObject.GetComponent<Image>().sprite = gateL_Images[winner];
        gateLabel.sprite = gateC_Images[winner];

        gateR.DOAnchorPos(new Vector2(480f, 0f), 0.25f);
        gateL.DOAnchorPos(new Vector2(-480f, 0f), 0.25f);
        await UniTask.Delay(250);
        gateBack.DOFade(0.75f, 0.25f);
        gateLabel.DOFade(1.0f, 0.25f);

        await UniTask.Delay(3000);
        photonView.RPC(nameof(PlayZizowMovie), PhotonTargets.AllBuffered);

        await UniTask.Delay(250);
        OpenGate().Forget();
    }
    public async UniTask OpenGate() {
        gateLabel.DOFade(0.0f, 0.25f);
        gateBack.DOFade(0.5f, 0.25f);
        await UniTask.Delay(250);
        gateR.DOAnchorPos(new Vector2(1500f, 0f), 0.25f);
        gateL.DOAnchorPos(new Vector2(-1500f, 0f), 0.25f);
    }

    public async UniTask CloseGateAndGoNext() {
        gateR.DOAnchorPos(new Vector2(480f, 0f), 0.25f);
        gateL.DOAnchorPos(new Vector2(-480f, 0f), 0.25f);
        await UniTask.Delay(250);
        gateBack.DOFade(0.75f, 0.25f);
        gateLabel.DOFade(1.0f, 0.25f);

        await UniTask.Delay(500);
        if (RoundManager.Instance.currentRound != RoundManager.Instance.RoundNum) {
            photonView.RPC(nameof(ToRuleSelect), PhotonTargets.AllBuffered);
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
    public void PlayZizowMovie() {
        gameObject.SetActive(false);
        zizowMovie.gameObject.SetActive(true);
        zizowMovie.Set(hasWin);
    }

    [PunRPC]
    public void ToRuleSelect() {
        gameObject.SetActive(false);
        viewManager.ruleSelectViewObj.SetActive(true);
        if (hasWin == 0) {
            viewManager.ruleSelectView.GetComponent<RuleSelectView>().Set(true).Forget();
        } else if (hasWin == 1) {
            viewManager.ruleSelectView.GetComponent<RuleSelectView>().Set(false).Forget();
        } else {
            //TODO:selector
            viewManager.ruleSelectView.GetComponent<RuleSelectView>().Set(PhotonNetwork.isMasterClient).Forget();
        }
    }

    [PunRPC]
    public void ToEndingView() {
        gameObject.SetActive(false);
        viewManager.endingViewObj.SetActive(true);
        viewManager.endingView.GetComponent<EndingView>().Set();
    }
}
