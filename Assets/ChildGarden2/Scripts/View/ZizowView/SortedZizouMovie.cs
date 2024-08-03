using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class SortedZizouMovie : MonoBehaviour {
    [SerializeField] VideoPlayer myVideoPlayer;
    [SerializeField] List<VideoClip> zizouVideoList = new List<VideoClip>();

    private void Awake() {
        myVideoPlayer.loopPointReached += ZizouMovieEnd;
    }

    public void Set(int isLastRoundWinner) {
        var id = -1;
        var winCount = RoundManager.instance.winCount;
        var loseCount = RoundManager.instance.loseCount;
        if (winCount + loseCount == 4) {
            if (RoundManager.instance.WholeWinnerIsMe()) {
                id = 6;
            } else {
                id = 7;
            }
        } else {
            if (isLastRoundWinner == 1) {
                id = winCount - 1;
            } else {
                id = 3 + loseCount - 1;
            }
        }
        SetZizouMovieId(id);
    }

    public void ZizouMovieEnd(VideoPlayer vp) {
        ViewManager.Instance.playingView.CloseNewGateAndGoNext().Forget();
    }

    public void SetZizouMovieId(int id) {
        myVideoPlayer.clip = zizouVideoList[id];
        myVideoPlayer.Prepare();
        myVideoPlayer.time = 0;
        myVideoPlayer.Play();
    }
}