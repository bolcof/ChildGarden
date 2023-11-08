using UnityEngine;
using System.Collections;
using Photon;

public class CacheCleaner : PunBehaviour {
    void OnLevelWasLoaded(int level) {
        // 未使用のアセットをアンロード
        Resources.UnloadUnusedAssets();
        // ガベージコレクションを強制的に実行
        System.GC.Collect();
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