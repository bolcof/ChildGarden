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
        photonView.RPC(nameof(IncreaseCount), PhotonTargets.AllBuffered); // PhotonTargets.AllBuffered���g�p
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
            if (currentScene == "Launcher") // "Launcher"����RuleScene��
            {
                PhotonNetwork.LoadLevel("RuleScene");
            } else if (currentScene == "RuleScene") // "RuleScene"����Q�[���V�[����
              {
                PhotonNetwork.LoadLevel("MainGame");
            }
            // �K�v�ɉ����đ��̃V�[���̏������ǉ����Ă�������
        } else {
            if (currentScene == "Launcher") // "Launcher"���Ɉ�l���������Ă��Ȃ�
            {
                SceneManager.LoadScene(currentScene); // ���݂̃V�[���������[�h
            } else if (currentScene == "RuleScene") //"RuleScene"���ɃJ�E���g�_�E�������Z�b�g
              {
                playButton.SetActive(true);
                if (buttonShadow != null) {
                    buttonShadow.SetActive(true);
                }
                readyText.SetActive(false);

                // �J�E���g�_�E���̒l�ƃe�L�X�g�����Z�b�g
                count = 0; // �J�E���g�l�����Z�b�g
                StopAllCoroutines(); // ���Ɏ��s���̃J�E���g�_�E�����~
            }
        }
    }
}