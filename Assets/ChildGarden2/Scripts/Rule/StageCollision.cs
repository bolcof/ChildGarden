using UnityEngine;
using System.Collections.Generic;

public class StageCollision : MonoBehaviour {
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
        if (holderId == RoomConector.Instance.MyPlayerId()) {
            isMine = true;
            RuleManager.instance.myUtsuwa = null; //TODO delete
        } else {
            isMine = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.CompareTag("Onbutu") && isMine) {
            var currentOnbutsu = collider.gameObject.GetComponent<Onbutsu>();
            if (!currentOnbutsu.hasLand_Utsuwa) {

                currentOnbutsu.hasLand_Utsuwa = true;
                currentOnbutsu.landing_Utsuwa = true;

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

            currentOnbutsu.landing_Utsuwa = false;
        }
    }
}