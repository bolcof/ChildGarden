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

    [SerializeField] private NetworkRunner networkRunnerPrefab;
    public NetworkRunner networkRunner;

    [SerializeField] private RpcListner rpcListnerObject;
    [SerializeField] private GameObject gameManagerObject;
    public RpcListner rpcListner;

    public int PlayerNum;

    private async void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        Debug.Log("MyDebug Fusion connect");

        networkRunner = Instantiate(networkRunnerPrefab);

        NetworkEvents networkEvents = networkRunner.GetComponent<NetworkEvents>();
        networkEvents.OnConnectedToServer.AddListener(OnConnectedToServer);

        await JoinEmptyLobby();
    }

    private void OnConnectedToServer(NetworkRunner runner) {
        Debug.Log("MyDebug Fusion connected");
        ViewManager.Instance.launcherView.ActivateStartButton();
        SoundManager.Instance.PlayBgm(SoundManager.Instance.BGM_Title);
        networkRunner.Spawn(gameManagerObject);
    }

    private async UniTask JoinEmptyLobby() {
        var joinResult = await networkRunner.StartGame(new StartGameArgs {
            GameMode = GameMode.Shared,
            SessionName = "Lobby",
            SceneManager = networkRunner.GetComponent<NetworkSceneManagerDefault>(),
            CustomLobbyName = _gameVersion
        });

        if (joinResult.Ok) {
            Debug.Log("MyDebug Success to connect empty room! Fusion");
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
        
        if (networkRunner.IsSharedModeMasterClient) {
            networkRunner.Spawn(rpcListnerObject);
        }

            ViewManager.Instance.matchingViewObj.SetActive(true);
        ViewManager.Instance.matchingView.Set().Forget();
    }

    public async UniTask GoRuleDelayed(int delay) {
        await UniTask.Delay(delay);
        rpcListner.RPC_RuleViewAppear();
    }

    // PlayerのRoom内ID PUN
    public int MyPlayerId() {
        // NetworkRunnerとセッション情報が有効かどうかを確認
        if (networkRunner != null && networkRunner.SessionInfo.IsValid) {
            // ローカルプレイヤーのIDを返す
            PlayerRef localPlayer = networkRunner.LocalPlayer;
            return localPlayer.PlayerId;
        } else {
            Debug.LogWarning("Disconnected or NotInRoom");
            return -1;
        }
    }
}