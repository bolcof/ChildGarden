using Cysharp.Threading.Tasks;
using ExitGames.Demos.DemoAnimator;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkLisner : SimulationBehaviour, IPlayerJoined {
    [SerializeField] private NetworkRunner networkRunner;
    public void PlayerJoined(PlayerRef player) {
        Debug.Log("MyDebug Fusion Player Joined to " + networkRunner.SessionInfo.Name + ": "+ player.ToString());
        Debug.Log("MyDebug "+ networkRunner.SessionInfo.Name + " Player Count " + networkRunner.SessionInfo.PlayerCount.ToString());
        if (networkRunner.IsSharedModeMasterClient) {
            Debug.Log("MyDebug Fusion Master");
            if (networkRunner.SessionInfo.IsValid && networkRunner.SessionInfo.Name != "Lobby") {
                if (networkRunner.SessionInfo.PlayerCount == RoomConector.Instance.PlayerNum) {
                    Debug.Log("MyDebug master make go rule");
                    RoomConector.Instance.GoRuleDelayed(2000).Forget();
                }
            }
        }
    }
}