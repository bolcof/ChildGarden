using UnityEngine;
using Photon;
using System.Collections.Generic;

public class FloorCollision : Photon.PunBehaviour
{
    private int collisionCount = 0; // 衝突回数をカウント
    public BoxCollider2D areaCollider; // 監視エリアのコライダー

    private bool sharedFlag = false; // 部屋全体で共有されるBool値

    private HashSet<GameObject> processedObjects = new HashSet<GameObject>(); // 既に処理されたオブジェクトのリスト

    private bool winPlayer = false; // 勝利プレイヤーフラグ（ローカル変数）
    private bool losePlayer = false; // 敗北プレイヤーフラグ（ローカル変数）

    public GameObject winPrefab; // 勝利時に表示するプレハブ
    private bool hasLogged = false; // ログが出力されたかどうかのフラグ

    private int winningPlayerID = -1; // 勝利プレイヤーのPlayerIDを保存する変数

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Onbutu"))
        {
            if (processedObjects.Contains(collider.gameObject))
            {
                return;
            }

            processedObjects.Add(collider.gameObject);

            PhotonView pv = collider.gameObject.GetComponent<PhotonView>();
            if (pv != null && pv.isMine)
            {
                collisionCount++;
                Debug.Log("床の上に落とした個数: " + collisionCount);

           if (collisionCount >= 10)
        {
            Debug.Log("01目標達成: " + collisionCount);
            photonView.RPC("SetSharedFlagTrue", PhotonTargets.AllBuffered);
            photonView.RPC("SetWinningPlayerID", PhotonTargets.AllBuffered, PhotonNetwork.player.ID); // PlayerIDを送信
            Instantiate(winPrefab, new Vector3(0, 0, -0.1f), Quaternion.identity);

        }
            }
        }
    }
 [PunRPC]
    void SetWinningPlayerID(int playerID)
    {
        winningPlayerID = playerID;
    }

    public int GetWinningPlayerID()
    {
        return winningPlayerID;
    }

    [PunRPC]
    void SetSharedFlagTrue()
    {
        sharedFlag = true;
    }

    public bool GetSharedFlag()
    {
        return sharedFlag;
    }
}
