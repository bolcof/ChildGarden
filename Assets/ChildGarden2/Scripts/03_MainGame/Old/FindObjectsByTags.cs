using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InstantiatePrefabsByTags : MonoBehaviour
{
    public string[] tagsToFind;  // Unityのインスペクタからアサインするタグの配列
    public GameObject[] prefabsToInstantiate;  // Unityのインスペクタからアサインするプレハブの配列
    public RectTransform startPosition;  // プレハブの開始位置
    public float spacing = 100.0f;  // 各プレハブ間の距離 (ピクセル単位)
    public Canvas targetCanvas;  // プレハブを配置するキャンバス

    void Start()
    {
        // タグとプレハブの配列のサイズが一致しているか確認
        if(tagsToFind.Length != prefabsToInstantiate.Length)
        {
            Debug.LogError("タグとプレハブの配列のサイズが一致していません!");
            return;
        }

        Vector2 currentSpawnPosition = startPosition.anchoredPosition;

        for (int i = 0; i < tagsToFind.Length; i++)
        {
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tagsToFind[i]);
            foreach (GameObject obj in objectsWithTag)
            {
                if (obj.activeInHierarchy)
                {
                    GameObject instance = Instantiate(prefabsToInstantiate[i], targetCanvas.transform, false);
                    RectTransform rectTransform = instance.GetComponent<RectTransform>();
                    if (rectTransform == null)
                    {
                        Debug.LogError("The prefab doesn't have a RectTransform component!");
                        continue;
                    }
                    rectTransform.anchoredPosition = currentSpawnPosition;
                    Debug.Log("Created prefab for active object with tag: " + obj.tag);
                    currentSpawnPosition.y -= spacing;  // 等間隔で下に配置
                }
            }
        }
    }
}
