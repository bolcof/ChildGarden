using UnityEngine;

public class DisplayHierarchy : MonoBehaviour
{
    public int fontSize = 12; // フォントサイズを変更するためのパブリック変数
    private Vector2 scrollPosition; // スクロールバーの位置

    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = fontSize;

        int windowWidth = 300; // ウィンドウの幅
        int windowHeight = Screen.height; // ウィンドウの高さ
        int lineHeight = fontSize + 2; // 1行の高さ
        int contentHeight = 0; // 内容の総高さ

        // すべてのルートのGameObjectを取得
        GameObject[] allObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject rootObject in allObjects)
        {
            contentHeight += lineHeight;
            contentHeight += CountChildObjects(rootObject.transform) * lineHeight;
        }

        // スクロールビューを作成
        scrollPosition = GUI.BeginScrollView(
            new Rect(Screen.width - windowWidth, 0, windowWidth, windowHeight),
            scrollPosition,
            new Rect(0, 0, windowWidth - 20, contentHeight)
        );

        int yPos = 0; // 現在のY座標

        // 各ルートのGameObjectを処理
        foreach (GameObject rootObject in allObjects)
        {
            // ルートのGameObjectの名前を表示
            DisplayGameObject(rootObject, 0, ref yPos, lineHeight, style);
        }

        // スクロールビューの終了
        GUI.EndScrollView();
    }

    // GameObjectを表示する関数
    void DisplayGameObject(GameObject go, int indentation, ref int yPos, int lineHeight, GUIStyle style)
    {
        string indentedName = new string(' ', indentation * 4) + go.name;
        if (go.activeInHierarchy)
        {
            style.normal.textColor = Color.green; // アクティブな場合は緑色
        }
        else
        {
            style.normal.textColor = Color.red; // 非アクティブな場合は赤色
        }
        GUI.Label(new Rect(10, yPos, 400, 20), indentedName, style);
        yPos += lineHeight;

        // 子のGameObjectの名前を再帰的に表示
        foreach (Transform child in go.transform)
        {
            DisplayGameObject(child.gameObject, indentation + 1, ref yPos, lineHeight, style);
        }
    }

    // 子のGameObjectの数を再帰的にカウントする関数
    int CountChildObjects(Transform parent)
    {
        int count = parent.childCount;
        foreach (Transform child in parent)
        {
            count += CountChildObjects(child);
        }
        return count;
    }
}
