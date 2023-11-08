using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro の namespace をインポート

public class DisplaySceneName : MonoBehaviour
{
    public TMP_Text sceneNameText; // TMP_Text コンポーネントへのリファレンス

    private void Start()
    {
        // 現在のシーンの名前を取得
        string sceneName = SceneManager.GetActiveScene().name;
        
        // TMP_Text にシーンの名前をセット
        sceneNameText.text = sceneName;
    }
}