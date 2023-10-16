using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RuleManager : Photon.PunBehaviour {
    public static RuleManager instance;

    [System.Serializable]
    public struct Rule {
        public int id;
        public float missionNum;
        public string explainText;
    }

    [SerializeField] public List<Rule> rules;

    //TODO 複数になるかも
    public Rule currentRule;

    public float progressRatio, pastProgressRatio;
    public bool isWinnerDecided;

    public Utsuwa myUtsuwa;
    public Utsuwa otherUtsuwa;

    public List<Onbutsu> OnbutsuList = new List<Onbutsu>();

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void SetFirstRound() {
        currentRule = rules.Find(r => r.id == 0);
        isWinnerDecided = false;
    }

    public void SetRule(int _ruleId) {
        currentRule = rules.Find(r => r.id == _ruleId);
    }

    public void ResetCount() {
        OnbutsuList.Clear();
        isWinnerDecided = false;
        progressRatio = pastProgressRatio = 0.0f;
    }

    private void Update() {
        if (GameManager.Instance.canPutOnbutsu) {
            switch (currentRule.id) {
                case -1:
                    Debug.LogError("Rule is not set yet!");
                    break;
                case 0:
                    CheckRule_0();
                    break;
                case 1:
                    CheckRule_1();
                    break;
                case 2:
                    CheckRule_2();
                    break;
                case 3:
                    CheckRule_3();
                    break;
                default:
                    Debug.LogError("RuleID is out of range!");
                    break;
            }
        }
    }

    //これが無いと動くけどエラーが出る
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            // ここにオブジェクトの状態を送信するコードを書きます
        } else {
            // ここにオブジェクトの状態を受信して更新するコードを書きます
        }
    }

    //Ruleごとに作る
    public bool CheckRule_0() {
        float missionNum = rules[0].missionNum;
        progressRatio = OnbutsuList.FindAll(on => on.dropped && on.holderId == MatchingStateManager.instance.MyPlayerId()).Count / missionNum;
        if (pastProgressRatio != progressRatio) {
            SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_Progress);
            pastProgressRatio = progressRatio;
            ViewManager.Instance.playingView.ApplyProgressBar(progressRatio);
        }
        if (OnbutsuList.FindAll(on => on.dropped && on.holderId == MatchingStateManager.instance.MyPlayerId()).Count >= missionNum) {
            GameManager.Instance.MyPlayerWin();
            return true;
        } else {
            return false;
        }
    }

    public bool CheckRule_1() {
        float missionNum = rules[1].missionNum;
        progressRatio = OnbutsuList.FindAll(on => on.landing_Utsuwa && on.StagingId == MatchingStateManager.instance.MyPlayerId()).Count / missionNum;
        if (pastProgressRatio != progressRatio) {
            SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_Progress);
            pastProgressRatio = progressRatio;
            ViewManager.Instance.playingView.ApplyProgressBar(progressRatio);
        }
        if (OnbutsuList.FindAll(on => on.landing_Utsuwa && on.StagingId == MatchingStateManager.instance.MyPlayerId()).Count >= missionNum) {
            GameManager.Instance.MyPlayerWin();
            return true;
        } else {
            return false;
        }
    }

    public bool CheckRule_2() {
        float missionNum = rules[2].missionNum;
        progressRatio = OnbutsuList.FindAll(on => on.landing_Utsuwa && on.holderId == MatchingStateManager.instance.MyPlayerId()).Count / missionNum;
        if (pastProgressRatio != progressRatio) {
            SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_Progress);
            pastProgressRatio = progressRatio;
            ViewManager.Instance.playingView.ApplyProgressBar(progressRatio);
        }
        if (OnbutsuList.FindAll(on => on.landing_Utsuwa && on.holderId == MatchingStateManager.instance.MyPlayerId()).Count >= missionNum) {
            GameManager.Instance.MyPlayerWin();
            return true;
        } else {
            return false;
        }
    }
    public bool CheckRule_3() {
        float missionNum = rules[3].missionNum;
        progressRatio = OnbutsuList.FindAll(on => on.dropped && on.holderId == MatchingStateManager.instance.MyPlayerId() && on.hasLand_Utsuwa).Count / missionNum;
        if (pastProgressRatio != progressRatio) {
            SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_Progress);
            pastProgressRatio = progressRatio;
            ViewManager.Instance.playingView.ApplyProgressBar(progressRatio);
        }
        if (OnbutsuList.FindAll(on => on.dropped && on.holderId == MatchingStateManager.instance.MyPlayerId() && on.hasLand_Utsuwa).Count >= missionNum) {
            GameManager.Instance.MyPlayerWin();
            return true;
        } else {
            return false;
        }
    }

    public bool WholeWinnerIsMe() {
        if (RoundManager.Instance.isWin.Count(r => r == 1) >= 3) {
            return true;
        } else {
            return false;
        }
    }
}
