using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : Photon.PunBehaviour {

    public static RoundManager Instance;

    //static
    public int RoundNum;
    public int currentRound;
    public List<int> isWin; /* 0:lose 1:win 2:draw */

    private void Awake() {
        for (int i = 0; i < RoundNum; i++) {
            isWin.Add(-1);
        }
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void FinishRound(int _isWin) {
        isWin[currentRound - 1] = _isWin;
    }

    //これが無いと動くけどエラーが出る
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            // ここにオブジェクトの状態を送信するコードを書きます
        } else {
            // ここにオブジェクトの状態を受信して更新するコードを書きます
        }
    }
}
