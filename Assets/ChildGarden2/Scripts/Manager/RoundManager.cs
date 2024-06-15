using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : NetworkBehaviour {

    public static RoundManager Instance;

    //static
    [Networked] public int RoundNum { get; set; }
    [Networked] public int currentRound { get; set; }
    public List<int> isWin; /* 0:lose 1:win 2:draw */

    public override void Spawned() {
        for (int i = 0; i < RoundNum; i++) {
            isWin.Add(-1);
        }
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        Debug.Log("MyDebug RoundManager Spawned");
    }

    public void FinishRound(int _isWin) {
        isWin[currentRound - 1] = _isWin;
    }
}
