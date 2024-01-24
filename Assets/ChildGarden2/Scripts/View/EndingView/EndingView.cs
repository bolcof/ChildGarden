using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using TMPro;
using System.Linq;
using Cysharp.Threading.Tasks;

public class EndingView : Photon.PunBehaviour {
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private TextMeshProUGUI testLabel;
    [SerializeField] private List<VideoClip> endingVideos = new List<VideoClip>();

    private ViewManager viewManager;

    private void Awake() {
        videoPlayer.loopPointReached += OnEndVideo;
    }

    public void Set() {
        GameObject.Find("Cursor").GetComponent<CursorBehaviour>().displayed = false;

        if (RuleManager.instance.WholeWinnerIsMe()) {
            videoPlayer.clip = endingVideos.Last();
        } else {
            int id = Random.Range(0, endingVideos.Count - 1);
            videoPlayer.clip = endingVideos[id];
        }
        videoPlayer.Play();

        if (viewManager == null) {
            viewManager = GameObject.Find("ViewManager").GetComponent<ViewManager>();
        }
        GameObject.Find("ForceRestarter").GetComponent<ForceRestarter>().ableForceRestart = false;
    }

    //これが無いと動くけどエラーが出る
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            // ここにオブジェクトの状態を送信するコードを書きます
        } else {
            // ここにオブジェクトの状態を受信して更新するコードを書きます
        }
    }

    public void OnEndVideo(VideoPlayer vp) {
        Debug.Log("EndVideo...");
        GameObject.Find("FaderCanvas").GetComponent<Fader>().WhiteOut();
        Invoke(nameof(Restart), 2.0f);
    }

    private void Restart() {
        GameObject.Find("ForceRestarter").GetComponent<ForceRestarter>().OnlyRestart();
    }
}