using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class RuleExplainManager : Photon.PunBehaviour {
    public static RuleExplainManager Instance;
    [SerializeField] private int finishRuleReadCount;
    private bool completed;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        completed = false;
    }

    private void Update() {
        if (PhotonNetwork.isMasterClient) {
            if (finishRuleReadCount == RoomConector.Instance.PlayerNum && !completed) {
                Debug.Log("go game");
                completed = true;
                GoGameDelayed(900).Forget();
                photonView.RPC(nameof(WhiteOut), PhotonTargets.AllBuffered);
            }
        }
    }

    public void PushHasRead() {
        photonView.RPC(nameof(IncreaseCount), PhotonTargets.AllBuffered);
    }

    private async UniTask GoGameDelayed(int delay) {
        await UniTask.Delay(delay);
        photonView.RPC(nameof(GameViewAppear), PhotonTargets.AllBuffered);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        //これが無いと動くけどエラーが出る
        if (stream.isWriting) {
            // ここにオブジェクトの状態を送信するコードを書きます
        } else {
            // ここにオブジェクトの状態を受信して更新するコードを書きます
        }
    }

    [PunRPC]
    public void WhiteOut() {
        Debug.Log("white");
        GameObject.Find("FaderCanvas").GetComponent<Fader>().Transit(2.7f);
    }

    [PunRPC]
    public void GameViewAppear() {
        ViewManager.Instance.playingViewObj.SetActive(true);
        GameManager.Instance.GameStart();
        ViewManager.Instance.ruleExplainViewObj.SetActive(false);
    }

    [PunRPC]
    public void IncreaseCount() {
        Debug.Log("Increase");
        finishRuleReadCount++;
    }
}