using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : Photon.PunBehaviour
{
    public GameObject playerPrefab;
    public GameObject otherPrefab; // 新しいプレハブ
    public GameObject otherPrefab2;
    public Vector3[] spawnPoints;
    public PhotonView photonView;

    // すでに使用された座標のインデックスを記録するHashSet
    private HashSet<int> usedSpawnIndexes = new HashSet<int>();
    

    void Start()
    {
        // if (!PhotonNetwork.connected)
        // {
        //     SceneManager.LoadScene("Launcher");
        //     return;
        // }

        if (PhotonNetwork.isMasterClient)
        {
            // マスタークライアントは、新しいプレイヤーに使用済みのスポーンポイントのインデックスを送信します。
            //photonView.RPC("InitializeUsedSpawnIndexes", PhotonTargets.Others, new List<int>(usedSpawnIndexes).ToArray());
            Debug.Log("aaaa isMasterClient");
        }
        else
        {
            // マスタークライアント以外のプレイヤーは、スポーンポイントの初期化情報を待つ
            StartCoroutine(WaitForSpawnPointsInitialization());
            Debug.Log("aaaa isNotMasterClient");
            return;
        }

        SpawnPlayer();
    }

    IEnumerator WaitForSpawnPointsInitialization()
    {
        yield return new WaitUntil(() => usedSpawnIndexes.Count > 0);
        SpawnPlayer();
    }

    void SpawnPlayer() {
        Debug.Log("aaaa SpawnPlayer");
        // ランダムな座標を選択
        Vector3 randomSpawnPoint = GetRandomSpawnPoint();
        GameObject Player = PhotonNetwork.Instantiate(this.playerPrefab.name, randomSpawnPoint, Quaternion.identity, 0);
        // 同じ座標のY軸+2に別のプレハブを生成
        PhotonNetwork.Instantiate(this.otherPrefab.name, new Vector3(randomSpawnPoint.x, randomSpawnPoint.y + 2, randomSpawnPoint.z), Quaternion.identity, 0);
        // 同じ座標に別のプレハブを生成
        PhotonNetwork.Instantiate(this.otherPrefab2.name, new Vector3(randomSpawnPoint.x, randomSpawnPoint.y, randomSpawnPoint.z), Quaternion.identity, 0);


        // 使用した座標のインデックスを記録
        int usedSpawnIndex = System.Array.IndexOf(spawnPoints, randomSpawnPoint);
        photonView.RPC("UpdateUsedSpawnIndexes", PhotonTargets.AllBuffered, usedSpawnIndex);
    }

    // まだ使用されていない座標のインデックスを取得するメソッド
    List<int> GetAvailableSpawnIndexes()
    {
        List<int> availableSpawnIndexes = new List<int>();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (!usedSpawnIndexes.Contains(i))
            {
                availableSpawnIndexes.Add(i);
            }
        }

        return availableSpawnIndexes;
    }

    // ランダムな座標を取得するメソッド
    Vector3 GetRandomSpawnPoint()
    {
        List<int> availableSpawnIndexes = GetAvailableSpawnIndexes();

        if (availableSpawnIndexes.Count == 0)
        {
            Debug.LogError("No available spawn points!");
            return Vector3.zero;
        }

        int randomIndex = UnityEngine.Random.Range(0, availableSpawnIndexes.Count);
        int spawnIndex = availableSpawnIndexes[randomIndex];

        return spawnPoints[spawnIndex];
    }

    [PunRPC]
    void UpdateUsedSpawnIndexes(int usedIndex) {
        Debug.Log("aaaa UpdateUsed");
        usedSpawnIndexes.Add(usedIndex);
    }
}
