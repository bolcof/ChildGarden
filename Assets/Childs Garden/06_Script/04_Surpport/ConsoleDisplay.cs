using UnityEngine;
using System.Collections.Generic;
using Photon;

public class ConsoleDisplay : Photon.MonoBehaviour {
    // ログの最大数
    public int maxLogs = 100;

    // ログのリスト
    private List<string> logs = new List<string>();

    // スクロールビューの位置
    private Vector2 scrollPosition;

    // フォントサイズ
    public int fontSize = 14;

    void OnEnable() {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable() {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type) {
        // 一定の文字列で始まるログはスキップ
        if (logString.StartsWith("Observed type is not serializable") || logString.StartsWith("Type of observed is unknown when receiving")) {
            return;
        }

        // スタックトレースの最初の行を取得
        string logSource = stackTrace.Split('\n')[0];

        // ログとそのソースを組み合わせて新しいログを作成
        string combinedLog = $"{logString} ({logSource})";

        // ログを追加
        photonView.RPC("AddLog", PhotonTargets.AllBuffered, combinedLog);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        //これが無いと動くけどエラーが出る
        if (stream.isWriting) {
            // ここにオブジェクトの状態を送信するコードを書きます
        } else {
            // ここにオブジェクトの状態を受信して更新するコードを書きます
        }
    }

    [PunRPC]
    void AddLog(string logString) {
        logs.Add(logString);

        // ログの最大数を超えたら古いものから削除
        if (logs.Count > maxLogs) {
            logs.RemoveAt(0);
        }
    }

    void OnGUI() {
        GUI.skin.label.fontSize = fontSize;
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height / 3));

        foreach (string log in logs) {
            GUILayout.Label(log);
        }

        GUILayout.EndScrollView();
    }
}