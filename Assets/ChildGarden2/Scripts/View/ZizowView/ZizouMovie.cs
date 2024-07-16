using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ZizouMovie : MonoBehaviour {
    [SerializeField] VideoPlayer myVideoPlayer;
    [SerializeField] List<VideoClip> zizouVideoList = new List<VideoClip>();
    [SerializeField] private List<int> canUseMovieIds = new List<int>();

    private void Awake() {
        myVideoPlayer.loopPointReached += ZizouMovieEnd;
        canUseMovieIds.Clear();
        for (int i = 0; i < zizouVideoList.Count - 1; i++) {
            canUseMovieIds.Add(i);
        }
    }

    public void Set(int isWinner) {
        int id = -1;
        if (RoomConector.Instance.rpcListner.HasStateAuthority) {
            if (RoundManager.instance.currentRound == RoundManager.instance.RoundNum) {
                id = zizouVideoList.Count - 1; //???I???E???h?p???f??
            } else {
                id = canUseMovieIds[Random.Range(0, canUseMovieIds.Count)];
                Debug.Log("Set ZizouMovie ID:" + id.ToString());
                canUseMovieIds.Remove(id);
            }
            RoomConector.Instance.rpcListner.RPC_ZizouMovie_SetZizouMovieId(id);
        }
    }

    public void ZizouMovieEnd(VideoPlayer vp) {
        Debug.Log("ZizowMovie End");
        ViewManager.Instance.playingView.CloseNewGateAndGoNext().Forget();
    }

    public void SetZizouMovieId(int id) {
        myVideoPlayer.clip = zizouVideoList[id];
        myVideoPlayer.Prepare();
        myVideoPlayer.time = 0;
        myVideoPlayer.Play();
    }
}