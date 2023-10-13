using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;
using Unity.Burst.CompilerServices;

public class CreateRayPoint : Photon.PunBehaviour {
    [SerializeField]
    private float rayDistance = 20.0f;
    private Camera camera;
    [SerializeField] private RectTransform canvasRect;

    [SerializeField] string onbutsuFolderName;
    public List<GameObject> OnbutsuList_Level1 = new List<GameObject>();
    public List<GameObject> OnbutsuList_Level2 = new List<GameObject>();
    public List<GameObject> OnbutsuList_Level3 = new List<GameObject>();
    public List<GameObject> OnbutsuList_Level4 = new List<GameObject>();

    [SerializeField] List<GameObject> ChargingEffects = new List<GameObject>();

    [SerializeField] private float chargingTime;
    private int currentChargeLevel, pastChargeLevel;
    [SerializeField] private float levelUpTime;

    [SerializeField] private Slider chargeSlider;
    [SerializeField] private GameObject sizeSignKnob;
    //[SerializeField] private List<GameObject> chargeEffectObject;
    [SerializeField] private List<Color> sliderColors = new List<Color>();

    private void Start() {
        camera = GetComponent<Camera>();
        chargingTime = 0.0f;
        sizeSignKnob.SetActive(false);
        chargeSlider.gameObject.SetActive(false);
        currentChargeLevel = -1;
        pastChargeLevel = -1;

        /*foreach (var ef in chargeEffectObject) {
            ef.SetActive(false);
        }*/
    }

    void Update() {
        if (GameManager.Instance.canPutOnbutsu) {

            if (Input.GetMouseButtonDown(0)) {
                pastChargeLevel = 0;
                currentChargeLevel = 0;
                ChargeLevelUp();
            }

            if (Input.GetMouseButton(0)) {
                //TODO:????????
                chargingTime += Time.deltaTime;
                chargeSlider.gameObject.SetActive(true);
                sizeSignKnob.SetActive(true);

                currentChargeLevel = FloatDivide(chargingTime, levelUpTime);
                if (currentChargeLevel >= 3) { currentChargeLevel = 3; }

                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, rayDistance)) {
                    Vector3 effectPosition = hit.point + Vector3.up;
                    //chargeEffectObject[currentChargeLevel].transform.position = effectPosition;
                }

                chargeSlider.fillRect.GetComponent<Image>().color = sliderColors[currentChargeLevel];
                sizeSignKnob.GetComponent<Image>().color = sliderColors[currentChargeLevel];
                if (currentChargeLevel <= 2) {
                    chargeSlider.value = FloatDivideRemain(chargingTime, levelUpTime) / levelUpTime * 1000;
                } else {
                    chargeSlider.value = 1000f;
                }

                if (pastChargeLevel != currentChargeLevel) {
                    ChargeLevelUp();
                }

                pastChargeLevel = currentChargeLevel;
            }

            if (Input.GetMouseButtonUp(0)) {
                SoundManager.Instance.StopSoundEffect();
                SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_PutOnbutsu);
                chargingTime = 0.0f;
                SoundManager.Instance.ChargeSource.Stop();

                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, rayDistance)) {
                    Vector3 spawnPosition = hit.point + Vector3.up * 0.5f;
                    switch (currentChargeLevel) {
                        case 0:
                            PhotonNetwork.Instantiate(onbutsuFolderName + OnbutsuList_Level1[PhotonNetwork.player.ID - 1].name, spawnPosition, Quaternion.identity, 0);
                            break;
                        case 1:
                            PhotonNetwork.Instantiate(onbutsuFolderName + OnbutsuList_Level2[PhotonNetwork.player.ID - 1].name, spawnPosition, Quaternion.identity, 0);
                            break;
                        case 2:
                            PhotonNetwork.Instantiate(onbutsuFolderName + OnbutsuList_Level3[PhotonNetwork.player.ID - 1].name, spawnPosition, Quaternion.identity, 0);
                            break;
                        default:
                            PhotonNetwork.Instantiate(onbutsuFolderName + OnbutsuList_Level4[PhotonNetwork.player.ID - 1].name, spawnPosition, Quaternion.identity, 0);
                            break;
                    }
                    DisappearGauge();
                }
            }
        }
    }

    public void DisappearGauge() {
        sizeSignKnob.SetActive(false);
        chargeSlider.gameObject.SetActive(false);
    }

    private void ChargeLevelUp() {
        if (currentChargeLevel > 3) { currentChargeLevel = 3; }
        /*foreach (var ef in chargeEffectObject) {
            ef.SetActive(false);
        }*/
        //chargeEffectObject[currentChargeLevel].SetActive(true);
        SoundManager.Instance.ChangeChargeEffect(currentChargeLevel);
    }

    private int FloatDivide(float n1, float n2) {
        int count = 0;
        while (n1 >= n2) {
            count++;
            n1 -= n2;
        }
        return count;
    }

    private float FloatDivideRemain(float n1, float n2) {
        while (n1 >= n2) {
            n1 -= n2;
        }
        return n1;
    }
}