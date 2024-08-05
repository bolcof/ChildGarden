using Fusion;
using UnityEngine;
using UnityEngine.Video;

public class RuleExplainView : NetworkBehaviour {
    public GameObject[] explainPages;
    private int currentIndex = 0; // 現在アクティブなオブジェクトのインデックス。
    [SerializeField] private GameObject fripButton;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject readyLabel;
    [SerializeField] private VideoPlayer vp;

    [SerializeField] private RuleExplainManager ruleExplainManager;

    public void ResetView() {
        SoundManager.Instance.PlayBgm(SoundManager.Instance.BGM_Introduction);

        fripButton.SetActive(true);
        startButton.SetActive(false);
        readyLabel.SetActive(false);

        explainPages[0].SetActive(true);
        for (int i = 1; i < explainPages.Length; i++) {
            explainPages[i].SetActive(false);
        }

        if (RoomConector.Instance.networkRunner.IsSharedModeMasterClient) {
            RoomConector.Instance.networkRunner.Spawn(ruleExplainManager);
        }
    }

    public void SwitchToNextObject() {
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_RuleTextAppearing);
        explainPages[currentIndex].SetActive(false);

        currentIndex = (currentIndex + 1) % explainPages.Length;

        explainPages[currentIndex].SetActive(true);

        if (currentIndex == 7) {
            fripButton.SetActive(false);
            startButton.SetActive(true);
        }

        vp.time = 0.0f;
        vp.Play();
    }

    public void PushHasRead() {
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_PushGamePlay);
        startButton.SetActive(false);
        readyLabel.SetActive(true);
        RuleExplainManager.Instance.PushHasRead();
    }
}