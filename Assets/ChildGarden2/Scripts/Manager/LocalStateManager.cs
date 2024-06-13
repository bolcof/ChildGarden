using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalStateManager : MonoBehaviour {
    public static LocalStateManager Instance;
    public bool canPutOnbutsu;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}