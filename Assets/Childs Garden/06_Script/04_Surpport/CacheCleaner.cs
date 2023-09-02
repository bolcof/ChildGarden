using UnityEngine;
using System.Collections;
using Photon;

public class CacheCleaner : PunBehaviour
{
    void OnLevelWasLoaded(int level)
    {
        // 未使用のアセットをアンロード
        Resources.UnloadUnusedAssets();
        // ガベージコレクションを強制的に実行
        System.GC.Collect();
    }
}