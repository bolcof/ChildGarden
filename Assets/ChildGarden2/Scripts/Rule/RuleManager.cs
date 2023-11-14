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
        [TextArea(1, 3)] public string explainText;
    }

    [SerializeField] public List<Rule> rules;

    //TODO 複数になるかも
    public Rule currentRule;
    [SerializeField] private int firstStageRuleId;

    public float progressRatio, pastProgressRatio;
    public bool isWinnerDecided;

    public Utsuwa myUtsuwa;
    public List<Utsuwa> otherUtsuwaList;

    public List<Onbutsu> OnbutsuList = new List<Onbutsu>();

    [SerializeField] private GameObject Rule05_GoalLine, Rule06_BigUtsuwa;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void SetFirstRound() {
        currentRule = rules.Find(r => r.id == firstStageRuleId);
        isWinnerDecided = false;
        Debug.Log("SetFirstRound");
        SetSpecialObject(firstStageRuleId);
    }

    public void SetRule(int _ruleId) {
        currentRule = rules.Find(r => r.id == _ruleId);
        SetSpecialObject(_ruleId);
    }

    private void SetSpecialObject(int id) {
        Debug.Log("SetSpecialObject");
        Rule05_GoalLine.SetActive(false);
        Rule06_BigUtsuwa.SetActive(false);
        myUtsuwa.gameObject.SetActive(true);
        foreach (var u in otherUtsuwaList) {
            u.gameObject.SetActive(true);
        }

        if (id == 5) {
            Rule05_GoalLine.SetActive(true);
        } else if (id == 6) {
            Rule06_BigUtsuwa.SetActive(true);
            myUtsuwa.gameObject.SetActive(false);
            foreach(var u in otherUtsuwaList) {
                u.gameObject.SetActive(false);
            }
        }
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
                case 4:
                    CheckRule_4();
                    break;
                case 5:
                    CheckRule_5();
                    break;
                case 6:
                    CheckRule_6();
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
        int targetCount = OnbutsuList.FindAll(on => on.dropped && on.holderId == RoomConector.Instance.MyPlayerId()).Count;

        ApplyProgressState(targetCount / missionNum);

        if (targetCount >= missionNum) {
            GameManager.Instance.MyPlayerWin();
            return true;
        } else {
            return false;
        }
    }

    public bool CheckRule_1() {
        float missionNum = rules[1].missionNum;
        int targetCount = OnbutsuList.FindAll(on => on.landing_Utsuwa && on.StagingId == RoomConector.Instance.MyPlayerId()).Count;

        ApplyProgressState(targetCount / missionNum);

        if (targetCount >= missionNum) {
            GameManager.Instance.MyPlayerWin();
            return true;
        } else {
            return false;
        }
    }

    public bool CheckRule_2() {
        float missionNum = rules[2].missionNum;
        int targetCount = OnbutsuList.FindAll(on => on.landing_Utsuwa && on.holderId == RoomConector.Instance.MyPlayerId()).Count;

        ApplyProgressState(targetCount / missionNum);

        if (targetCount >= missionNum) {
            GameManager.Instance.MyPlayerWin();
            return true;
        } else {
            return false;
        }
    }
    public bool CheckRule_3() {
        float missionNum = rules[3].missionNum;
        int targetCount = OnbutsuList.FindAll(on => on.dropped && on.holderId == RoomConector.Instance.MyPlayerId() && on.hasLand_Utsuwa).Count;

        ApplyProgressState(targetCount / missionNum);

        if (targetCount >= missionNum) {
            GameManager.Instance.MyPlayerWin();
            return true;
        } else {
            return false;
        }
    }

    public bool CheckRule_4() {
        float missionNum = rules[4].missionNum;
        int targetCount =
            Mathf.Min(OnbutsuList.FindAll(on => on.holderId == RoomConector.Instance.MyPlayerId() && on.landing_Utsuwa && on.onbutsuSize == 1).Count, 1)
            + Mathf.Min(OnbutsuList.FindAll(on => on.holderId == RoomConector.Instance.MyPlayerId() && on.landing_Utsuwa && on.onbutsuSize == 2).Count, 1)
            + Mathf.Min(OnbutsuList.FindAll(on => on.holderId == RoomConector.Instance.MyPlayerId() && on.landing_Utsuwa && on.onbutsuSize == 3).Count, 1)
            + Mathf.Min(OnbutsuList.FindAll(on => on.holderId == RoomConector.Instance.MyPlayerId() && on.landing_Utsuwa && on.onbutsuSize == 4).Count, 1);

        ApplyProgressState(targetCount / missionNum);

        if (targetCount >= missionNum) {
            GameManager.Instance.MyPlayerWin();
            return true;
        } else {
            return false;
        }
    }
    public bool CheckRule_5() {
        float missionNum = Rule05_GoalLine.transform.position.y;

        float highestOnbutsuHeight = myUtsuwa.transform.position.y;
        foreach (var on in OnbutsuList.FindAll(onb => onb.landing_Utsuwa && onb.StagingId == RoomConector.Instance.MyPlayerId() && onb.hasLand_Utsuwa)) {
            if (highestOnbutsuHeight < on.transform.position.y) {
                highestOnbutsuHeight = on.transform.position.y;
            }
        }

        float targetCount = highestOnbutsuHeight;
        ApplyProgressState(Devide5Per((targetCount - myUtsuwa.transform.position.y) / (missionNum - myUtsuwa.transform.position.y)));

        if (targetCount >= missionNum) {
            GameManager.Instance.MyPlayerWin();
            Rule05_GoalLine.SetActive(false);
            return true;
        } else {
            return false;
        }
    }
    public bool CheckRule_6() {
        float missionNum = rules[6].missionNum;
        int targetCount = OnbutsuList.FindAll(on => on.landing_Utsuwa && on.hasLand_Utsuwa).Count;

        ApplyProgressState(targetCount / missionNum);

        if (targetCount >= missionNum) {
            GameManager.Instance.MyPlayerWin();
            return true;
        } else {
            return false;
        }
    }

    private void ApplyProgressState(float currentRatio) {
        progressRatio = currentRatio;
        if (pastProgressRatio != progressRatio) {
            if (pastProgressRatio < progressRatio) {
                SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_Progress);
            } else {
                SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_Progress_minus);
            }
            pastProgressRatio = progressRatio;
            myUtsuwa.holdersProgress = progressRatio;
            ViewManager.Instance.playingView.ApplyProgressBar(progressRatio);
        }
    }

    private float Devide5Per(float ratio) {
        float i = 0;
        while (ratio > 0.05f) {
            ratio -= 0.05f;
            i += 0.05f;
        }
        return i;
    }

    public bool WholeWinnerIsMe() {
        if (RoundManager.Instance.isWin.Count(r => r == 1) > RoundManager.Instance.isWin.Count(r => r == 0)) {
            return true;
        } else {
            return false;
        }
    }
}
