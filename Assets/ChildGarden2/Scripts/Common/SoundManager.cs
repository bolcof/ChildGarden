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
    [SerializeField] public AudioClip SE_Progress_minus;
    [SerializeField] public AudioClip SE_PutOnbutsu;
    [SerializeField] public List<AudioClip> SE_OnbutsuCharging = new List<AudioClip>();
    [SerializeField] public AudioClip SE_ChargeLevelUp;
    [SerializeField] public AudioClip SE_RoundFinish;
    [SerializeField] public AudioClip SE_SpawnZizou;
    [SerializeField] public AudioClip SE_CloseDoor;
    [SerializeField] public AudioClip SE_OpenDoor;
    [SerializeField] public AudioClip SE_CloseNewDoor;
    [SerializeField] public AudioClip SE_OpenNewDoor;
    [SerializeField] public AudioClip SE_RuleSelectViewOpening;
    [SerializeField] public AudioClip SE_RuleSelectViewClosing;
    [SerializeField] public AudioClip SE_WholeClick;
    [SerializeField] public AudioClip SE_SelectRule;
    [SerializeField] public AudioClip SE_ClickGo;
    [SerializeField] public AudioClip SE_NotSelectableClick;

    [SerializeField] public AudioClip BGM_Title;
    [SerializeField] public AudioClip BGM_Introduction;
    [SerializeField] public List<AudioClip> BGM_GameScene = new List<AudioClip>();
    [SerializeField] public AudioClip BGM_RuleSelect;
    [SerializeField] public AudioClip BGM_Ending;

    [SerializeField] public AudioSource BgmSource, SeSource, ChargeSource;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void PlaySoundEffect(AudioClip sound) {
        SeSource.PlayOneShot(sound);
    }

    public void PlayBgm(AudioClip bgm) {
        BgmSource.clip = bgm;
        BgmSource.Play();
    }

    public void ChangeChargeEffect(int level) {
        ChargeSource.Stop();
        ChargeSource.clip = SE_OnbutsuCharging[level];
        ChargeSource.Play();
    }

    public void StopSoundEffect() {
        SeSource.loop = false;
        SeSource.Stop();
    }

    public void AllSoundStop() {
        ChargeSource.Stop();
        SeSource.Stop();
        BgmSource.Stop();
    }
}