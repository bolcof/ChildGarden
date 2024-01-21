using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UtsuwaManager : Photon.PunBehaviour {
    [SerializeField] private List<GameObject> UtsuwaList = new List<GameObject>();
    [SerializeField] private GameObject myBoxCollision;
    [SerializeField] private List<Vector3> spawnPoints;
    [SerializeField] private PhotonView photonView;

    public int mySpawnPositionId = -1;
    public int myUtsuwaId = -1;
    public int myOnbutsuColorId = -1;

    public Utsuwa myUtsuwa;

    public void SetStage() {

        Debug.Log("Game Start");

        //TODO ポジションが未設定だったらとしたいけど、本当は状態がそろっているかを確認した方が良い
        if (mySpawnPositionId == -1) {
            if (PhotonNetwork.isMasterClient) {
                Debug.Log("I'm masterClient");
                // DONE ID全部ここから振り分けないと、クライアント側で処理が並行して被る
                List<int> positionIdList = RandomizedPositionIdList();
                RandomizedColorIds();

                Debug.Log("positionID list :  " + string.Join(",", positionIdList));
                Debug.Log("colorId list : " + string.Join(",", GameObject.Find("MainCamera").GetComponent<CreateRayPoint>().usingOnbutsuColor));

                switch (RoomConector.Instance.PlayerNum) {
                    case 2:
                        Debug.Log("2player Play");
                        photonView.RPC(nameof(SetSpawnId_2player), PhotonTargets.All, positionIdList[0], positionIdList[1]);
                        break;
                    case 3:
                        Debug.Log("3player Play");
                        photonView.RPC(nameof(SetSpawnId_3player), PhotonTargets.All, positionIdList[0], positionIdList[1], positionIdList[2]);
                        break;
                    default:
                        Debug.LogAssertion("wrong playerNum!");
                        break;
                }
            } else {
                Debug.Log("I'm not masterClient");
                // マスタークライアント以外のプレイヤーは、スポーンポイントの初期化情報を待つ
                StartCoroutine(WaitForSpawnPointsInitialization());
                StartCoroutine(WaitForOnbutsuColorInitialization());
                return;
            }
        }

        SpawnPlayer();
    }

    IEnumerator WaitForSpawnPointsInitialization() {
        yield return new WaitUntil(() => mySpawnPositionId != -1);
        SpawnPlayer();
    }
    IEnumerator WaitForOnbutsuColorInitialization() {
        yield return new WaitUntil(() => myOnbutsuColorId != -1);
    }

    void SpawnPlayer() {
        Vector3 spawnPoint = spawnPoints[mySpawnPositionId];
        GameObject Player = PhotonNetwork.Instantiate("Box/" + this.UtsuwaList[RoomConector.Instance.MyPlayerId() - 1].name, new Vector3(spawnPoint.x, spawnPoint.y + 2, spawnPoint.z), Quaternion.identity, 0);
        myUtsuwa = Player.GetComponent<Utsuwa>();
        AppearMyPlayerPin();
    }

    public void AppearMyPlayerPin() {
        myUtsuwa.SignEnabled(true);
    }

    private List<int> RandomizedPositionIdList() {

        if (RoomConector.Instance.PlayerNum > spawnPoints.Count) {
            Debug.LogError("playerNum cannot be greater than spawnPositionIds");
            return new List<int>();
        }

        List<int> positionIdList = new List<int>();
        List<int> randomizeList = new List<int>();

        for (int i = 0; i < spawnPoints.Count; i++) {
            positionIdList.Add(i);
        }

        for (int i = 0; i < RoomConector.Instance.PlayerNum; i++) {
            int index = Random.Range(0, positionIdList.Count);
            randomizeList.Add(positionIdList[index]);
            positionIdList.RemoveAt(index);
        }

        return randomizeList;
    }

    private List<int> RandomizedColorIds() {
        List<int> colorIdList = new List<int>();
        List<int> randomizeList = new List<int>();

        for (int i = 0; i < 4; i++) {
            colorIdList.Add(i);
        }

        for (int i = 0; i < 4; i++) {
            int index = Random.Range(0, colorIdList.Count);
            randomizeList.Add(colorIdList[index]);
            colorIdList.RemoveAt(index);
        }

        photonView.RPC(nameof(SetOnbutsuColor), PhotonTargets.All, randomizeList[0], randomizeList[1], randomizeList[2], randomizeList[3]);
        return randomizeList;
    }

    //TODO:必要だったら使う
    private List<int> RandomizedUtsuwaIdList() {

        if (RoomConector.Instance.PlayerNum > UtsuwaList.Count) {
            Debug.LogError("playerNum cannot be greater than utsuwaList");
            return new List<int>();
        }

        List<int> utsuwaIdList = new List<int>();
        List<int> randomizeList = new List<int>();

        for (int i = 0; i < spawnPoints.Count; i++) {
            utsuwaIdList.Add(i);
        }

        for (int i = 0; i < RoomConector.Instance.PlayerNum; i++) {
            int index = Random.Range(0, utsuwaIdList.Count);
            randomizeList.Add(utsuwaIdList[index]);
            utsuwaIdList.RemoveAt(index);
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
        switch (RoomConector.Instance.MyPlayerId()) {
            case 1:
                mySpawnPositionId = player1posId;
                Debug.Log("Set Player" + RoomConector.Instance.MyPlayerId().ToString() + " -> PositionID:" + player1posId.ToString());
                break;
            case 2:
                mySpawnPositionId = player2posId;
                Debug.Log("Set Player" + RoomConector.Instance.MyPlayerId().ToString() + " -> PositionID:" + player2posId.ToString());
                break;
            default:
                Debug.LogError("wrong Player Num int SetSpawnPlayerId");
                break;
        }
    }

    [PunRPC]
    public void SetSpawnId_3player(int player1posId, int player2posId, int player3posId) {
        //PlayerIDは1から始まる
        switch (RoomConector.Instance.MyPlayerId()) {
            case 1:
                mySpawnPositionId = player1posId;
                Debug.Log("Set Player" + RoomConector.Instance.MyPlayerId().ToString() + " -> PositionID:" + player1posId.ToString());
                break;
            case 2:
                mySpawnPositionId = player2posId;
                Debug.Log("Set Player" + RoomConector.Instance.MyPlayerId().ToString() + " -> PositionID:" + player2posId.ToString());
                break;
            case 3:
                mySpawnPositionId = player3posId;
                Debug.Log("Set Player" + RoomConector.Instance.MyPlayerId().ToString() + " -> PositionID:" + player3posId.ToString());
                break;
            default:
                Debug.LogError("wrong Player Num int SetSpawnPlayerId");
                break;
        }
    }


    [PunRPC]
    public void SetOnbutsuColor(int color1, int color2, int color3, int color4) {
        GameObject.Find("MainCamera").GetComponent<CreateRayPoint>().usingOnbutsuColor.Clear();
        GameObject.Find("MainCamera").GetComponent<CreateRayPoint>().usingOnbutsuColor.Add(color1);
        GameObject.Find("MainCamera").GetComponent<CreateRayPoint>().usingOnbutsuColor.Add(color2);
        GameObject.Find("MainCamera").GetComponent<CreateRayPoint>().usingOnbutsuColor.Add(color3);
        GameObject.Find("MainCamera").GetComponent<CreateRayPoint>().usingOnbutsuColor.Add(color4);
        myOnbutsuColorId = GameObject.Find("MainCamera").GetComponent<CreateRayPoint>().usingOnbutsuColor[PhotonNetwork.player.ID - 1];
    }
}