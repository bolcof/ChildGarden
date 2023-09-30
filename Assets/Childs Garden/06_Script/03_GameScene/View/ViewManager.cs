using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour {
    public static ViewManager Instance;

    public GameObject playingViewObj;
    public PlayingView playingView;

    public GameObject zizouViewObj;
    public ZizouView zizouView;

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
    }
}