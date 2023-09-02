using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public GameObject initialCameraPrefab;
    public GameObject alternateCameraPrefab;
    public GameObject WinPrefab;
    private GameObject currentCamera;

    void Start()
    {
        currentCamera = Instantiate(initialCameraPrefab, new Vector3(0, 0, -10), Quaternion.identity);
    }

    void Update()
    {
        // WinPrefabのクローンがヒエラルキーに存在するか確認
        GameObject winClone = GameObject.Find(WinPrefab.name + "(Clone)");
        if (winClone != null)
        {
            // クローンが存在すれば、現在のカメラを削除
            Destroy(currentCamera);
            currentCamera = Instantiate(alternateCameraPrefab, new Vector3(0, 0, -10), Quaternion.identity);
        }
    }
}
