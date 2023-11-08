using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfomationText : MonoBehaviour {
    public static InfomationText instance;
    [SerializeField] private TMP_Text label;

    private void Awake() {
        label = GetComponent<TMP_Text>();
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void ChangeText(string tx) {
        label.text = tx;
    }
}