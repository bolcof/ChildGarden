using UnityEngine;
using UnityEngine.UI;
using Photon;
using UnityEngine.SceneManagement;

public class RuleSelectScript : Photon.PunBehaviour // Photon.PunBehaviourを継承
{
    public GameObject prefabToActivate; // アクティブにするプレハブ
    public GameObject prefabPushCounter; // インスタンス化するプレハブ
    public GameObject prefabImage; // インスタンス化するプレハブ
    //public string targetTag = "NoTouch"; // ターゲットとなるオブジェクトのタグ
    private bool canClick = true; // スイッチが一度しか押せないように
void Start()
    {
        // 配列内のすべてのプレハブを非アクティブにするRPCメソッドを呼び出す
        photonView.RPC("DeactivatePrefabsRPC", PhotonTargets.All);
        
    }
    
    public void HideButtonAndShowImage()
    {

    if (canClick == true)
    {   
        // すべてのプレイヤーにRPCメソッドを送信してプレハブをアクティブにする
        photonView.RPC("ActivatePrefab", PhotonTargets.All);
        canClick = false; 
        
    }
    
    }

    [PunRPC] // RPCメソッドの属性を指定
    void ActivatePrefab()
    {
        // 指定したプレハブをアクティブにする
        prefabToActivate.SetActive(true);
        // インスペクタウィンドウでドラッグアンドドロップしたプレハブをインスタンス化
        Instantiate(prefabPushCounter, new Vector3(0, 0, 0), Quaternion.identity);
        Instantiate(prefabImage, new Vector3(0, 0, 0), Quaternion.identity);
    }

    [PunRPC]
    void DeactivatePrefabsRPC()
    {
        prefabToActivate.SetActive(false);
    }
}