﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RuleManager : Photon.PunBehaviour {
    public static RuleManager instance;

    [System.Serializable]
    public struct Rule {
        public int id;
        public string explainText;
    }

    [SerializeField] public List<Rule> rules;

    //TODO 複数になるかも
    public Rule currentRule;

    public float progressRatio, pastProgressRatio;
    public bool isWinnerDecided;

    [SerializeField] private FloorCollision myFloor;
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

    public void SetRule(int _id) {
        currentRule = rules.Find(r => r.id == _id);
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
        progressRatio = OnbutsuList.FindAll(on => on.dropped).Count / 3.0f;
        if (pastProgressRatio != progressRatio) {
            SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_Progress);
            pastProgressRatio = progressRatio;
            ViewManager.Instance.playingView.ApplyProgressBar(progressRatio);
        }
        if (OnbutsuList.FindAll(on => on.dropped).Count >= 3) {
            GameManager.Instance.MyPlayerWin();
            return true;
        } else {
            return false;
        }
    }

    public bool CheckRule_1() {
        progressRatio = OnbutsuList.FindAll(on => on.landing_Utsuwa && on.StagingId == MatchingStateManager.instance.MyPlayerId()).Count / 5.0f;
        if (pastProgressRatio != progressRatio) {
            SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_Progress);
            pastProgressRatio = progressRatio;
            ViewManager.Instance.playingView.ApplyProgressBar(progressRatio);
        }
        if (OnbutsuList.FindAll(on => on.landing_Utsuwa && on.StagingId == MatchingStateManager.instance.MyPlayerId()).Count >= 5) {
            GameManager.Instance.MyPlayerWin();
            return true;
        } else {
            return false;
        }
    }

    public bool CheckRule_2() {
        progressRatio = OnbutsuList.FindAll(on => on.landing_Utsuwa && on.holderId == MatchingStateManager.instance.MyPlayerId()).Count / 15.0f;
        if (pastProgressRatio != progressRatio) {
            SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SE_Progress);
            pastProgressRatio = progressRatio;
            ViewManager.Instance.playingView.ApplyProgressBar(progressRatio);
        }
        if (OnbutsuList.FindAll(on => on.landing_Utsuwa && on.holderId == MatchingStateManager.instance.MyPlayerId()).Count >= 15) {
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
