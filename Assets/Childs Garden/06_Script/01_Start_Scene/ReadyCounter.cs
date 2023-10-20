using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using Photon;
using System.Linq;

public class ReadyCounter : PunBehaviour // PunBehaviourを継承
{
    private int count = 0;

    public string nextSceneName = "YourNextSceneName";

    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject readyText;
    [SerializeField] private GameObject buttonShadow;

    public void Ready() {
        string currentScene = SceneManager.GetActiveScene().name;
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_PushGamePlay);
        photonView.RPC(nameof(IncreaseCount), PhotonTargets.AllBuffered); // PhotonTargets.AllBufferedを使用
        playButton.SetActive(false);
        if (buttonShadow != null) {
            buttonShadow.SetActive(false);
        }
        readyText.SetActive(true);
    }

    [PunRPC]
    void IncreaseCount() {
        count++;
        Debug.Log("Ready is called. Count is now: " + count);
        // カウントが1のときだけカウントダウンを開始
        if (count == 1) {
            StartCoroutine(StartCountdown());
        }
    }

    IEnumerator StartCountdown() {
        int countdownTime = 6;
        while (countdownTime > 0) {
            yield return new WaitForSeconds(1);
            countdownTime--;
        }
        Debug.Log("Countdown finished!");

        HandleSceneChange();
    }

    void HandleSceneChange() {
        string currentScene = SceneManager.GetActiveScene().name;

        if (count >= 2) {
            if (currentScene == "Launcher") // "Launcher"からtest_Ruleへ
            {
                PhotonNetwork.LoadLevel("test_Rule");
            } else if (currentScene == "test_Rule") // "test_Rule"からゲームシーンへ
              {
                PhotonNetwork.LoadLevel("MainGame");
            }
            // 必要に応じて他のシーンの条件も追加してください
        } else {
            if (currentScene == "Launcher") // "Launcher"時に一人しか押していない
            {
                SceneManager.LoadScene(currentScene); // 現在のシーンをリロード
            } else if (currentScene == "test_Rule") //"test_Rule"時にカウントダウンをリセット
              {
                playButton.SetActive(true);
                if (buttonShadow != null) {
                    buttonShadow.SetActive(true);
                }
                readyText.SetActive(false);

                // カウントダウンの値とテキストをリセット
                count = 0; // カウント値をリセット
                StopAllCoroutines(); // 既に実行中のカウントダウンを停止
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        //これが無いと動くけどエラーが出る
        if (stream.isWriting) {
            // ここにオブジェクトの状態を送信するコードを書きます
        } else {
            // ここにオブジェクトの状態を受信して更新するコードを書きます
        }
    }
}