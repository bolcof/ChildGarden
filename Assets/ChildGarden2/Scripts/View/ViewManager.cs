using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour {
    public static ViewManager Instance;

    public GameObject launcherViewObj;
    public LauncherView launcherView;

    public GameObject matchingViewObj;
    public MatchingView matchingView;

    public GameObject ruleExplainViewObj;
    public RuleExplainView ruleExplainView;

    public GameObject playingViewObj;
    public PlayingView playingView;

    public GameObject ruleSelectViewObj;
    public RuleSelectView ruleSelectView;

    public GameObject endingViewObj;
    public EndingView endingView;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        launcherViewObj.SetActive(true);
        matchingViewObj.SetActive(false);
        ruleExplainViewObj.SetActive(false);
        playingViewObj.SetActive(false);
        ruleSelectViewObj.SetActive(false);
        endingViewObj.SetActive(false);
    }
}