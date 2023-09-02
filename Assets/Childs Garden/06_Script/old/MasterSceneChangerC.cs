using UnityEngine;
using Photon;

public class MasterSceneChangerC : Photon.MonoBehaviour
{
    public void ChangeScene(string testB_0822)
    {
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.LoadLevel(testB_0822);
        }
    }

    [PunRPC]
    private void RequestSceneChange(string testB_0822)
    {
        ChangeScene(testB_0822);
    }
}
