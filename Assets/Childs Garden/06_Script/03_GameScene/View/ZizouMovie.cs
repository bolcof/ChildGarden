using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ZizouMovie : Photon.PunBehaviour {
    private ViewManager viewManager;

    [SerializeField] VideoPlayer myVideoPlayer;
    [SerializeField] List<VideoClip> zizouVideoList = new List<VideoClip>();

    private void Awake() {
        myVideoPlayer.loopPointReached += ZizouMovieEnd;
    }

    public void Set(int isWinner) {
        Debug.Log("ZizowMovie Set");
        if (PhotonNetwork.isMasterClient) {
            Debug.Log("ZizowMovie Master");
            int id = -1;
            if (RoundManager.Instance.currentRound == RoundManager.Instance.RoundNum) {
                id = zizouVideoList.Count - 1;
            } else {
                id = Random.Range(0, zizouVideoList.Count - 1);
            }
            photonView.RPC(nameof(SetZizouMovieId), PhotonTargets.All, id);
        }
    }

    //これが無いと動くけどエラーが出る
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            // ここにオブジェクトの状態を送信するコードを書きます
        } else {
            // ここにオブジェクトの状態を受信して更新するコードを書きます
        }
    }

    public void ZizouMovieEnd(VideoPlayer vp) {
        Debug.Log("ZizowMovie End");
        ViewManager.Instance.playingView.CloseGateAndGoNext().Forget();
    }

    [PunRPC]
    public void SetZizouMovieId(int id) {
        Debug.Log("ZizowMovie SetMovieId");
        myVideoPlayer.clip = zizouVideoList[id];
        myVideoPlayer.Play();
    }
}