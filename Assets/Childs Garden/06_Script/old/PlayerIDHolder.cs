using UnityEngine;
using System.Collections.Generic;
using Photon;

public class PlayerIDHolder : Photon.PunBehaviour
{
    public GameObject prefabToInstantiate;  // インスタンス化するプレハブ

    private List<int> winningPlayerIDs = new List<int>();

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);  // このオブジェクトを破壊しないように設定
    }

    private void Start()
    {
        if (PhotonNetwork.connected)
        {
            FetchWinningPlayerIDs();
        }
    }

    // `WinLose`タグを持つプレイヤーのPhoton IDを取得する
    private void FetchWinningPlayerIDs()
    {
        GameObject[] winningPlayers = GameObject.FindGameObjectsWithTag("WinLose");
        foreach (var player in winningPlayers)
        {
            PhotonView pView = player.GetComponent<PhotonView>();
            if (pView != null)
            {
                winningPlayerIDs.Add(pView.ownerId);
            }
        }

        // 自分のIDが勝利プレイヤーのIDの中にあるかどうかを確認
        if (winningPlayerIDs.Contains(PhotonNetwork.player.ID))
        {
            InstantiatePrefab();  // 自分のIDが含まれている場合、プレハブをインスタンス化
        }
    }

    // 指定のプレハブをインスタンス化するメソッド
    private void InstantiatePrefab()
    {
        if (prefabToInstantiate != null)
        {
            Instantiate(prefabToInstantiate, transform.position, Quaternion.identity);
        }
    }
}
