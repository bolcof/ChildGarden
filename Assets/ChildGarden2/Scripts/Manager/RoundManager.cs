using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : NetworkBehaviour {

    public static RoundManager instance;

    //static
    public int RoundNum;
    public int currentRound;
    public List<int> isWin; /* 0:lose 1:win 2:draw */

    public override void Spawned() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        for (int i = 0; i < RoundNum; i++) {
            isWin.Add(-1);
        }

        Debug.Log("MyDebug RoundManager Spawned");
    }

    public void FinishRound(int _isWin) {
        isWin[currentRound - 1] = _isWin;
    }
}
