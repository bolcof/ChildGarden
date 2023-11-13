using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Utsuwa : Photon.PunBehaviour {
    public GameObject myPlayerSign;
    public bool isMine;
    public int holderId;
    public int CpuId;
    public float holdersProgress;

    private void Awake() {
        PhotonView photonView = GetComponent<PhotonView>();
        holderId = photonView.owner.ID;
        if (holderId == RoomConector.Instance.MyPlayerId()) {
            isMine = true;
            RuleManager.instance.myUtsuwa = this;
        } else {
            isMine = false;
            RuleManager.instance.otherUtsuwaList.Add(this);
            CpuId = RuleManager.instance.otherUtsuwaList.Count;
        }

        if (!photonView.isMine) {
            myPlayerSign.SetActive(false);
        }

        holdersProgress = 0.0f;
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