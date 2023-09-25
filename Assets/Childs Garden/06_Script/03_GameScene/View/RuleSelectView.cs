using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class RuleSelectView : MonoBehaviour {

    //static
    [SerializeField] private int selectableRuleNum;

    [SerializeField] private GameObject RuleSubjectRoot;
    [SerializeField] private GameObject RuleSubjectButton;
    [SerializeField] private GameObject DecideButton;

    private List<RuleSubjectButton> buttonsList = new List<RuleSubjectButton>();
    private int currentSelectRuleId;

    public void Set(bool isWinner) {
        for (int i = 0; i < selectableRuleNum; i++) {
            var subject = Instantiate(RuleSubjectButton, RuleSubjectRoot.transform);
            //TODO:randomize
            subject.GetComponent<RuleSubjectButton>().SetInfomation(i, this);
            buttonsList.Add(subject.GetComponent<RuleSubjectButton>());
        }
        DecideButton.SetActive(isWinner);
    }

    public void PushRule(int index) {
        foreach(var rsb in buttonsList) {
            rsb.Highlighted(false);
        }
        buttonsList[index].Highlighted(true);
        currentSelectRuleId = buttonsList[index].thisButtonsRuleId;
    }

    public void RepushRule() {
        foreach (var rsb in buttonsList) {
            rsb.Highlighted(false);
        }
        currentSelectRuleId = -1;
    }

    public void PushDecide() {
        gameObject.SetActive(false);
        GameManager.Instance.NextRoundStart();
    }
}