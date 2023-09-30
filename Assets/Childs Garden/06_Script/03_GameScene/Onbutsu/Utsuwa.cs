using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utsuwa : Photon.PunBehaviour {
    public GameObject myPlayerSign;

    private void Awake() {
        if (!photonView.isMine) {
            myPlayerSign.SetActive(false);
        }
    }

    public void SignEnabled(bool enabled) {
        if (!photonView.isMine) {
            myPlayerSign.SetActive(false);
        } else {
            myPlayerSign.SetActive(enabled);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        //これが無いと動くけどエラーが出る
        if (stream.isWriting) {
            // ここにオブジェクトの状態を送信するコードを書きます
        } else {
            // ここにオブジェクトの状態を受信して更新するコードを書きます
        }
    }
}