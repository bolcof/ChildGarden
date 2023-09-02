using UnityEngine;
using Photon;

public class SenderScript : Photon.PunBehaviour
{
    public GameObject prefabParent;  // The prefab that will have the child object.
    public string childObjectName;  // The name of the child object to watch.
    private int intValue = 0;
    public string targetTag = "NoTouch"; // ターゲットとなるオブジェクトのタグを指定

    private void Start()
    {
        // 送信側のプレハブのViewIDを全てのプレイヤーに送信するRPCメソッドを呼び出す
        photonView.RPC("SendViewID", PhotonTargets.AllBuffered, photonView.viewID);
        
    }

    void Update(){


    if (prefabParent != null)
        {
            Transform childObject = prefabParent.transform.Find(childObjectName);

            if (childObject != null)
            {
                // Check if specified child object is active.
                if (childObject.gameObject.activeInHierarchy)
                {
                    Debug.Log("現状のint値: " + intValue);
                    // 送信側のプレハブのViewIDを全てのプレイヤーに送信するRPCメソッドを呼び出す
                    photonView.RPC("SendViewID", PhotonTargets.AllBuffered, photonView.viewID);
                    Debug.Log("Child object is instantiated and active!");
                    // Do something with the child object.
                    UpdateValue();
                }
            }
            

        }
    }

    public void UpdateValue()
    {
        int newValue = intValue + 1;
        Debug.Log("更新されたint値: " + newValue);
        // int値が更新されたことを全てのプレイヤーに通知するRPCメソッドを呼び出す
        photonView.RPC("UpdateIntValue", PhotonTargets.AllBuffered, intValue);
    }

    public int GetIntValue()
    {
        return intValue;
    }

    [PunRPC]
    public void SendViewID(int viewID)
    {
        // このメソッドは送信されたViewIDを受け取るためのものであるため、ここでは何もしない
    }

    [PunRPC]
    public void UpdateIntValue(int newValue)
    {
        // このメソッドはint値が更新されたことを受け取るためのものであるため、ここでは何もしない
    }
}

