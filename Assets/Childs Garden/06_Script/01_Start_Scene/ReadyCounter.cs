using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using Photon;

public class ReadyCounter : PunBehaviour // PunBehaviourを継承
{
    private int count = 0;
    public TextMeshProUGUI countdownText;

    public string nextSceneName = "YourNextSceneName"; 

    public GameObject PlayObj;

    public GameObject ReadyObj;

    
    public void Ready()
    {

        photonView.RPC("IncreaseCount", PhotonTargets.AllBuffered); // PhotonTargets.AllBufferedを使用
        PlayObj.SetActive(false);
        ReadyObj.SetActive(true);
        
    }

    [PunRPC]
    void IncreaseCount()
    {
        count++;
        Debug.Log("Ready is called. Count is now: " + count);

        if (count == 1) // カウントが1のときだけカウントダウンを開始
        {
            StartCoroutine(StartCountdown());
        }
    }

    IEnumerator StartCountdown()
    {
        int countdownTime = 10; 
        while(countdownTime > 0)
        {
            countdownText.text = countdownTime.ToString(); 
            yield return new WaitForSeconds(1);
            countdownTime--;
        }

        countdownText.text = "0";
        Debug.Log("Countdown finished!");

        HandleSceneChange();
    }

    void HandleSceneChange()
{
    string currentScene = SceneManager.GetActiveScene().name;

    if (count >= 2)
    {
        if (currentScene == "Launcher") // "Launcher"からtest_Ruleへ
        {
            PhotonNetwork.LoadLevel("test_Rule");
        }
        else if (currentScene == "test_Rule") // "test_Rule"からゲームシーンへ
        {
            PhotonNetwork.LoadLevel("testA_0822");
        }
        // 必要に応じて他のシーンの条件も追加してください
    }
    else
    {
        if (currentScene == "Launcher") // "Launcher"時に一人しか押していない
        {
            SceneManager.LoadScene(currentScene); // 現在のシーンをリロード
        }
        else if (currentScene == "test_Rule") //"test_Rule"時にカウントダウンをリセット
        {
            PlayObj.SetActive(true);
            ReadyObj.SetActive(false);

            // カウントダウンの値とテキストをリセット
            countdownText.text = "10"; // 初期値に戻す
            count = 0; // カウント値をリセット
            StopAllCoroutines(); // 既に実行中のカウントダウンを停止
        }
    }
}
}
