using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConnectState : Photon.PunBehaviour {
    [SerializeField] private Image panel;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] bool defaultAppear;

    private void Awake() {
        panel.enabled = defaultAppear;
        label.enabled = defaultAppear;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.D)) {
            panel.enabled = !panel.enabled;
            label.enabled = !label.enabled;
        }

        if (label.enabled) {
            if (PhotonNetwork.connected) {
                if (PhotonNetwork.inRoom) {
                    label.text = "Lobby:" + PhotonNetwork.lobby.Type + " Room:" + PhotonNetwork.room.Name + " PlayerID:" + PhotonNetwork.player.ID;
                } else {
                    label.text = "Lobby:" + PhotonNetwork.lobby.Type + " Room:None";
                }
            } else {
                label.text = "No Connect";
            }
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