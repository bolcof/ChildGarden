using UnityEngine;
using Photon;

public class PlayerUI : Photon.MonoBehaviour {
    public GameObject uiImagePrefab; // UIイメージプレハブをアサイン

    void Start() {
        // このスクリプトがアタッチされているオブジェクトが自分のプレイヤーオブジェクトである場合、UIイメージを生成
        if (photonView.isMine) {
            // UIイメージプレハブを指定の位置に生成
            var uiImage = Instantiate(uiImagePrefab, transform.position, Quaternion.identity);

            // UIイメージプレハブをこのゲームオブジェクトの子として設定
            uiImage.transform.SetParent(transform);
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

