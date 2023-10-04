using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;

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
    [SerializeField] private float levelUpTime;

    [SerializeField] private Slider chargeSlider;
    [SerializeField] private GameObject sizeSignKnob;
    [SerializeField] private List<Color> sliderColors = new List<Color>();

    private void Start() {
        camera = GetComponent<Camera>();
        chargingTime = 0.0f;
        sizeSignKnob.SetActive(false);
        chargeSlider.gameObject.SetActive(false);
    }

    void Update() {
        if (GameManager.Instance.canPutOnbutsu) {
            int level = FloatDivide(chargingTime, levelUpTime);

            if (Input.GetMouseButton(0)) {
                chargingTime += Time.deltaTime;
                chargeSlider.gameObject.SetActive(true);
                sizeSignKnob.SetActive(true);

                chargeSlider.fillRect.GetComponent<Image>().color = sliderColors[level];
                sizeSignKnob.GetComponent<Image>().color = sliderColors[level];
                if (level <= 2) {
                    chargeSlider.value = FloatDivideRemain(chargingTime, levelUpTime) / levelUpTime * 1000;
                } else {
                    chargeSlider.value = 1000f;
                }
            }

            if (Input.GetMouseButtonUp(0)) {
                chargingTime = 0.0f;

                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, rayDistance)) {
                    Vector3 spawnPosition = hit.point + Vector3.up * 0.5f;
                    switch (level) {
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
                    sizeSignKnob.SetActive(false);
                    chargeSlider.gameObject.SetActive(false);
                }
            }
        }
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