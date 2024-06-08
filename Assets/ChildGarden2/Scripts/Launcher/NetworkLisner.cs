using Cysharp.Threading.Tasks;
using ExitGames.Demos.DemoAnimator;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkLisner : SimulationBehaviour, IPlayerJoined {
    [SerializeField] private NetworkRunner networkRunner;
    public void PlayerJoined(PlayerRef player) {
        Debug.Log($"MyDebug Fusion Player Joined: {player}");
        Debug.Log("MyDebug Player Count " + networkRunner.SessionInfo.PlayerCount.ToString());
        if (networkRunner.IsSharedModeMasterClient) {
            Debug.Log("MyDebug Fusion Master" + networkRunner.SessionInfo.PlayerCount.ToString());
            if (networkRunner.SessionInfo.PlayerCount == RoomConector.Instance.PlayerNum) {
                Debug.Log("MyDebug go rule");
                RoomConector.Instance.GoRuleDelayed(2000).Forget();
            }
        }
    }
}