using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fusion;

public class PlayingView : MonoBehaviour {

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
    [SerializeField] private List<GameObject> resultLabels = new List<GameObject>();
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
    }

    public async UniTask RoundFinish(int result) {
        finishLabel.SetActive(true);
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_RoundFinish);
        await UniTask.Delay(4000);
        finishLabel.SetActive(false);

        hasWin = result;
        CloseNewGate(result).Forget();
    }

    public async UniTask CloseNewGate(int winner /* -1:not yet 0:other 1:me 2:draw */) {
        Debug.Log("Close New Gate");
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_CloseNewDoor);
        offedScreen.enabled = true;
        foreach (var l in resultLabels) {
            l.SetActive(false);
        }

        var doorBaseSpeed = 0.36f;
        newGateLB.DOAnchorPos(new Vector2(0f, 0f), doorBaseSpeed);
        newGateRB.DOAnchorPos(new Vector2(0f, 0f), doorBaseSpeed);
        newGateLF.DOAnchorPos(new Vector2(0f, 0f), doorBaseSpeed);
        newGateRF.DOAnchorPos(new Vector2(0f, 0f), doorBaseSpeed);

        await UniTask.Delay(600);

        var topSequence = DOTween.Sequence();
        var topBarSpeed = 0.051f;
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
        var underBarSpeed = 0.153f;
        underSequence
            .Append(underBars[0].DOAnchorPos(new Vector2(0f, 0f), underBarSpeed))
            .Append(underBars[1].DOAnchorPos(new Vector2(0f, 0f), underBarSpeed))
            .Append(underBars[2].DOAnchorPos(new Vector2(0f, 0f), underBarSpeed))
            .Append(underBars[3].DOAnchorPos(new Vector2(0f, 0f), underBarSpeed));

        await UniTask.Delay(1100);
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_NewDoorBlink);

        offedScreen.enabled = false;

        await UniTask.Delay(1000);
        resultLabels[winner].SetActive(true);

        await UniTask.Delay(500);
        resultLabels[winner].SetActive(false);

        await UniTask.Delay(500);
        resultLabels[winner].SetActive(true);

        await UniTask.Delay(500);
        resultLabels[winner].SetActive(false);

        await UniTask.Delay(500);
        resultLabels[winner].SetActive(true);

        await UniTask.Delay(1800);

        PlayZizowMovie();

        await UniTask.Delay(200);
        OpenNewGate(false).Forget();
    }

    public async UniTask OpenNewGate(bool isZizowMovieClose) {
        Debug.Log("Open New Gate");
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_OpenNewDoor);

        foreach (var l in resultLabels) {
            l.SetActive(false);
        }
        offedScreen.enabled = false;
        resultLabels[hasWin].SetActive(true);

        var underSequence = DOTween.Sequence();
        var underBarSpeed = 0.075f;
        underSequence
            .Append(underBars[0].DOAnchorPos(new Vector2(0f, -300f), underBarSpeed))
            .Append(underBars[1].DOAnchorPos(new Vector2(0f, -300f), underBarSpeed))
            .Append(underBars[2].DOAnchorPos(new Vector2(0f, -300f), underBarSpeed))
            .Append(underBars[3].DOAnchorPos(new Vector2(0f, -300f), underBarSpeed));

        var topBarSpeed = 0.033f;

        for (int i = 12; i >= 0; i--) {
            topBars[i].DOAnchorPos(new Vector2(0, topBarSpeed * (i+1)), topBarSpeed);
            await UniTask.Delay(33);
        }

        await UniTask.Delay(400);

        if (isZizowMovieClose) {
            zizowMovie.gameObject.SetActive(false);
        }

        var doorBaseSpeed = 0.36f;
        newGateLB.DOAnchorPos(new Vector2(-1300f, 0f), doorBaseSpeed);
        newGateRB.DOAnchorPos(new Vector2(1620f, 0f), doorBaseSpeed);
        newGateLF.DOAnchorPos(new Vector2(-1300f, 0f), doorBaseSpeed);
        newGateRF.DOAnchorPos(new Vector2(1620f, 0f), doorBaseSpeed);
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

    public async UniTask CloseNewGateAndGoNext() {
        if (RoundManager.Instance.currentRound != RoundManager.Instance.RoundNum) {
            Debug.Log("Close New Gate And Go Next");
            SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_CloseNewDoor);

            var doorBaseSpeed = 0.36f;
            newGateLB.DOAnchorPos(new Vector2(0f, 0f), doorBaseSpeed);
            newGateRB.DOAnchorPos(new Vector2(0f, 0f), doorBaseSpeed);
            newGateLF.DOAnchorPos(new Vector2(0f, 0f), doorBaseSpeed);
            newGateRF.DOAnchorPos(new Vector2(0f, 0f), doorBaseSpeed);

            await UniTask.Delay(600);

            topBarRoot.SetActive(true);

            var topSequence = DOTween.Sequence();
            var topBarSpeed = 0.051f;
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
            var underBarSpeed = 0.153f;
            underSequence
                .Append(underBars[0].DOAnchorPos(new Vector2(0f, 0f), underBarSpeed))
                .Append(underBars[1].DOAnchorPos(new Vector2(0f, 0f), underBarSpeed))
                .Append(underBars[2].DOAnchorPos(new Vector2(0f, 0f), underBarSpeed))
                .Append(underBars[3].DOAnchorPos(new Vector2(0f, 0f), underBarSpeed));

            await UniTask.Delay(1100);
        }
    }

    public void PlayZizowMovie() {
        Debug.Log("PlayZizowMovie");
        zizowMovie.gameObject.SetActive(true);
        zizowMovie.Set(hasWin);
    }

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

    public void ToEndingView() {
        Debug.Log("To Ending View");
        gameObject.SetActive(false);
        viewManager.endingViewObj.SetActive(true);
        viewManager.endingView.GetComponent<EndingView>().Set();
    }

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
