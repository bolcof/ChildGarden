using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ZizouMovie : Photon.PunBehaviour {
    private ViewManager viewManager;

    [SerializeField] VideoPlayer myVideoPlayer;
    [SerializeField] List<VideoClip> zizouVideoList = new List<VideoClip>();
    [SerializeField] private List<int> canUseMovieIds = new List<int>();

    private void Awake() {
        myVideoPlayer.loopPointReached += ZizouMovieEnd;
        canUseMovieIds.Clear();
        for(int i = 0; i < zizouVideoList.Count - 1; i++) {
            canUseMovieIds.Add(i);
        }
    }

    public void Set(int isWinner) {
        Debug.Log("ZizouMovie Set");
        if (PhotonNetwork.isMasterClient) {
            Debug.Log("ZizouMovie Master");
            int id = -1;
            if (RoundManager.Instance.currentRound == RoundManager.Instance.RoundNum) {
                id = zizouVideoList.Count - 1;
            } else {
                id = canUseMovieIds[Random.Range(0, canUseMovieIds.Count)];
                Debug.Log("ZizouMovie ID:" + id.ToString());
                canUseMovieIds.Remove(id);
            }
            photonView.RPC(nameof(SetZizouMovieId), PhotonTargets.All, id);
        }
    }

    //?????????????????????G???[???o??
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            // ???????I?u?W?F?N?g???????????M?????R?[?h??????????
        } else {
            // ???????I?u?W?F?N?g???????????M?????X?V?????R?[?h??????????
        }
    }

    public void ZizouMovieEnd(VideoPlayer vp) {
        Debug.Log("ZizowMovie End");
        ViewManager.Instance.playingView.CloseNewGateAndGoNext().Forget();
    }

    [PunRPC]
    public void SetZizouMovieId(int id) {
        Debug.Log("ZizowMovie SetMovieId");
        myVideoPlayer.clip = zizouVideoList[id];
        myVideoPlayer.Prepare();
        myVideoPlayer.time = 0;
        myVideoPlayer.Play();
    }
}