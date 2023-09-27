using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour {
    [SerializeField] GameObject playingViewObj;
    [SerializeField] PlayingView playingView;

    [SerializeField] GameObject zizouViewObj;
    [SerializeField] ZizouView zizouView;

    [SerializeField] GameObject ruleSelectViewObj;
    [SerializeField] RuleSelectView ruleSelectView;

    [SerializeField] GameObject endingViewObj;
    [SerializeField] EndingView endingView;
}
