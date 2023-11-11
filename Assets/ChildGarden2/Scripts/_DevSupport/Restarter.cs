using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restarter : MonoBehaviour {
    void Start() {
        SceneManager.LoadScene("MainGame");
    }
}