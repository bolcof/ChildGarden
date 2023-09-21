using UnityEngine;
using Photon;
using System.Collections.Generic;

public class FloorCollision : UnityEngine.MonoBehaviour {
    private int collisionCount = 0; // 衝突回数をカウント
    public BoxCollider2D areaCollider; // 監視エリアのコライダー

    public GameObject winPrefab; // 勝利時に表示するプレハブ

    private int winningPlayerID = -1; // 勝利プレイヤーのPlayerIDを保存する変数

    private void Start() {
        
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.CompareTag("Onbutu")) {

            PhotonView pv = collider.gameObject.GetComponent<PhotonView>();
            if (pv != null && pv.isMine) {
                collisionCount++;
                Debug.Log("床の上に落とした個数: " + collisionCount);
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
