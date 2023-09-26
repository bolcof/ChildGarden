using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZizouView : Photon.PunBehaviour {
    [SerializeField] GameObject oldZizouObject;
    [SerializeField] GameObject winZizouObject, loseZizouObject;
    [SerializeField] GameObject topButton;

    public void Set(bool isWinner) {
        Instantiate(oldZizouObject, new Vector3(0, 0, -0.1f), Quaternion.identity);
        winZizouObject.SetActive(isWinner);
        loseZizouObject.SetActive(!isWinner);
        topButton.SetActive(isWinner);
    }

    //これが無いと動くけどエラーが出る
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            // ここにオブジェクトの状態を送信するコードを書きます
        } else {
            // ここにオブジェクトの状態を受信して更新するコードを書きます
        }
    }

    public void PushTopButton() {
        Debug.Log("aaaa push button...");
        photonView.RPC(nameof(SendPushingTopButton), PhotonTargets.MasterClient);
        Destroy(GameManager.Instance.gameObject);
    }

    [PunRPC]
    public void SendPushingTopButton() {
        Debug.Log("aaaa send pushing...");
        GameManager.Instance.MoveTopScene();
        Debug.Log("aaaa move scene...");
        PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.LoadLevel("Launcher");
    }
}