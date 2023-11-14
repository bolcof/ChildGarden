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

    [SerializeField] Image myProgressGuage;
    [SerializeField] TextMeshProUGUI myProgressLabel;

    [SerializeField] List<Image> otherProgressGuages;
    [SerializeField] List<TextMeshProUGUI> otherProgressLabels;

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

    public void BeginningCountDown(int sec) {
        if (sec != 0) {
            countDownLabel.gameObject.SetActive(true);
            countDownLabel.text = sec.ToString();
        } else {
            countDownLabel.gameObject.SetActive(false);
        }
    }

    public void ApplyTimeLimit(int sec) {
        timerLabel.text = sec.ToString();
    }

    public void ApplyProgressBar(float progress) {
        myProgressGuage.fillAmount = progress;
        myProgressLabel.text = (progress * 100).ToString("F0");
        //photonView.RPC(nameof(ApplyOtherProgressGuages), PhotonTargets.Others, RoomConector.Instance.MyPlayerId(), progress);
    }

    public async UniTask RoundFinish(int result) {
        finishLabel.SetActive(true);
        await UniTask.Delay(4000);
        finishLabel.SetActive(false);

        hasWin = result;
        AppearGate(result).Forget();
    }

    public async UniTask AppearGate(int winner /* -1:not yet 0:other 1:me 2:draw */) {
        Debug.Log("AppearGate");
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
    public async UniTask OpenGateToZizou() {
        Debug.Log("OpenGate");
        gateLabel.DOFade(0.0f, 0.25f);
        gateBack.DOFade(0.5f, 0.25f);

        await UniTask.Delay(250);

        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_OpenDoor);
        gateBack.DOFade(0.0f, 0.1f);
        gateR.DOAnchorPos(new Vector2(1500f, 0f), 0.25f);
        gateL.DOAnchorPos(new Vector2(-1500f, 0f), 0.25f);
    }
    public async UniTask OpenGateToNext() {
        Debug.Log("OpenGate");
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
        Debug.Log("CloseGateAndGoNext");
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
            viewManager.ruleSelectView.GetComponent<RuleSelectView>().Set(false).Forget();
        } else if (hasWin == 1) {
            viewManager.ruleSelectView.GetComponent<RuleSelectView>().Set(true).Forget();
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
        otherProgressGuages[cpuId].fillAmount = RuleManager.instance.otherUtsuwaList[cpuId].holdersProgress;
        otherProgressLabels[cpuId].text = (RuleManager.instance.otherUtsuwaList[cpuId].holdersProgress * 100).ToString("F0");
    }
}
