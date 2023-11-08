using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;

public class ReloadSceneOnEsc : Photon.MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // MasterClientがRPCを呼び出してすべてのプレイヤーのシーンを再ロード
            if (PhotonNetwork.isMasterClient)
            {
                photonView.RPC("ReloadScene", PhotonTargets.All);
            }
        }
    }

    [PunRPC]
    void ReloadScene()
    {
        // 現在のシーンを再ロード
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
