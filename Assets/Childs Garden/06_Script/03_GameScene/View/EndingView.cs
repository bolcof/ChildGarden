using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class EndingView : Photon.PunBehaviour {
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private List<VideoClip> endingVideos = new List<VideoClip>();

    private ViewManager viewManager;

    public void Set(int id) {
        videoPlayer.clip = endingVideos[id];
        videoPlayer.Play();

        if (viewManager == null) {
            viewManager = GameObject.Find("ViewManager").GetComponent<ViewManager>();
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

    [PunRPC]
    public void DestroyGameManager() {
        Destroy(GameManager.Instance.gameObject);
    }

    [PunRPC]
    public void SendPushingTopButton() {
        Debug.Log("aaaa send pushing...");
        PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.LoadLevel("Launcher");
    }

    public void PushTopButton() {
        Debug.Log("aaaa push button...");
        photonView.RPC(nameof(DestroyGameManager), PhotonTargets.All);
        photonView.RPC(nameof(SendPushingTopButton), PhotonTargets.MasterClient);
    }
}