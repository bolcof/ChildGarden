using UnityEngine;
using Photon;

public class MasterSceneChangerB : Photon.MonoBehaviour
{
    public void ChangeScene(string testC_0822)
    {
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.LoadLevel(testC_0822);
        }
    }

    [PunRPC]
    private void RequestSceneChange(string testC_0822)
    {
        ChangeScene(testC_0822);
    }
}
