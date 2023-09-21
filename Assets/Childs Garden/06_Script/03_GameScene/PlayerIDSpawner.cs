using UnityEngine;
using Photon;
using UnityEngine.SceneManagement;

public class PlayerIDSpawner : Photon.PunBehaviour
{
    public GameObject winPrefabScene1;          // シーン1での勝利プレイヤー用のプレハブ
    public GameObject otherPlayerPrefabScene1;  // シーン1でのその他のプレイヤー用のプレハブ

    public GameObject winPrefabScene2;    // シーン2での勝利プレイヤー用のプレハブ
    public GameObject otherPrefabScene2;  // シーン2でのその他のプレイヤー用のプレハブ

    public GameObject winPrefabScene3;    // シーン3での勝利プレイヤー用のプレハブ
    public GameObject otherPrefabScene3;  // シーン3でのその他のプレイヤー用のプレハブ

    public GameObject winPrefabScene4;    // シーン4での勝利プレイヤー用のプレハブ
    public GameObject otherPrefabScene4;  // シーン4でのその他のプレイヤー用のプレハブ

    public GameObject winPrefabScene5;    // シーン5での勝利プレイヤー用のプレハブ
    public GameObject otherPrefabScene5;  // シーン5でのその他のプレイヤー用のプレハブ

    private bool hasSpawned = false;

    private void Update()
    {
        //if (hasSpawned || FindObjectOfType<FloorCollision>().GetWinningPlayerID() == -1) return;

        //int winningPlayerID = FindObjectOfType<FloorCollision>().GetWinningPlayerID();
        string currentScene = SceneManager.GetActiveScene().name;

        GameObject winPrefab = null;
        GameObject otherPrefab = null;

        switch (currentScene)
        {
            case "testA_0822":
                winPrefab = winPrefabScene1;
                otherPrefab = otherPlayerPrefabScene1;
                break;
            case "testB_0822":
                winPrefab = winPrefabScene2;
                otherPrefab = otherPrefabScene2;
                break;
            case "testC_0822":
                winPrefab = winPrefabScene3;
                otherPrefab = otherPrefabScene3;
                break;
            case "testD_0822":
                winPrefab = winPrefabScene4;
                otherPrefab = otherPrefabScene4;
                break;
            case "testE_0822":
                winPrefab = winPrefabScene5;
                otherPrefab = otherPrefabScene5;
                break;
        }

        /*if (PhotonNetwork.player.ID == winningPlayerID)
        {
            Instantiate(winPrefab, new Vector3(0, 0, -0.1f), Quaternion.identity);
        }
        else
        {
            Instantiate(otherPrefab, new Vector3(0, 0, -0.1f), Quaternion.identity);
        }*/

        hasSpawned = true;
    }
}
