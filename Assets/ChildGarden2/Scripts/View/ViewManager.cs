using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour {
    public static ViewManager Instance;

    public GameObject LauncheViewObj;
    public LauncherView launcherView;

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

        playingViewObj.SetActive(true);
        ruleSelectViewObj.SetActive(false);
        endingViewObj.SetActive(false);
    }
}