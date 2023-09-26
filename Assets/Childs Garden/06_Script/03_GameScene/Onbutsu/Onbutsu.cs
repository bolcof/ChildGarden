using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Onbutsu : MonoBehaviour {
    public int holderId, StagingId;

    public bool hasLand_Stage, hasLand_Floor;
    public bool Landing_Stage, Landing_Floor;

    private void Awake() {
        hasLand_Floor = false;
        hasLand_Stage = false;
        Landing_Floor = false;
        Landing_Stage = false;

        PhotonView photonView = GetComponent<PhotonView>();
        holderId = photonView.owner.ID;
        StagingId = -1;
    }
}
