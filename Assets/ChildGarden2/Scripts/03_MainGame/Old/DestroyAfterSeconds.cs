using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    private void Start()
    {
        // 4秒後にDestroyObjectメソッドを実行
        Invoke("DestroyObject", 4.0f);
    }

    private void DestroyObject()
    {
        // オブジェクトを破壊
        Destroy(gameObject);
    }
}