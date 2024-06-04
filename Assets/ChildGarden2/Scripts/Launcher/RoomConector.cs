using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fusion;
using System.Threading.Tasks;

public class RoomConector : NetworkBehaviour {
    public static RoomConector Instance;

    [SerializeField] private string _gameVersion;
    [SerializeField] private string roomId;
    public string computerName;

    [SerializeField] private NetworkRunner networkRunnerPrefab;
    public NetworkRunner networkRunner;
    private NetworkObject networkObject;

    public int PlayerNum;

    private async void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        Debug.Log("Fusion connect");

        networkRunner = Instantiate(networkRunnerPrefab);

        NetworkEvents networkEvents = networkRunner.GetComponent<NetworkEvents>();
        networkEvents.OnConnectedToServer.AddListener(OnConnectedToServer);

        await JoinEmptyLobby();
    }
    private void OnConnectedToServer(NetworkRunner runner) {
        Debug.Log("Fusion connected");
        ViewManager.Instance.launcherView.ActivateStartButton();
        SoundManager.Instance.PlayBgm(SoundManager.Instance.BGM_Title);
    }

    private async UniTask JoinEmptyLobby() {
        var joinResult = await networkRunner.StartGame(new StartGameArgs {
            GameMode = GameMode.Shared,
            SessionName = "", // セッション名を空にする
            SceneManager = networkRunner.GetComponent<NetworkSceneManagerDefault>(),
            CustomLobbyName = _gameVersion
        });

        if (joinResult.Ok) {
            Debug.Log("Success to connect empty room! Fusion");
        }
    }

    public async Task PushJoinAsync() {
        Debug.Log("try to join random room Fusion");

        // 既存のセッションをシャットダウン
        await networkRunner.Shutdown();
        Destroy(networkRunner);
        networkRunner = Instantiate(networkRunnerPrefab);

        // 新しいセッションを開始して特定のルームに参加
        var startGameArgs = new StartGameArgs {
            GameMode = GameMode.Shared,
            SessionName = roomId, // 特定のルーム名を設定
            CustomLobbyName = _gameVersion,
            SceneManager = networkRunner.GetComponent<NetworkSceneManagerDefault>()
        };

        var result = await networkRunner.StartGame(startGameArgs);

        if (result.Ok) {
            Debug.Log($"ルーム '{roomId}' に参加しました。");
        } else {
            Debug.LogError($"ルーム '{roomId}' への参加に失敗しました: {result.ShutdownReason}");
        }

        ViewManager.Instance.matchingViewObj.SetActive(true);
        ViewManager.Instance.matchingView.Set().Forget();
    }

    public void OnFusionPlayerConnected(NetworkRunner runner, PlayerRef player) {
        Debug.Log($"MyDebug Fusion Player Joined: {player}");
        Debug.Log("MyDebug Player Count " + networkRunner.SessionInfo.PlayerCount.ToString());
        if (networkRunner.IsSharedModeMasterClient) {
            Debug.Log("MyDebug Fusion Master" + networkRunner.SessionInfo.PlayerCount.ToString());
            if (networkRunner.SessionInfo.PlayerCount == PlayerNum) {
                Debug.Log("go rule");
                GoRuleDelayed(2000).Forget();
            }
        }
    }

    private async UniTask GoRuleDelayed(int delay) {
        await UniTask.Delay(delay);
        RuleViewAppear();
    }

    //[Rpc(RpcSources.All, RpcTargets.All)]
    public void RuleViewAppear() {
        Debug.Log("fusion appear");
        ViewManager.Instance.ruleExplainViewObj.SetActive(true);
        ViewManager.Instance.ruleExplainView.ResetView();
        ViewManager.Instance.launcherViewObj.SetActive(false);
        ViewManager.Instance.matchingView.Disappear().Forget();
    }

    //PlayerのRoom内ID PUN
    public int MyPlayerId() {
        if (PhotonNetwork.connected && PhotonNetwork.inRoom) {
            return PhotonNetwork.player.ID;
        } else {
            Debug.LogWarning("Out of Connect or Room! PUN");
            return -1;
        }
    }
}