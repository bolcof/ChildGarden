using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class LauncherView : MonoBehaviour {
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject readyLabel;
    [SerializeField] private GameObject buttonShadow;

    public void ActivateStartButton() {
        playButton.SetActive(true);
        buttonShadow.SetActive(true);
        readyLabel.SetActive(false);
        GameObject.Find("ForceRestarter").GetComponent<ForceRestarter>().ableForceRestart = true;
    }

    public void ResetView() {
        playButton.SetActive(false);
        buttonShadow.SetActive(false);
        readyLabel.SetActive(false);
    }

    public void PushStart() {
        _ = RoomConector.Instance.PushJoinAsync();

        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_PushGamePlay);
        playButton.SetActive(false);
        buttonShadow.SetActive(false);
        readyLabel.SetActive(true);
    }
}