using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleSelectView : MonoBehaviour {
    private int round;
    [SerializeField] private GameObject RuleSubjectRoot;
    [SerializeField] private GameObject RuleSubjectButton;

    public void Set(int subjectNum) {
        Instantiate(RuleSubjectButton, RuleSubjectRoot.transform);
    }
}