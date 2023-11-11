using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using TMPro;
using System.Linq;

public class EndingView : Photon.PunBehaviour {
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private TextMeshProUGUI testLabel;
    [SerializeField] private List<VideoClip> endingVideos = new List<VideoClip>();
    [SerializeField] private GameObject testTopButton;

    [SerializeField] private GameObject standaloneWhiteBack;

    private ViewManager viewManager;

    private void Awake() {
        videoPlayer.loopPointReached += OnEndVideo;
    }

    public void Set() {
        testTopButton.SetActive(false);

        if (RuleManager.instance.WholeWinnerIsMe()) {
            videoPlayer.clip = endingVideos.Last();
            videoPlayer.Play();
            testLabel.text = "Win End";
        } else {
            int id = Random.Range(0, endingVideos.Count - 1);
            videoPlayer.clip = endingVideos[id];
            videoPlayer.Play();
            testLabel.text = "Lose End " + id.ToString();
        }

        if (viewManager == null) {
            viewManager = GameObject.Find("ViewManager").GetComponent<ViewManager>();
        }

        standaloneWhiteBack.SetActive(true);
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
        Destroy(GameManager.Instance.gameObject);
        Destroy(ViewManager.Instance.gameObject);
        SoundManager.Instance.PlayBgm(SoundManager.Instance.BGM_Title);
        PhotonNetwork.LoadLevel("Restarter");
    }
}