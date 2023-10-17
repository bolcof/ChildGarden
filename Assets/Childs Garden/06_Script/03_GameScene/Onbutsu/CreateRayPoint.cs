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

    [SerializeField] string onbutsuFolderName;
    public List<GameObject> OnbutsuList_Level1 = new List<GameObject>();
    public List<GameObject> OnbutsuList_Level2 = new List<GameObject>();
    public List<GameObject> OnbutsuList_Level3 = new List<GameObject>();
    public List<GameObject> OnbutsuList_Level4 = new List<GameObject>();

    [SerializeField] List<GameObject> ChargingEffects = new List<GameObject>();

    [SerializeField] private float chargingTime;
    private int currentChargeLevel, pastChargeLevel;
    [SerializeField] private float levelUpTime;
    [SerializeField] private int chargeLevelMax;

    [SerializeField] private GameObject guageRoot;
    [SerializeField] private Image chargeGuage;
    [SerializeField] private List<Image> objectImages = new List<Image>();

    [SerializeField] private List<Color> sliderColors = new List<Color>();

    private void Start() {
        camera = GetComponent<Camera>();
        chargingTime = 0.0f;
        guageRoot.SetActive(false);
        //chargeSlider.gameObject.SetActive(false);
        chargeGuage.fillAmount = 0.0f;
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
                //chargeSlider.gameObject.SetActive(true);
                guageRoot.SetActive(true);

                guageRoot.transform.position = Input.mousePosition;

                currentChargeLevel = FloatDivide(chargingTime, levelUpTime);
                if (currentChargeLevel >= chargeLevelMax) { currentChargeLevel = chargeLevelMax; }

                chargeGuage.color = sliderColors[currentChargeLevel];
                if (currentChargeLevel == 0) {
                    foreach(var im in objectImages) {
                        im.gameObject.SetActive(false);
                    }
                } else {
                    foreach (var im in objectImages) {
                        im.gameObject.SetActive(false);
                    }
                    objectImages[currentChargeLevel - 1].gameObject.SetActive(true);
                }

                if (currentChargeLevel <= chargeLevelMax - 1) {
                    chargeGuage.fillAmount = FloatDivideRemain(chargingTime, levelUpTime) / levelUpTime;
                } else {
                    chargeGuage.fillAmount = 1.0f;
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
                            break;
                        case 1:
                            PhotonNetwork.Instantiate(onbutsuFolderName + OnbutsuList_Level1[PhotonNetwork.player.ID - 1].name, spawnPosition, Quaternion.identity, 0);
                            break;
                        case 2:
                            PhotonNetwork.Instantiate(onbutsuFolderName + OnbutsuList_Level2[PhotonNetwork.player.ID - 1].name, spawnPosition, Quaternion.identity, 0);
                            break;
                        case 3:
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
        guageRoot.SetActive(false);
        SoundManager.Instance.ChargeSource.Stop();
    }

    private void ChargeLevelUp() {
        if (currentChargeLevel > chargeLevelMax) { currentChargeLevel = chargeLevelMax; }
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