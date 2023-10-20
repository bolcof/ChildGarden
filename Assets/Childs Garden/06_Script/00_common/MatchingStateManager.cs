﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchingStateManager : MonoBehaviour {
    public static MatchingStateManager instance;

    //対戦人数
    public int PlayerNum = 2;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Update() {
        //InfomationText.instance.ChangeText("MyPlayerId : " + MyPlayerId().ToString());
    }

    //PlayerのRoom内ID
    public int MyPlayerId() {
        if (PhotonNetwork.connected && PhotonNetwork.inRoom) {
            return PhotonNetwork.player.ID;
        } else {
            Debug.LogWarning("Out of Connect or Room!");
            return -1;
        }
    }
}