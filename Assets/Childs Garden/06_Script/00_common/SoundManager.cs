using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager Instance;

    [SerializeField] public AudioClip SE_SceneMove;
    [SerializeField] public AudioClip SE_Talking;
    [SerializeField] public AudioClip SE_PushGamePlay;
    [SerializeField] public AudioClip SE_RuleTextAppearing;
    [SerializeField] public AudioClip SE_CountDown;
    [SerializeField] public AudioClip SE_Progress;
    [SerializeField] public AudioClip SE_PutOnbutsu;
    [SerializeField] public AudioClip SE_OnbutsuCharging;
    [SerializeField] public AudioClip SE_ChargeLevelUp;
    [SerializeField] public AudioClip SE_RoundFinish;
    [SerializeField] public AudioClip SE_SpawnZizou;
    [SerializeField] public AudioClip SE_CloseDoor;
    [SerializeField] public AudioClip SE_OpenDoor;
    [SerializeField] public AudioClip SE_RuleSelectViewOpening; /* to webm */

    [SerializeField] public AudioClip BGM_TitleAndRule;
    [SerializeField] public AudioClip BGM_GameScene;
    [SerializeField] public AudioClip BGM_RuleSelect;
    [SerializeField] public AudioClip BGM_Ending;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}