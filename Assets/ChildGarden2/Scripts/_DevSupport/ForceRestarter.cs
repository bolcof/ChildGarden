﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class ForceRestarter : Photon.PunBehaviour {
    [SerializeField] private float inactivityTime;
    [SerializeField] private float timer = 0f;
    [SerializeField] private List<GameObject> MustDestroyObject;

    void Update() {
        if (SceneManager.GetActiveScene().name != "Launcher") {
            if (Input.anyKeyDown || Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2) || Input.mouseScrollDelta.y != 0) {
                timer = 0f;
            } else {
                timer += Time.deltaTime;
                if (timer >= inactivityTime) {
                    Debug.Log(inactivityTime.ToString() + "秒間の無操作を検知しました。");
                    OnInactivityDetected();
                    timer = 0f;
                }
            }

            if (Input.GetKeyDown(KeyCode.R)) {
                Debug.Log("Rでの再起動");
                OnInactivityDetected();
                timer = 0f;
            }
        }

        if (Input.GetKey(KeyCode.Escape)) {

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
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
    private void OnInactivityDetected() {
        Debug.Log("OnInactivityDetected");
        photonView.RPC(nameof(RoomBreakAndRestart), PhotonTargets.AllBuffered);
        //photonView.RPC(nameof(DestroyGameManager), PhotonTargets.All);
        //photonView.RPC(nameof(SendPushingTopButton), PhotonTargets.All);
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer) {
        Debug.Log("OnPhotonPlayerDisconnected");
        base.OnPhotonPlayerDisconnected(otherPlayer);
        photonView.RPC(nameof(RoomBreakAndRestart), PhotonTargets.AllBuffered);
    }

    /*[PunRPC]
    public void DestroyGameManager() {
        if (SceneManager.GetActiveScene().name == "MainGame") {
            Destroy(GameManager.Instance.gameObject);
            Destroy(ViewManager.Instance.gameObject);
        }
    }

    [PunRPC]
    public void SendPushingTopButton() {
        Debug.Log("Send pushing...");
        PhotonNetwork.automaticallySyncScene = true;
        SoundManager.Instance.PlayBgm(SoundManager.Instance.BGM_Title);
        PhotonNetwork.LoadLevel("Launcher");
    }*/

    [PunRPC]
    public void RoomBreakAndRestart() {
        foreach (var obj in MustDestroyObject) {
            Destroy(obj);
        }

        SoundManager.Instance.PlayBgm(SoundManager.Instance.BGM_Title);
        PhotonNetwork.LoadLevel("Restarter");

        if (PhotonNetwork.inRoom) {
            PhotonNetwork.LeaveRoom();
        }
    }
}