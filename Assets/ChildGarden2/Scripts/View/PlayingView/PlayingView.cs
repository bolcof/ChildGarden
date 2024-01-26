using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class PlayingView : Photon.PunBehaviour {

    public GameObject countDownObject;

    [SerializeField] TMP_Text purposeLabel;

    [SerializeField] List<Image> roundResults;
    [SerializeField] List<Sprite> roundResultImage; /* 0:win 1:lose 2:draw */

    [SerializeField] TMP_Text timerLabel;

    [SerializeField] Image myProgressGuage;
    [SerializeField] TextMeshProUGUI myProgressLabel;

    [SerializeField] List<Image> otherProgressGuages;
    [SerializeField] List<TextMeshProUGUI> otherProgressLabels;

    public AngelSpeaking angelSpeaking;

    [SerializeField] private GameObject finishLabel;

    [SerializeField] private Image gateBack;
    [SerializeField] private RectTransform gateR, gateL;
    [SerializeField] private Image gateLabel;
    [SerializeField] private List<Sprite> gateR_Images = new List<Sprite>();
    [SerializeField] private List<Sprite> gateL_Images = new List<Sprite>();
    [SerializeField] private List<Sprite> gateC_Images = new List<Sprite>();

    [SerializeField] private RectTransform newGateRF, newGateLF, newGateRB, newGateLB;
    [SerializeField] private Image offedScreen;
    [SerializeField] private List<Image> resultLabels = new List<Image>();
    [SerializeField] private List<RectTransform> underBars = new List<RectTransform>();
    [SerializeField] private GameObject topBarRoot;
    [SerializeField] private List<RectTransform> topBars = new List<RectTransform>();
    [SerializeField] private float topBarXPosDiff;

    private int hasWin;
    [SerializeField] private ZizouMovie zizowMovie;

    private ViewManager viewManager;

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
        myProgressGuage.fillAmount = 0.0f;
        myProgressLabel.text = "0";
        for (int i = 0; i < otherProgressGuages.Count; i++) {
            otherProgressGuages[i].fillAmount = 0.0f;
            otherProgressLabels[i].text = "0";
        }
    }

    public void ApplyTimeLimit(int sec) {
        timerLabel.text = sec.ToString();
    }

    public void ApplyProgressBar(float progress) {
        myProgressGuage.fillAmount = progress;
        myProgressLabel.text = (progress * 100).ToString("F0");
        photonView.RPC(nameof(ApplyOtherProgressGuages), PhotonTargets.Others, RoomConector.Instance.MyPlayerId(), progress);
    }

    public async UniTask RoundFinish(int result) {
        finishLabel.SetActive(true);
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_RoundFinish);
        await UniTask.Delay(4000);
        finishLabel.SetActive(false);

        hasWin = result;
        //AppearGate(result).Forget();
        CloseNewGate(result).Forget();
    }

    public async UniTask AppearGate(int winner /* -1:not yet 0:other 1:me 2:draw */) {
        Debug.Log("AppearGate old");
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_OpenDoor);

        gateR.gameObject.GetComponent<Image>().sprite = gateR_Images[winner];
        gateL.gameObject.GetComponent<Image>().sprite = gateL_Images[winner];
        gateLabel.sprite = gateC_Images[winner];

        gateR.DOAnchorPos(new Vector2(480f, 0f), 0.25f);
        gateL.DOAnchorPos(new Vector2(-480f, 0f), 0.25f);
        await UniTask.Delay(250);
        gateBack.DOFade(0.75f, 0.25f);
        gateLabel.DOFade(1.0f, 0.25f);

        await UniTask.Delay(3000);
        PlayZizowMovie();

        await UniTask.Delay(250);
        OpenGateToZizou().Forget();
    }

    public async UniTask CloseNewGate(int winner /* -1:not yet 0:other 1:me 2:draw */) {
        Debug.Log("Close New Gate");
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_OpenDoor);
        offedScreen.enabled = true;
        foreach (var l in resultLabels) {
            l.enabled = false;
        }

        newGateLB.DOAnchorPos(new Vector2(0f, 0f), 0.28f);
        newGateRB.DOAnchorPos(new Vector2(0f, 0f), 0.28f);
        newGateLF.DOAnchorPos(new Vector2(0f, 0f), 0.28f);
        newGateRF.DOAnchorPos(new Vector2(0f, 0f), 0.28f);

        await UniTask.Delay(450);

        var topSequence = DOTween.Sequence();
        var topBarSpeed = 0.065f;
        topSequence
            .Append(topBars[0].DOAnchorPos(new Vector2(0f, 0f), topBarSpeed))
            .Append(topBars[1].DOAnchorPos(new Vector2(topBarXPosDiff, 0f), topBarSpeed))
            .Append(topBars[2].DOAnchorPos(new Vector2(topBarXPosDiff * 2, 0f), topBarSpeed))
            .Append(topBars[3].DOAnchorPos(new Vector2(topBarXPosDiff * 3, 0f), topBarSpeed))
            .Append(topBars[4].DOAnchorPos(new Vector2(topBarXPosDiff * 4, 0f), topBarSpeed))
            .Append(topBars[5].DOAnchorPos(new Vector2(topBarXPosDiff * 5, 0f), topBarSpeed))
            .Append(topBars[6].DOAnchorPos(new Vector2(topBarXPosDiff * 6, 0f), topBarSpeed))
            .Append(topBars[7].DOAnchorPos(new Vector2(topBarXPosDiff * 7, 0f), topBarSpeed))
            .Append(topBars[8].DOAnchorPos(new Vector2(topBarXPosDiff * 8, 0f), topBarSpeed))
            .Append(topBars[9].DOAnchorPos(new Vector2(topBarXPosDiff * 9, 0f), topBarSpeed))
            .Append(topBars[10].DOAnchorPos(new Vector2(topBarXPosDiff * 10, 0f), topBarSpeed))
            .Append(topBars[11].DOAnchorPos(new Vector2(topBarXPosDiff * 11, 0f), topBarSpeed))
            .Append(topBars[12].DOAnchorPos(new Vector2(topBarXPosDiff * 12, 0f), topBarSpeed));

        var underSequence = DOTween.Sequence();
        var underBarSpeed = 0.18f;
        underSequence
            .Append(underBars[0].DOAnchorPos(new Vector2(0f, 0f), underBarSpeed))
            .Append(underBars[1].DOAnchorPos(new Vector2(0f, 0f), underBarSpeed))
            .Append(underBars[2].DOAnchorPos(new Vector2(0f, 0f), underBarSpeed))
            .Append(underBars[3].DOAnchorPos(new Vector2(0f, 0f), underBarSpeed));

        await UniTask.Delay(800);

        offedScreen.enabled = false;

        await UniTask.Delay(540);
        resultLabels[winner].enabled = true;

        await UniTask.Delay(225);
        resultLabels[winner].enabled = false;

        await UniTask.Delay(225);
        resultLabels[winner].enabled = true;

        await UniTask.Delay(225);
        resultLabels[winner].enabled = false;

        await UniTask.Delay(225);
        resultLabels[winner].enabled = true;

        await UniTask.Delay(2400);

        PlayZizowMovie();

        await UniTask.Delay(250);
        OpenNewGate(false).Forget();
    }

    public async UniTask OpenNewGate(bool isZizowMovieClose) {
        Debug.Log("Open New Gate");
        foreach(var l in resultLabels) {
            l.enabled = false;
        }
        offedScreen.enabled = false;
        resultLabels[hasWin].enabled = true;

        var underSequence = DOTween.Sequence();
        var underBarSpeed = 0.1f;
        underSequence
            .Append(underBars[0].DOAnchorPos(new Vector2(0f, -300f), underBarSpeed))
            .Append(underBars[1].DOAnchorPos(new Vector2(0f, -300f), underBarSpeed))
            .Append(underBars[2].DOAnchorPos(new Vector2(0f, -300f), underBarSpeed))
            .Append(underBars[3].DOAnchorPos(new Vector2(0f, -300f), underBarSpeed));

        var topBarSpeed = 0.036f;

        for (int i = 12; i >= 0; i--) {
            topBars[i].DOAnchorPos(new Vector2(0, topBarSpeed * (i+1)), topBarSpeed);
            await UniTask.Delay(36);
        }

        await UniTask.Delay(150);

        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_OpenDoor);
        if (isZizowMovieClose) {
            zizowMovie.gameObject.SetActive(false);
        }

        newGateLB.DOAnchorPos(new Vector2(-1300f, 0f), 0.28f);
        newGateRB.DOAnchorPos(new Vector2(1620f, 0f), 0.28f);
        newGateLF.DOAnchorPos(new Vector2(-1300f, 0f), 0.28f);
        newGateRF.DOAnchorPos(new Vector2(1620f, 0f), 0.28f);
    }

    public async UniTask OpenGateToZizou() {
        Debug.Log("OpenGate old");
        gateLabel.DOFade(0.0f, 0.25f);
        gateBack.DOFade(0.5f, 0.25f);

        await UniTask.Delay(250);

        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_OpenDoor);
        gateBack.DOFade(0.0f, 0.1f);
        gateR.DOAnchorPos(new Vector2(1500f, 0f), 0.25f);
        gateL.DOAnchorPos(new Vector2(-1500f, 0f), 0.25f);
    }
    public async UniTask OpenGateToNext() {
        Debug.Log("OpenGate old");
        gateLabel.DOFade(0.0f, 0.25f);
        gateBack.DOFade(0.5f, 0.25f);

        await UniTask.Delay(250);

        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_OpenDoor);
        zizowMovie.gameObject.SetActive(false);
        gateBack.DOFade(0.0f, 0.1f);
        gateR.DOAnchorPos(new Vector2(1500f, 0f), 0.25f);
        gateL.DOAnchorPos(new Vector2(-1500f, 0f), 0.25f);
    }

    public async UniTask CloseGateAndGoNext() {
        Debug.Log("CloseGateAndGoNext old");
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_CloseDoor);

        gateR.DOAnchorPos(new Vector2(480f, 0f), 0.25f);
        gateL.DOAnchorPos(new Vector2(-480f, 0f), 0.25f);
        await UniTask.Delay(250);
        gateBack.DOFade(0.75f, 0.25f);
        gateLabel.DOFade(1.0f, 0.25f);

        await UniTask.Delay(500);

        if (PhotonNetwork.isMasterClient) {
            if (RoundManager.Instance.currentRound != RoundManager.Instance.RoundNum) {
                photonView.RPC(nameof(ToRuleSelectFromPlayingView), PhotonTargets.AllBuffered);
            } else {
                photonView.RPC(nameof(ToEndingView), PhotonTargets.AllBuffered);
            }
        }
    }

    public async UniTask CloseNewGateAndGoNext() {
        Debug.Log("Close New Gate And Go Next");
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_CloseDoor);

        newGateLB.DOAnchorPos(new Vector2(0f, 0f), 0.28f);
        newGateRB.DOAnchorPos(new Vector2(0f, 0f), 0.28f);
        newGateLF.DOAnchorPos(new Vector2(0f, 0f), 0.28f);
        newGateRF.DOAnchorPos(new Vector2(0f, 0f), 0.28f);

        await UniTask.Delay(450);

        topBarRoot.SetActive(true);

        var topSequence = DOTween.Sequence();
        var topBarSpeed = 0.065f;
        topSequence
            .Append(topBars[0].DOAnchorPos(new Vector2(0f, 0f), topBarSpeed))
            .Append(topBars[1].DOAnchorPos(new Vector2(topBarXPosDiff, 0f), topBarSpeed))
            .Append(topBars[2].DOAnchorPos(new Vector2(topBarXPosDiff * 2, 0f), topBarSpeed))
            .Append(topBars[3].DOAnchorPos(new Vector2(topBarXPosDiff * 3, 0f), topBarSpeed))
            .Append(topBars[4].DOAnchorPos(new Vector2(topBarXPosDiff * 4, 0f), topBarSpeed))
            .Append(topBars[5].DOAnchorPos(new Vector2(topBarXPosDiff * 5, 0f), topBarSpeed))
            .Append(topBars[6].DOAnchorPos(new Vector2(topBarXPosDiff * 6, 0f), topBarSpeed))
            .Append(topBars[7].DOAnchorPos(new Vector2(topBarXPosDiff * 7, 0f), topBarSpeed))
            .Append(topBars[8].DOAnchorPos(new Vector2(topBarXPosDiff * 8, 0f), topBarSpeed))
            .Append(topBars[9].DOAnchorPos(new Vector2(topBarXPosDiff * 9, 0f), topBarSpeed))
            .Append(topBars[10].DOAnchorPos(new Vector2(topBarXPosDiff * 10, 0f), topBarSpeed))
            .Append(topBars[11].DOAnchorPos(new Vector2(topBarXPosDiff * 11, 0f), topBarSpeed))
            .Append(topBars[12].DOAnchorPos(new Vector2(topBarXPosDiff * 12, 0f), topBarSpeed));

        var underSequence = DOTween.Sequence();
        var underBarSpeed = 0.18f;
        underSequence
            .Append(underBars[0].DOAnchorPos(new Vector2(0f, 0f), underBarSpeed))
            .Append(underBars[1].DOAnchorPos(new Vector2(0f, 0f), underBarSpeed))
            .Append(underBars[2].DOAnchorPos(new Vector2(0f, 0f), underBarSpeed))
            .Append(underBars[3].DOAnchorPos(new Vector2(0f, 0f), underBarSpeed));

        await UniTask.Delay(500);

        if (PhotonNetwork.isMasterClient) {
            if (RoundManager.Instance.currentRound != RoundManager.Instance.RoundNum) {
                photonView.RPC(nameof(ToRuleSelectFromPlayingView), PhotonTargets.AllBuffered);
            } else {
                photonView.RPC(nameof(ToEndingView), PhotonTargets.AllBuffered);
            }
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

    public void PlayZizowMovie() {
        Debug.Log("PlayZizowMovie");
        zizowMovie.gameObject.SetActive(true);
        zizowMovie.Set(hasWin);
    }

    [PunRPC]
    public void ToRuleSelectFromPlayingView() {
        Debug.Log("To Rule Select from PlayingView");
        viewManager.ruleSelectViewObj.SetActive(true);
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_RuleSelectViewOpening);
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
        Debug.Log("To Ending View");
        gameObject.SetActive(false);
        viewManager.endingViewObj.SetActive(true);
        viewManager.endingView.GetComponent<EndingView>().Set();
    }

    [PunRPC]
    public void ApplyOtherProgressGuages(int playerId, float progress) {
        int cpuId = RuleManager.instance.otherUtsuwaList.Find(u => u.holderId == playerId).CpuId;
        //Debug.Log("aaaa " + playerId.ToString() + ", " + cpuId.ToString() + ", " + progress.ToString());
        if (!RuleManager.instance.nearToLoseAppeared && progress >= 0.8f) {
            angelSpeaking.NearToLose().Forget();
            RuleManager.instance.nearToLoseAppeared = true;
        }
        otherProgressGuages[cpuId].fillAmount = progress;
        otherProgressLabels[cpuId].text = (progress * 100).ToString("F0");
    }
}
