using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyOnSpecificScenes : MonoBehaviour
{
    // 指定のシーンの名前をこの配列に格納します。
    public string[] scenesToDestroyOn;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (string sceneName in scenesToDestroyOn)
        {
            if (scene.name == sceneName)
            {
                Destroy(this.gameObject);
                return;
            }
        }
    }

    private void OnDestroy()
    {
        // 必要に応じて、イベントの購読を解除します。
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
