using Fusion;
using NUnit.Framework.Internal.Execution;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoundManager : NetworkBehaviour {

    public static RoundManager instance;

    //static
    public int RoundNum;
    public int currentRound;
    public List<int> isWin; /* 0:lose 1:win 2:draw */

    public int winCount, loseCount/* count up at draw too */;

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
        winCount = 0;
        loseCount = 0;
    }

    public void FinishRound(int _isWin) {
        isWin[currentRound - 1] = _isWin;

        switch (_isWin) {
            case 0:
                loseCount++;
                break;
            case 1:
                winCount++;
                break;
            case 2:
                loseCount++;
                break;
        }
    }

    public bool WholeWinnerIsMe() {
        if (RoundManager.instance.isWin.Count(r => r == 1) > RoundManager.instance.isWin.Count(r => r == 0)) {
            return true;
        } else {
            return false;
        }
    }
}
