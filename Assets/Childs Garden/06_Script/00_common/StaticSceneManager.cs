using UnityEngine;
using Photon;
using UnityEngine.SceneManagement;

public class StaticSceneManager : Photon.MonoBehaviour
{
    private int highestIntValuePlayerID = -1;

    public string[] randomScenesForOtherPlayers;  // その他のプレイヤーのためのランダムシーンのリスト

    void Start()
    {
        PhotonNetwork.automaticallySyncScene = true;
    }

    public void ChangeToNextScene()
    {
    string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    string targetScene = GetNextSceneBasedOnCurrent(currentScene);  // ここでtargetSceneを定義

    if (PhotonNetwork.isMasterClient && !string.IsNullOrEmpty(targetScene))
    {
        PhotonNetwork.LoadLevel(targetScene);
    }

    }

    [PunRPC]
    private void RequestSceneChange()
    {
        ChangeToNextScene();
    }

    private string GetNextSceneBasedOnCurrent(string currentScene)
    {
        switch (currentScene)
        {
            case "testA_0822":
                return "testB_0822";
            case "testB_0822":
                return "testC_0822";
            default:
                Debug.LogError("Unexpected scene: " + currentScene);
                return "";
        }
    }
    
//TODO ------------------------エンディングのシーン分岐--------------------------------
  
}
