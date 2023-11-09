using UnityEngine;

public class RuleExplainView : MonoBehaviour {
    public GameObject[] explainPages;
    private int currentIndex = 0; // 現在アクティブなオブジェクトのインデックス。
    [SerializeField] private GameObject fripButton;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject readyLabel;

    //TODO cant use Start
    private void Start() {
        SoundManager.Instance.PlayBgm(SoundManager.Instance.BGM_Introduction);

        // スタート時に全てのオブジェクトを非アクティブにします
        for (int i = 0; i < explainPages.Length; i++) {
            explainPages[i].SetActive(false);
        }

        // 最初のオブジェクトをアクティブにします
        if (explainPages.Length > 0) {
            explainPages[0].SetActive(true);
        }
    }

    private void ResetView() {
        fripButton.SetActive(true);
        startButton.SetActive(false);
        readyLabel.SetActive(false);

        explainPages[0].SetActive(true);
        for (int i = 1; i < explainPages.Length; i++) {
            explainPages[i].SetActive(false);
        }
    }

    public void SwitchToNextObject() {
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_RuleTextAppearing);
        explainPages[currentIndex].SetActive(false);

        currentIndex = (currentIndex + 1) % explainPages.Length;

        explainPages[currentIndex].SetActive(true);

        if (currentIndex == 6) {
            fripButton.SetActive(false);
            startButton.SetActive(true);
        }
    }

    public void PushHasRead() {
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_PushGamePlay);
        startButton.SetActive(false);
        readyLabel.SetActive(true);
        RuleExplainManager.Instance.PushHasRead();
    }
}