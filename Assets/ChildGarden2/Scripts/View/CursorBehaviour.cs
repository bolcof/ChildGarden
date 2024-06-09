using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorBehaviour : MonoBehaviour {
    [SerializeField] private Image finger;
    [SerializeField] private Sprite idle, clicked;
    public bool displayed;
    public bool isRuleSelectView;
    public bool isSelector;

    private void Awake() {
        displayed = true;
        isRuleSelectView = false;
        isSelector = false;
        Cursor.visible = false;
    }

    private void Update() {
        transform.position = Input.mousePosition;

        if (LocalStateManager.Instance.canPutOnbutsu || !displayed) {
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
                if (!isRuleSelectView) {
                    SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_WholeClick);
                } else {
                    if (!isSelector) {
                        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_NotSelectableClick);
                    }
                }
            }
        }
    }
}