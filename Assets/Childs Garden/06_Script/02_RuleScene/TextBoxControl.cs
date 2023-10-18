using UnityEngine;

public class TextBoxControl : MonoBehaviour {
    public GameObject[] objects; // インスペクターでオブジェクトの配列を指定します。
    private int currentIndex = 0; // 現在アクティブなオブジェクトのインデックス。
    public GameObject fripObj;
    public GameObject startObj;

    private void Start() {
        SoundManager.Instance.PlayBgm(SoundManager.Instance.BGM_Introduction);

        // スタート時に全てのオブジェクトを非アクティブにします
        for (int i = 0; i < objects.Length; i++) {
            objects[i].SetActive(false);
        }

        // 最初のオブジェクトをアクティブにします
        if (objects.Length > 0) {
            objects[0].SetActive(true);
        }
    }

    // このメソッドを呼ぶと、次のオブジェクトがアクティブになり、現在のオブジェクトは非アクティブになります。
    public void SwitchToNextObject() {
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_RuleTextAppearing);
        // 現在のオブジェクトを非アクティブにします
        objects[currentIndex].SetActive(false);

        // 次のオブジェクトのインデックスを計算します
        currentIndex = (currentIndex + 1) % objects.Length;

        // 次のオブジェクトをアクティブにします
        objects[currentIndex].SetActive(true);

        if (currentIndex == 6) {
            fripObj.SetActive(false);
            startObj.SetActive(true);
        }
    }
}