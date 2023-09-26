using UnityEngine;
using Photon;
using System.Collections.Generic;

public class StageCollision : Photon.PunBehaviour {
    public bool isMine;
    public int holderId;

    public int sumOnbutsuCount = 0;
    public int othersOnbutsuCount = 0;
    public int myOnbutsuCount = 0;

    public void ResetCount() {
        sumOnbutsuCount = 0;
        othersOnbutsuCount = 0;
        myOnbutsuCount = 0;
    }

    private void Awake() {
        PhotonView photonView = GetComponent<PhotonView>();
        holderId = photonView.owner.ID;
        if (holderId == MatchingStateManager.instance.MyPlayerId()) {
            isMine = true;
            RuleManager.instance.myStage = this;
        } else {
            isMine = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.CompareTag("Onbutu") && isMine) {
            var currentOnbutsu = collider.gameObject.GetComponent<Onbutsu>();
            if (!currentOnbutsu.hasLand_Stage) {

                currentOnbutsu.hasLand_Stage = true;
                currentOnbutsu.Landing_Stage = true;

                if (currentOnbutsu.holderId == holderId) {
                    Debug.Log("Land Stage My Onbutsu");
                    myOnbutsuCount++;
                    sumOnbutsuCount++;
                } else {
                    Debug.Log("Land Stage Others Onbutsu");
                    othersOnbutsuCount++;
                    sumOnbutsuCount++;
                }
                Debug.Log(
                    "器の自分のおんぶつ:" + myOnbutsuCount.ToString()
                    + "器の相手のおんぶつ:" + othersOnbutsuCount.ToString()
                    + "合計:" + sumOnbutsuCount.ToString());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider) {

        if (collider.gameObject.CompareTag("Onbutu")) {
            var currentOnbutsu = collider.gameObject.GetComponent<Onbutsu>();

            currentOnbutsu.Landing_Stage = false;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        //これが無いと動くけどエラーが出る
        if (stream.isWriting) {
            // ここにオブジェクトの状態を送信するコードを書きます
        } else {
            // ここにオブジェクトの状態を受信して更新するコードを書きます
        }
    }
}