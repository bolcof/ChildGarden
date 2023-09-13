using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : Photon.PunBehaviour {
    public GameObject playerPrefab;
    public GameObject otherPrefab; // 新しいプレハブ
    public GameObject otherPrefab2;
    public Vector3[] spawnPoints;
    public PhotonView photonView;

    private List<int> spawnPositionIds = new List<int>();

    //TODO 本当はStartで始めるのもイマイチ。シーンが始まること自体をどこかで一元管理したい
    void Start() {
        // if (!PhotonNetwork.connected)
        // {
        //     SceneManager.LoadScene("Launcher");
        //     return;
        // })

        //TODO ポジションが未設定だったらとしたいけど、本当は状態がそろっているかを確認した方が良い
        if (StateManager.instance.mySpawnPositionId == -1) {

            if (PhotonNetwork.isMasterClient) {
                // DONE ID全部ここから振り分けないと、クライアント側で処理が並行して被るリスクがある
                List<int> positionIdList = RandomizedIdList();
                photonView.RPC(nameof(SetSpawnId), PhotonTargets.AllBuffered, positionIdList);
            } else {
                // マスタークライアント以外のプレイヤーは、スポーンポイントの初期化情報を待つ
                StartCoroutine(WaitForSpawnPointsInitialization());
                return;
            }
        }

        SpawnPlayer();
    }

    IEnumerator WaitForSpawnPointsInitialization() {
        yield return new WaitUntil(() => StateManager.instance.mySpawnPositionId != -1);
        SpawnPlayer();
    }

    void SpawnPlayer() {
        // ランダムな座標を選択
        Vector3 spawnPoint = spawnPoints[StateManager.instance.mySpawnPositionId];
        GameObject Player = PhotonNetwork.Instantiate(this.playerPrefab.name, spawnPoint, Quaternion.identity, 0);
        // 同じ座標のY軸+2に別のプレハブを生成
        PhotonNetwork.Instantiate(this.otherPrefab.name, new Vector3(spawnPoint.x, spawnPoint.y + 2, spawnPoint.z), Quaternion.identity, 0);
        // 同じ座標に別のプレハブを生成
        PhotonNetwork.Instantiate(this.otherPrefab2.name, new Vector3(spawnPoint.x, spawnPoint.y, spawnPoint.z), Quaternion.identity, 0);
    }

    private List<int> RandomizedIdList() {

        if (StateManager.instance.PlayerNum > spawnPositionIds.Count) {
            Debug.LogError("playerNum cannot be greater than spawnPositionIds");
            return new List<int>();
        }

        List<int> positionList = new List<int>();
        List<int> randomizeList = new List<int>();

        for (int i = 0; i < spawnPositionIds.Count; i++) {
            positionList.Add(i);
        }

        for (int i = 0; i < StateManager.instance.PlayerNum; i++) {
            int index = Random.Range(0, positionList.Count);
            randomizeList.Add(positionList[index]);
            positionList.RemoveAt(index);
        }

        return randomizeList;
    }

    [PunRPC]
    private void SetSpawnId(List<int> positionIdList) {
        StateManager.instance.mySpawnPositionId = positionIdList[StateManager.instance.MyPlayerId()];
    }
}
