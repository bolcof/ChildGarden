using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;

public class CreateRayPoint : Photon.PunBehaviour
{
    [SerializeField]
    private float distance = 20.0f;
    private Camera camera;

    public GameObject[] s1Prefabs;
    private int number;
    private GameObject spawnPosition;

    public GameObject[] s2Prefabs;
    public GameObject[] s3Prefabs;
    private int currentHp = 0;

    public Slider targetSlider;  // 対象となるSlider
    public Slider targetSlider2;  // 対象となるSlider

    public GameObject SmallObj;
    public GameObject MiddleObj;
    public GameObject targetSlider3;

    private void Start()
    {
        camera = GetComponent<Camera>();

        // Sliderの初期値を設定
        UpdateSliderFromInt();
        UpdateSliderFromInt2();
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && currentHp <= 100)
        {
            SmallObj.SetActive (true);
        }
        
        if (Input.GetMouseButton(0) && currentHp > 100 && currentHp < 200)
        {
            MiddleObj.SetActive (true);
            SmallObj.SetActive (false);
        }
        
        if (Input.GetMouseButton(0) && currentHp >= 200)
        {
            targetSlider3.SetActive (true);
            MiddleObj.SetActive (false);
        }
        
        if (Input.GetMouseButton(0))
        {
            ++currentHp;
            currentHp = Mathf.Clamp(currentHp, 0, 200);
            UpdateSliderFromInt();
            UpdateSliderFromInt2();
        }
    
        if(Input.GetMouseButtonUp(0) && currentHp <= 100)
        { Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, distance))
            {
                number = Random.Range(0, s1Prefabs.Length);
                Vector3 spawnPosition = hit.point + Vector3.up * 0.5f;
                PhotonNetwork.Instantiate(s1Prefabs[number].name, spawnPosition, Quaternion.identity, 0);
                currentHp = 0;
                Debug.Log("value : " + currentHp);
                SmallObj.SetActive (false);
            }
        }

        if (Input.GetMouseButtonUp(0) && currentHp > 100 && currentHp < 200)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, distance))
            {
                number = Random.Range(0, s2Prefabs.Length);
                Vector3 spawnPosition = hit.point + Vector3.up * 0.5f;
                PhotonNetwork.Instantiate(s2Prefabs[number].name, spawnPosition, Quaternion.identity, 0);
                currentHp = 0;
                Debug.Log("value : " + currentHp);
                MiddleObj.SetActive (false);
            }

        }

        if (Input.GetMouseButtonUp(0) && currentHp >= 200)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, distance))
            {
                number = Random.Range(0, s3Prefabs.Length);
                Vector3 spawnPosition = hit.point + Vector3.up * 0.5f;
                PhotonNetwork.Instantiate(s3Prefabs[number].name, spawnPosition, Quaternion.identity, 0);
                currentHp = 0;
                Debug.Log("value : " + currentHp);
                targetSlider3.SetActive (false);
                //2秒後にオブジェクトを消す
                //Invoke("Slider3Active", 1);

            }
        }

        if (currentHp == 0)
        {
            ResetIntValue();
        }
    }

    public void SetIntValue(int newValue)
    {
        currentHp = Mathf.Clamp(newValue, 0, 100);
        UpdateSliderFromInt();
    }

    private void UpdateSliderFromInt()
    {
        targetSlider.value = currentHp;
    }

    public void ResetIntValue()
    {
        currentHp = 0;
        UpdateSliderFromInt();
        UpdateSliderFromInt2();
    }

     public void SetIntValue2(int newValue)
    {
        currentHp = Mathf.Clamp(newValue, 100, 200);
        UpdateSliderFromInt2();
    }

    private void UpdateSliderFromInt2()
    {
        targetSlider2.value = currentHp;
    }
    // private void Slider3Active()
    // {
    //     targetSlider3.SetActive (false);
    // }
}