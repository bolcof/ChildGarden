using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;

public class DontDestroy : Photon.MonoBehaviour
{
    public int sceneChangeCounter = 0;
    public string targetSceneName = "YourSceneName";
    private string lastLoadedSceneName = "";  // 前回ロードしたシーンの名前を保持

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (photonView.isMine)
        {
            photonView.TransferOwnership(PhotonNetwork.player.ID);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (lastLoadedSceneName == scene.name)
        {
            return; // 前回と同じシーンがロードされた場合、処理をスキップ
        }
        lastLoadedSceneName = scene.name;

        // マスタークライアントの場合のみ、シーン変更カウンタを増やす
        if (PhotonNetwork.isMasterClient)
        {
            sceneChangeCounter++;
            Debug.Log("現状のシーンカウンター: " + sceneChangeCounter);

            // シーンが○回変更されたときの処理
            if (sceneChangeCounter == 4)
            {
                RequestMasterToChangeScene();
                
            }
        }
    }
    private void RequestMasterToChangeScene()
    {
        // 自分がマスタークライアントの場合、直接シーンを変更
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.LoadLevel(targetSceneName);
        }
        else
        {
            // マスタークライアントにシーン変更をリクエスト
            photonView.RPC("RequestSceneChangeToMaster", PhotonTargets.MasterClient, targetSceneName);
        }
    }

    [PunRPC]
    private void RequestSceneChangeToMaster(string sceneName)
    {
        // マスタークライアントの場合のみ、シーン変更を実行
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.LoadLevel(sceneName);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
