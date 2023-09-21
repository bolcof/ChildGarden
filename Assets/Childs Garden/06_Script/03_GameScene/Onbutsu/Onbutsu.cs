using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Onbutsu : MonoBehaviour {
    public bool isMine;

    public int SpawnerID, StagingID;

    public bool hasLand_Stage, hasLand_Floor;
    public bool Landing_Stage, Landing_Floor;

    private void Awake() {
        hasLand_Floor = false;
        hasLand_Stage = false;
        Landing_Floor = false;
        Landing_Stage = false;

        SpawnerID = MatchingStateManager.instance.MyPlayerId();
        StagingID = -1;
    }
}
