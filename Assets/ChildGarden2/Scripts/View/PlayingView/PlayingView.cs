﻿using System.Collections;
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

    [SerializeField] private GameObject prayElements;
    private Vector3 prayElementsTargetPosition;
    private bool canPray = true;
    private bool effectPray = true;
    [SerializeField] private GameObject idleAnimObj;
    [SerializeField] private GameObject prayAnimObj;

    [SerializeField] private GameObject prayButtonEffectPrefab;
    private GameObject prayButtonEffect;
    public float fadeInDuration = 2.0f;
    public float fadeOutDuration = 2.0f;

    [SerializeField] private GameObject finishLabel;
    [SerializeField] private GameObject finishScreen;
    [SerializeField] private Image frontLightImage; 
    [SerializeField] private Image backLightImage;

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
    //public RandomZizouMovie zizowMovie;
    [SerializeField] SortedZizouMovie sortedZizouMovie;

    private ViewManager viewManager;

    public void RoundStart(int round, RuleManager.Rule currentRule) {
        purposeLabel.text = currentRule.explainText;

        // 初期の透明度を0に設定
        LightTextAlpha(frontLightImage, 0f);
        LightTextAlpha(backLightImage, 0f);
        LightTextAlpha(purposeLabel, 0f);
        LightTextAlpha(timerLabel, 0f);
        LightTextAlpha(myProgressGuage, 0f);
        LightTextAlpha(myProgressLabel, 0f);

        foreach (var guage in otherProgressGuages)
        {
            LightTextAlpha(guage, 0f);
        }

        foreach (var label in otherProgressLabels)
        {
            LightTextAlpha(label, 0f);
        }
        
        // 2秒のディレイの後、2秒かけてフェードイン
        frontLightImage.DOFade(1f, 2f).SetDelay(2f);
        backLightImage.DOFade(1f, 2f).SetDelay(2f);
        purposeLabel.DOFade(1f, 2f).SetDelay(2f);
        timerLabel.DOFade(1f, 2f).SetDelay(2f);
        myProgressGuage.DOFade(1f, 2f).SetDelay(2f);
        myProgressLabel.DOFade(1f, 2f).SetDelay(2f);

        foreach (var guage in otherProgressGuages)
        {
            guage.DOFade(1f, 2f).SetDelay(2f);
        }

        foreach (var label in otherProgressLabels)
        {
            label.DOFade(1f, 2f).SetDelay(2f);
        }

        for (int i = 0; i < 4; i++) {
            roundResults[i].enabled = false;
        }
        for (int i = 0; i < round - 1; i++) {
            roundResults[i].enabled = true;
            roundResults[i].sprite = roundResultImage[RoundManager.instance.isWin[i]];
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

        if (round == 1) {
            prayElementsTargetPosition = prayElements.transform.position;
            SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_AppearPray);
        }
        prayElements.transform.position = prayElementsTargetPosition + new Vector3(500.0f, 0.0f, 0.0f);
    }
    
    private void LightTextAlpha(Graphic graphic, float alpha)
    {
        Color color = graphic.color;
        color.a = alpha;
        graphic.color = color;
    }


    public void ApplyTimeLimit(int sec) {
        timerLabel.text = sec.ToString();
    }

    public void ApplyProgressBar(float progress) {
        myProgressGuage.fillAmount = progress;
        myProgressLabel.text = (progress * 100).ToString("F0");
        RoomConector.Instance.rpcListner.RPC_PlayingView_ApplyOtherProgressGuages(RoomConector.Instance.MyPlayerId(), progress);
    }

    public void ApplyOtherProgressGuages(int playerId, float progress, RpcInfo info) {
        if (info.Source != RoomConector.Instance.networkRunner.LocalPlayer) {
            int cpuId = RuleManager.instance.otherUtsuwaList.Find(u => u.holderId == playerId).CpuId;
            if (!RuleManager.instance.nearToLoseAppeared && progress >= 0.8f) {
                angelSpeaking.NearToLose().Forget();
                RuleManager.instance.nearToLoseAppeared = true;
            }
            otherProgressGuages[cpuId].fillAmount = progress;
            otherProgressLabels[cpuId].text = (progress * 100).ToString("F0");
        }
    }

    public void AppearPrayButton() {
        prayElements.SetActive(true);
        prayElements.transform.DOMove(prayElementsTargetPosition, 0.5f);
        canPray = true;
        AppearPrayButtonEffect();
        effectPray = false;
    }

    public void AppearPrayButtonEffect(){
        if(effectPray){ 
            Vector3 spawnPosition = new Vector3(4.2f, 2.6f, 0f);
            prayButtonEffect = Instantiate(prayButtonEffectPrefab, spawnPosition, Quaternion.identity);
        }
    }

    public void PushPrayButton() {
        if (canPray)
        {
            GameManager.Instance.MyPlayerWin();
            canPray = false;
            idleAnimObj.SetActive(false);
            prayAnimObj.SetActive(true);             
        }
    }

    public async UniTask RoundFinish(int result) {
        finishLabel.SetActive(true);
        finishScreen.SetActive(true);
        frontLightImage.DOFade(0f, 2f);
        backLightImage.DOFade(0f, 2f);
        purposeLabel.DOFade(0f, 2f);
        timerLabel.DOFade(0f, 2f);
        myProgressGuage.DOFade(0f, 2f);
        myProgressLabel.DOFade(0f, 2f);
        effectPray = true;
        Destroy(prayButtonEffect); 

        prayElements.transform.DOMove(prayElementsTargetPosition + new Vector3(500.0f, 0.0f, 0.0f), 0.5f);

        foreach (var guage in otherProgressGuages)
        {
            guage.DOFade(0f, 2f);
        }

        foreach (var label in otherProgressLabels)
        {
            label.DOFade(0f, 2f);
        }

        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_RoundFinish);
        await UniTask.Delay(4000);
        hasWin = result;
        CloseNewGate(result).Forget();
    }

    public async UniTask CloseNewGate(int winner /* -1:not yet 0:other 1:me 2:draw */) {
        Debug.Log("Close New Gate");
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_CloseNewDoor);
        offedScreen.enabled = true;
        idleAnimObj.SetActive(true);
        prayAnimObj.SetActive(false);
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

        GameManager.Instance.ResetWorld();
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
        finishLabel.SetActive(false);
        finishScreen.SetActive(false);

        var underSequence = DOTween.Sequence();
        var underBarSpeed = 0.075f;
        underSequence
            .Append(underBars[0].DOAnchorPos(new Vector2(0f, -300f), underBarSpeed))
            .Append(underBars[1].DOAnchorPos(new Vector2(0f, -300f), underBarSpeed))
            .Append(underBars[2].DOAnchorPos(new Vector2(0f, -300f), underBarSpeed))
            .Append(underBars[3].DOAnchorPos(new Vector2(0f, -300f), underBarSpeed));

        var topBarSpeed = 0.033f;

        for (int i = 12; i >= 0; i--) {
            topBars[i].DOAnchorPos(new Vector2(0, topBarSpeed * (i + 1)), topBarSpeed);
            await UniTask.Delay(33);
        }

        await UniTask.Delay(400);

        if (isZizowMovieClose) {
            sortedZizouMovie.gameObject.SetActive(false);
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
        prayElements.SetActive(false);
        await UniTask.Delay(250);

        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_OpenDoor);
        sortedZizouMovie.gameObject.SetActive(false);
        gateBack.DOFade(0.0f, 0.1f);
        gateR.DOAnchorPos(new Vector2(1500f, 0f), 0.25f);
        gateL.DOAnchorPos(new Vector2(-1500f, 0f), 0.25f);
    }

    public async UniTask CloseNewGateAndGoNext() {
        if (RoundManager.instance.currentRound != RoundManager.instance.RoundNum) {
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

        if (RoundManager.instance.currentRound != RoundManager.instance.RoundNum) {
            RoomConector.Instance.rpcListner.RPC_PlayingView_ToRuleSelectFromPlayingView();
        } else {
            RoomConector.Instance.rpcListner.RPC_PlayingView_ToEndingView();
        }
    }

    public void PlayZizowMovie() {
        Debug.Log("PlayZizowMovie");
        sortedZizouMovie.gameObject.SetActive(true);
        sortedZizouMovie.Set(hasWin);
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
            viewManager.ruleSelectView.GetComponent<RuleSelectView>().Set(RoomConector.Instance.networkRunner.IsSharedModeMasterClient).Forget();
        }
    }

    public void ToEndingView() {
        Debug.Log("To Ending View");
        gameObject.SetActive(false);
        viewManager.endingViewObj.SetActive(true);
        viewManager.endingView.GetComponent<EndingView>().Set();
    }
}
