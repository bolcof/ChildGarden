using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restarter : MonoBehaviour {
    void Start() {
        // 未使用のアセットをアンロード
        Resources.UnloadUnusedAssets();
        // ガベージコレクションを強制的に実行
        System.GC.Collect();
        if (PhotonNetwork.inRoom) {
            PhotonNetwork.LeaveRoom();
        }
        SceneManager.LoadScene("MainGame");
    }
}