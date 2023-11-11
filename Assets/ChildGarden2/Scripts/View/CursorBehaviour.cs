using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorBehaviour : MonoBehaviour {
    [SerializeField] private Image pointer;
    [SerializeField] private Sprite idle, clicked;

    private void Awake() {
        Cursor.visible = false;
    }

    private void Update() {
        transform.position = Input.mousePosition;

        if (Input.GetMouseButton(0)) {
            pointer.sprite = clicked;
        } else {
            pointer.sprite = idle;
        }
    }
}