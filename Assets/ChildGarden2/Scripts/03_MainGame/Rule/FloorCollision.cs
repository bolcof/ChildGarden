using UnityEngine;
using Photon;
using System.Collections.Generic;

public class FloorCollision : UnityEngine.MonoBehaviour {
    /*
    public int sumOnbutsuCount = 0;
    public int othersOnbutsuCount = 0;
    public int myOnbutsuCount = 0;

    public void ResetCount() {
        sumOnbutsuCount = 0;
        othersOnbutsuCount = 0;
        myOnbutsuCount = 0;
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.CompareTag("Onbutu")) {
            var currentOnbutsu = collider.gameObject.GetComponent<Onbutsu>();
            if (!currentOnbutsu.hasLand_Floor) {

                currentOnbutsu.hasLand_Floor = true;
                currentOnbutsu.Landing_Floor = true;

                if (currentOnbutsu.holderId == -1) {
                    Debug.LogError("Onbutsu ID is Wrong!");
                } else if (currentOnbutsu.holderId == MatchingStateManager.instance.MyPlayerId()) {
                    Debug.Log("Land Floor My Onbutsu");
                    myOnbutsuCount++;
                    sumOnbutsuCount++;
                } else {
                    Debug.Log("Land Floor Others Onbutsu");
                    othersOnbutsuCount++;
                    sumOnbutsuCount++;
                }
                Debug.Log(
                    "床の自分のおんぶつ:" + myOnbutsuCount.ToString()
                    + "床の相手のおんぶつ:" + othersOnbutsuCount.ToString()
                    + "合計:" + sumOnbutsuCount.ToString());
            }
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
    }*/
}