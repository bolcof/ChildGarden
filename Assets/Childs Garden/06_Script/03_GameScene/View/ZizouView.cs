using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Video;
using Cysharp.Threading.Tasks;

public class ZizouView : Photon.PunBehaviour {
    [SerializeField] List<VideoClip> zizouVideoList = new List<VideoClip>();
    [SerializeField] VideoPlayer zizouVideoPlayer;
    [SerializeField] GameObject zizouRoot;
    [SerializeField] TextMeshProUGUI zizouInfoLabel;
    [SerializeField] GameObject toRuleSelectButton;

    private bool hasWin;

    private ViewManager viewManager;

    private void Awake() {
        zizouVideoPlayer.loopPointReached += PushToRuleSelect;
    }

    public void Set(bool isWinner) {
        if(PhotonNetwork.isMasterClient) {
            int id = -1;
            if (RoundManager.Instance.currentRound == RoundManager.Instance.RoundNum) {
                id = zizouVideoList.Count - 1;
            } else {
                id = Random.Range(0, zizouVideoList.Count - 1);
            }
            photonView.RPC(nameof(SetZizouMovieId), PhotonTargets.All, id);
        }

        if (viewManager == null) {
            viewManager = GameObject.Find("ViewManager").GetComponent<ViewManager>();
        }
        hasWin = isWinner;

        //TODO:Unitask timing
        toRuleSelectButton.SetActive(false);
        zizouVideoPlayer.Play();
    }

    //これが無いと動くけどエラーが出る
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            // ここにオブジェクトの状態を送信するコードを書きます
        } else {
            // ここにオブジェクトの状態を受信して更新するコードを書きます
        }
    }

    public void PushToRuleSelect(VideoPlayer vp) {
        Debug.Log("Rule Select vp");
        if (RoundManager.Instance.currentRound != RoundManager.Instance.RoundNum) {
            photonView.RPC(nameof(ToRuleSelect), PhotonTargets.AllBuffered);
        } else {
            photonView.RPC(nameof(ToEndingView), PhotonTargets.AllBuffered);
        }
    }

    [PunRPC]
    public void SetZizouMovieId(int id) {
        zizouVideoPlayer.clip = zizouVideoList[id];
        zizouInfoLabel.text = "Zizou " + id.ToString();
    }

    [PunRPC]
    public void ToRuleSelect() {
        gameObject.SetActive(false);
        viewManager.ruleSelectViewObj.SetActive(true);
        viewManager.ruleSelectView.GetComponent<RuleSelectView>().Set(hasWin).Forget();
    }

    [PunRPC]
    public void ToEndingView() {
        gameObject.SetActive(false);
        viewManager.endingViewObj.SetActive(true);
        viewManager.endingView.GetComponent<EndingView>().Set();
    }
}