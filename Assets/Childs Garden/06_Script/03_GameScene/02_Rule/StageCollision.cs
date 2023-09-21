using UnityEngine;
using Photon;
using System.Collections.Generic;

public class StageCollision : Photon.PunBehaviour {
    public bool isMine;
    public int holderId;

    private int collisionCount = 0; // 衝突回数をカウント
    public BoxCollider2D areaCollider; // 監視エリアのコライダー

    private bool sharedFlag = false; // 部屋全体で共有されるBool値

    private HashSet<GameObject> processedObjects = new HashSet<GameObject>(); // 既に処理されたオブジェクトのリスト

    private bool winPlayer = false; // 勝利プレイヤーフラグ（ローカル変数）
    private bool losePlayer = false; // 敗北プレイヤーフラグ（ローカル変数）

    public GameObject winPrefab; // 勝利時に表示するプレハブ

    private bool hasLogged = false; // ログが出力されたかどうかのフラグ

    private int winningPlayerID = -1; // 勝利プレイヤーのPlayerIDを保存する変数

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.CompareTag("Onbutu")) {
            if (processedObjects.Contains(collider.gameObject)) {
                return;
            }

            processedObjects.Add(collider.gameObject);

            PhotonView pv = collider.gameObject.GetComponent<PhotonView>();
            if (pv != null && pv.isMine) {
                collisionCount++;
                Debug.Log("台の上に落とした個数: " + collisionCount);

                if (collisionCount >= 7) {
                    Debug.Log("02目標達成: " + collisionCount);
                    photonView.RPC(nameof(SetSharedFlagTrue), PhotonTargets.AllBuffered);
                    photonView.RPC(nameof(SetWinningPlayerID), PhotonTargets.AllBuffered, PhotonNetwork.player.ID); // PlayerIDを送信
                    Instantiate(winPrefab, new Vector3(0, 0, -0.1f), Quaternion.identity);
                }
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

    [PunRPC]
    void SetWinningPlayerID(int playerID) {
        winningPlayerID = playerID;
    }

    public int GetWinningPlayerID() {
        return winningPlayerID;
    }

    [PunRPC]
    void SetSharedFlagTrue() {
        sharedFlag = true;
    }

    public bool GetSharedFlag() {
        return sharedFlag;
    }
}
