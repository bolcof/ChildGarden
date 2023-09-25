using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : Photon.PunBehaviour {
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject myBoxStand;
    [SerializeField] private GameObject myFloorObject;
    [SerializeField] private List<Vector3> spawnPoints;
    [SerializeField] private PhotonView photonView;
    public int mySpawnPositionId = -1;

    //TODO 本当はStartで始めるのもイマイチ。シーンが始まること自体をどこかで一元管理したい
    public void SetStage() {

        Debug.Log("Game Start");

        //TODO ポジションが未設定だったらとしたいけど、本当は状態がそろっているかを確認した方が良い
        if (mySpawnPositionId == -1) {
            if (PhotonNetwork.isMasterClient) {
                Debug.Log("I'm masterClient");
                // DONE ID全部ここから振り分けないと、クライアント側で処理が並行して被るリスクがある
                List<int> positionIdList = RandomizedIdList();
                Debug.Log("positionID list :  " + string.Join(",", positionIdList));
                switch (MatchingStateManager.instance.PlayerNum) {
                    case 2:
                        Debug.Log("2player Play");
                        photonView.RPC(nameof(SetSpawnId_2player), PhotonTargets.All, positionIdList[0], positionIdList[1]);
                        break;
                    default:
                        Debug.LogAssertion("wrong playerNum!");
                        break;
                }
            } else {
                Debug.Log("I'm not masterClient");
                // マスタークライアント以外のプレイヤーは、スポーンポイントの初期化情報を待つ
                StartCoroutine(WaitForSpawnPointsInitialization());
                return;
            }
        }

        SpawnPlayer();
    }

    IEnumerator WaitForSpawnPointsInitialization() {
        yield return new WaitUntil(() => mySpawnPositionId != -1);
        SpawnPlayer();
    }

    void SpawnPlayer() {
        // ランダムな座標を選択
        Vector3 spawnPoint = spawnPoints[mySpawnPositionId];
        GameObject Player = PhotonNetwork.Instantiate("Box/" + this.playerPrefab.name, spawnPoint, Quaternion.identity, 0);
        // 同じ座標のY軸+2にStageを生成
        PhotonNetwork.Instantiate("StageObject/" + this.myBoxStand.name, new Vector3(spawnPoint.x, spawnPoint.y + 2, spawnPoint.z), Quaternion.identity, 0);
    }

    private List<int> RandomizedIdList() {

        if (MatchingStateManager.instance.PlayerNum > spawnPoints.Count) {
            Debug.LogError("playerNum cannot be greater than spawnPositionIds");
            return new List<int>();
        }

        List<int> positionIdList = new List<int>();
        List<int> randomizeList = new List<int>();

        for (int i = 0; i < spawnPoints.Count; i++) {
            positionIdList.Add(i);
        }

        for (int i = 0; i < MatchingStateManager.instance.PlayerNum; i++) {
            int index = Random.Range(0, positionIdList.Count);
            randomizeList.Add(positionIdList[index]);
            positionIdList.RemoveAt(index);
        }

        return randomizeList;
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
    public void SetSpawnId_2player(int player1posId, int player2posId) {
        //PlayerIDは1から始まる
        switch (MatchingStateManager.instance.MyPlayerId()) {
            case 1:
                mySpawnPositionId = player1posId;
                Debug.Log("Set Player" + MatchingStateManager.instance.MyPlayerId().ToString() + " -> PositionID:" + player1posId.ToString());
                break;
            case 2:
                mySpawnPositionId = player2posId;
                Debug.Log("Set Player" + MatchingStateManager.instance.MyPlayerId().ToString() + " -> PositionID:" + player2posId.ToString());
                break;
            default:
                Debug.LogError("wrong Player Num int SetSpawnPlayerId");
                break;
        }
    }
}