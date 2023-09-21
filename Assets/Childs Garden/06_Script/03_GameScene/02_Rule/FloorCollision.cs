using UnityEngine;
using Photon;
using System.Collections.Generic;

public class FloorCollision : UnityEngine.MonoBehaviour {
    public int sumOnbutsuCount = 0;
    public int othersOnbutsuCount = 0;
    public int myOnbutsuCount = 0;

    public void ResetCount() {
        sumOnbutsuCount = 0;
        othersOnbutsuCount = 0;
        myOnbutsuCount= 0;
    }

    private void OnTriggerEnter2D(Collider2D collider) {

        PhotonView photonView = GetComponent<PhotonView>();
        if (collider.gameObject.CompareTag("Onbutu") && photonView.owner.ID == MatchingStateManager.instance.MyPlayerId()) {
            var currentOnbutsu = collider.gameObject.GetComponent<Onbutsu>();

            currentOnbutsu.hasLand_Floor = true;
            currentOnbutsu.Landing_Floor = true;

            if (currentOnbutsu.holderID == -1) {
                Debug.LogError("Onbutsu ID is Wrong!");
            } else if (currentOnbutsu.holderID == MatchingStateManager.instance.MyPlayerId()) {
                Debug.Log("Land Floor My Onbutsu");
                myOnbutsuCount++;
                sumOnbutsuCount++;
            } else {
                Debug.Log("Land Floor Others Onbutsu");
                othersOnbutsuCount++;
                sumOnbutsuCount++;
            }
            Debug.Log(
                photonView.owner.ID.ToString() + " → " 
                +"床の自分のおんぶつ:" + myOnbutsuCount.ToString()
                + "床の相手のおんぶつ:" + othersOnbutsuCount.ToString()
                + "合計:" + sumOnbutsuCount.ToString());
        }
    }

    private void OnTriggerExit2D(Collider2D collider) {

        if (collider.gameObject.CompareTag("Onbutu")) {
            var currentOnbutsu = collider.gameObject.GetComponent<Onbutsu>();

            currentOnbutsu.Landing_Floor = false;
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