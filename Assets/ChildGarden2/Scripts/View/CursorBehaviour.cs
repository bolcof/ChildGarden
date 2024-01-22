using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorBehaviour : MonoBehaviour {
    [SerializeField] private Image finger;
    [SerializeField] private Sprite idle, clicked;
    public bool displayed;
    public bool wholeClickView;

    private void Awake() {
        displayed = true;
        wholeClickView = true;
        Cursor.visible = false;
    }

    private void Update() {
        transform.position = Input.mousePosition;

        if (GameManager.Instance.canPutOnbutsu || !displayed) {
            finger.enabled = false;
        } else {
            finger.enabled = true;
            if (Input.GetMouseButton(0)) {
                finger.sprite = clicked;
            } else {
                finger.sprite = idle;
            }
        }

        if (Input.GetMouseButtonDown(0)) {
            if (finger.enabled) {
                if (wholeClickView) {
                    SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_WholeClick);
                } else {
                    SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_NotSelectableClick);
                }
            }
        }
    }
}