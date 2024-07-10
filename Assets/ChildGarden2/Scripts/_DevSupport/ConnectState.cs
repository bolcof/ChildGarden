using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class ConnectState : MonoBehaviour {
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
            NetworkRunner networkRunner = RoomConector.Instance.networkRunner;
            if (networkRunner != null && networkRunner.IsRunning) {
                var lobbyType = networkRunner.GameMode.ToString();
                var roomName = networkRunner.SessionInfo.Name;
                var playerId = networkRunner.LocalPlayer.PlayerId;
                if (networkRunner.SessionInfo.IsValid) {
                    label.text = "Lobby:" + lobbyType + " Room:" + roomName + " PlayerID:" + playerId;
                } else {
                    label.text = "Lobby:" + lobbyType + " Room:None";
                }
            } else {
                label.text = "No Connect";
            }
        }
    }
}