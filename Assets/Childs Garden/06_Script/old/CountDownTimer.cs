using UnityEngine;
using System.Collections;
using Photon;
using TMPro;

public class CountDownTimer : Photon.MonoBehaviour
{
    // Start is called before the first frame update
    
    public TextMeshProUGUI countdownText;
    private float countdownTime = 90.0f;
    void Start()
    {
        StartCoroutine(DelayedStart());
    }

    // Update is called once per frame
    //----------------------カウントダウンタイマー---------------------------------
    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(5.0f);  // 5秒待つ
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        while (countdownTime > 0)
        {
            // 分と秒に変換
            int minutes = Mathf.FloorToInt(countdownTime / 60);
            int seconds = Mathf.FloorToInt(countdownTime % 60);

            countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            yield return new WaitForSeconds(1.0f);

            countdownTime -= 1.0f;
        }

        countdownText.text = "00:00";

       RequestSceneChange();
    }

    //-----------------------Photonを用いたシーン移動------------------------------

 // この関数を呼び出すと、マスタークライアントにシーンの変更をリクエストします。
    public void RequestSceneChange()
    {
        // RPCを使用して、MasterSceneChangerAのRequestSceneChangeを呼び出します。
        photonView.RPC("RequestSceneChangeRPC", PhotonTargets.MasterClient);
    }

    [PunRPC]
    private void RequestSceneChangeRPC()
    {
        // この部分はMasterClientのみ実行します。
        MasterSceneChangerA sceneChanger = FindObjectOfType<MasterSceneChangerA>();
        if (sceneChanger != null)
        {
            sceneChanger.ChangeToNextScene();
        }
    }
}
