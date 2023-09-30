using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RuleSubjectButton : MonoBehaviour {
    [SerializeField] TextMeshProUGUI label;
    public int appearedId, thisButtonsRuleId;
    public bool selected;
    private RuleSelectView ruleSelectView;
    public void SetInfomation(int _index, RuleSelectView _view) {
        label.text = RuleManager.instance.rules.Find(r => r.id == _index).explainText;
        //TODO:randomize
        thisButtonsRuleId = _index;
        appearedId = _index;
        ruleSelectView = _view;
    }
    public void PushRuleButton() {
        if (GameManager.Instance.canOperateUI) {
            if (!selected) {
                ruleSelectView.PushRule(appearedId);
            } else {
                ruleSelectView.RepushRule();
            }
        }
    }

    public void SetHighlight(bool on) {
        if(on) {
            GetComponent<Image>().color = new Color(0.5f, 1.0f, 0.5f, 1.0f);
            label.text = RuleManager.instance.rules.Find(r => r.id == thisButtonsRuleId).explainText + "　←";
            selected = true;
        } else {
            GetComponent<Image>().color = Color.white;
            label.text = RuleManager.instance.rules.Find(r => r.id == thisButtonsRuleId).explainText;
            selected = false;
        }
    }
}
