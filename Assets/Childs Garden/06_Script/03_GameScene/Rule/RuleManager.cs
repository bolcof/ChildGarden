using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleManager : Photon.PunBehaviour {
    public static RuleManager instance;

    [System.Serializable]
    public struct Rule {
        public int id;
        public string checkMethodName;
        public string explainText;
    }

    [SerializeField] public List<Rule> rules;

    //TODO 複数になるかも
    public Rule currentRule;
    public bool isWinnerDecided;

    [SerializeField] private FloorCollision myFloor;
    public StageCollision myStage;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void SetFirstRound() {
        currentRule = rules.Find(r => r.id == 1);
    }

    public void SetRule(int _id) {
        currentRule = rules.Find(r => r.id == _id);
    }

    private void Update() {
        switch (currentRule.id) {
            case -1:
                Debug.LogError("Rule is not set yet!");
                break;
            case 1:
                CheckRule1();
                break;
            default:
                Debug.LogError("RuleID is out of range!");
                break;
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
    public bool CheckRule1() {
        if (myFloor.myOnbutsuCount >= 2) {
            GameManager.Instance.MyPlayerWin();
            return true;
        } else {
            return false;
        }
    }
}
