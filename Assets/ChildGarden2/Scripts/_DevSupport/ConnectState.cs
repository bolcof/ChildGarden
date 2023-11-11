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
                    label.text = "Lobby:" + PhotonNetwork.lobby.Type + " Room:" + PhotonNetwork.room.Name;
                } else {
                    label.text = "Lobby:" + PhotonNetwork.lobby.Type + " Room:None";
                }
            } else {
                label.text = "No Connect";
            }
        }
    }
}