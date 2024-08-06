using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkLisner : SimulationBehaviour, IPlayerJoined, INetworkRunnerCallbacks {
    [SerializeField] private NetworkRunner networkRunner;
    public void PlayerJoined(PlayerRef player) {
        Debug.Log("MyDebug Fusion Player Joined to " + networkRunner.SessionInfo.Name + ": " + player.ToString());
        Debug.Log("MyDebug " + networkRunner.SessionInfo.Name + " Player Count " + networkRunner.SessionInfo.PlayerCount.ToString());
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

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {

    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {

    }

    public void OnInput(NetworkRunner runner, NetworkInput input) {

    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {

    }

    public void OnConnectedToServer(NetworkRunner runner) {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) {

    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) {

    }

    public void OnSceneLoadDone(NetworkRunner runner) {

    }

    public void OnSceneLoadStart(NetworkRunner runner) {

    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
        if (networkRunner.SessionInfo.IsValid && networkRunner.SessionInfo.Name != "Lobby") {
            Debug.Log("MyDebug player left");
            ForceRestarter.instance.ForceSuperRestart();
        }
    }
}