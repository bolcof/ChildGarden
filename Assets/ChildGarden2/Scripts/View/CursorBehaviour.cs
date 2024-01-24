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

    [SerializeField] List<GameObject> SpecificSoundButtons = new List<GameObject>();

    private void Awake() {
        displayed = true;
        isRuleSelectView = false;
        isSelector = false;
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
                if (!isRuleSelectView) {
                    SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_WholeClick);
                } else {
                    if (isSelector) {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit = new RaycastHit();
                        GameObject clickedGameObject;
                        if (Physics.Raycast(ray, out hit)) {
                            clickedGameObject = hit.collider.gameObject;
                            if (!SpecificSoundButtons.Contains(clickedGameObject)) {
                                SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_WholeClick);
                            }
                        }
                    } else {
                        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_NotSelectableClick);
                    }
                }
            }
        }
    }

    public void ClickInRuleView(bool isRuleButton, bool isGoButton) {
        if (isRuleButton) {
        
        }else if (isGoButton) {
            SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_WholeClick);
        }


    }
}