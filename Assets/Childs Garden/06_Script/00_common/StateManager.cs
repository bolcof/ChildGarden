﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StateManager : MonoBehaviour
{
    public static StateManager instance;

    //使用しているSpawn PositionのID
    public int mySpawnPositionId;

    //対戦人数
    public int PlayerNum = 2;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    mySpawnPositionId = -1;
    }

    private void Update() {
        InfomationText.instance.ChangeText("MyPlayerId : " + MyPlayerId().ToString() + "\n" +
                              "mySpawnPositionId : " + mySpawnPositionId.ToString());
    }

    //PlayerのRoom内ID
    public int MyPlayerId() {
        if (PhotonNetwork.connected && PhotonNetwork.inRoom) {
            return PhotonNetwork.player.ID;
        } else {
            Debug.LogError("Out of Connect or Room!");
            return -1;
        }
    }
}