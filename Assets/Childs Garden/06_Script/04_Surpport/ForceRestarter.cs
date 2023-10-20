using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class ForceRestarter : Photon.PunBehaviour {
    [SerializeField] private float inactivityTime = 60f;
    private float timer = 0f;

    void Update() {
        if (Input.anyKeyDown || Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2) || Input.mouseScrollDelta.y != 0) {
            timer = 0f;
        } else {
            timer += Time.deltaTime;
            if (timer >= inactivityTime) {
                Debug.Log("1分間の無操作を検知しました。");
                OnInactivityDetected();
                timer = 0f;
            }
        }

        if(Input.GetKeyDown(KeyCode.R)) {
            Debug.Log("Rでの再起動");
            OnInactivityDetected();
            timer = 0f;
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
        photonView.RPC(nameof(DestroyGameManager), PhotonTargets.All);
        photonView.RPC(nameof(SendPushingTopButton), PhotonTargets.All);
    }

    [PunRPC]
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
        PhotonNetwork.LoadLevel("Launcher");
    }
}