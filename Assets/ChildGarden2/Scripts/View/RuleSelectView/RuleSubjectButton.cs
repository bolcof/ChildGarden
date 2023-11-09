using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RuleSubjectButton : MonoBehaviour {
    [SerializeField] TextMeshProUGUI label;
    public int thisButtonsRuleId;
    public bool selected;
    [SerializeField] private Image image;
    [SerializeField] private Sprite disSelectedImage, selcetedImage;
    private RuleSelectView ruleSelectView;
    /*public void SetInfomation(int _index, RuleSelectView _view) {
        label.text = RuleManager.instance.rules.Find(r => r.id == _index).explainText;
        //TODO:randomize
        thisButtonsRuleId = _index;
        appearedId = _index;
        ruleSelectView = _view;
    }*/

    //TODO:for 1007
    public void SetInfomation(RuleSelectView _view) {
        //TODO:randomize
        ruleSelectView = _view;
    }

    public void PushRuleButton() {
        if (GameManager.Instance.canOperateUI) {
            if (!selected) {
                ruleSelectView.PushRule(thisButtonsRuleId);
            } else {
                ruleSelectView.RepushRule();
            }
        }
    }

    public void SetHighlight(bool on) {
        if(on) {
            GetComponent<Image>().color = new Color(0.5f, 1.0f, 0.5f, 1.0f);
            //label.text = RuleManager.instance.rules.Find(r => r.id == thisButtonsRuleId).explainText + "　←";
            image.sprite = selcetedImage;
            selected = true;
        } else {
            GetComponent<Image>().color = Color.white;
            //label.text = RuleManager.instance.rules.Find(r => r.id == thisButtonsRuleId).explainText;
            image.sprite = disSelectedImage;
            selected = false;
        }
    }
}
