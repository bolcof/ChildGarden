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

    private int hasWin;

    private ViewManager viewManager;

    private void Awake() {
        zizouVideoPlayer.loopPointReached += PushToRuleSelect;
    }

    public void Set(int isWinner) {
        if (PhotonNetwork.isMasterClient) {
            int id = -1;
            if (RoundManager.instance.currentRound == RoundManager.instance.RoundNum) {
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
        if (RoundManager.instance.currentRound != RoundManager.instance.RoundNum) {
            photonView.RPC(nameof(ToRuleSelectFromZizowView), PhotonTargets.AllBuffered);
        } else {
            photonView.RPC(nameof(ToEndingView), PhotonTargets.AllBuffered);
        }
    }

    [PunRPC]
    public void SetZizouMovieId(int id) {
        zizouVideoPlayer.clip = zizouVideoList[id];
    }

    [PunRPC]
    public void ToRuleSelectFromZizowView() {
        Debug.Log("To Rule Select from ZizowView");
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