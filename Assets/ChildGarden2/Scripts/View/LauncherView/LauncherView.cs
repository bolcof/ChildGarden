using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class LauncherView : Photon.PunBehaviour {
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject readyLabel;
    [SerializeField] private GameObject buttonShadow;
    private int count = 0;

    private void Awake() {

    }

    public void PushStart() {
        string currentScene = SceneManager.GetActiveScene().name;
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_PushGamePlay);
        photonView.RPC(nameof(IncreaseCount), PhotonTargets.AllBuffered); // PhotonTargets.AllBufferedを使用
        playButton.SetActive(false);
        if (buttonShadow != null) {
            buttonShadow.SetActive(false);
        }
        readyLabel.SetActive(true);
    }

    [PunRPC]
    void IncreaseCount() {
        count++;
        Debug.Log("Ready is called. Count is now: " + count);
    }

    void HandleSceneChange() {
        string currentScene = SceneManager.GetActiveScene().name;

        if (count >= MatchingStateManager.instance.PlayerNum) {
            if (currentScene == "Launcher") // "Launcher"からRuleSceneへ
            {
                PhotonNetwork.LoadLevel("RuleScene");
            } else if (currentScene == "RuleScene") // "RuleScene"からゲームシーンへ
              {
                PhotonNetwork.LoadLevel("MainGame");
            }
            // 必要に応じて他のシーンの条件も追加してください
        } else {
            if (currentScene == "Launcher") // "Launcher"時に一人しか押していない
            {
                SceneManager.LoadScene(currentScene); // 現在のシーンをリロード
            } else if (currentScene == "RuleScene") //"RuleScene"時にカウントダウンをリセット
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
}