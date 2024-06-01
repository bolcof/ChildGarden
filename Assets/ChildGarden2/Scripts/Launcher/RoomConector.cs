using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fusion;
using System.Threading.Tasks;

public class RoomConector : Photon.PunBehaviour {
    public static RoomConector Instance;

    [SerializeField] private string _gameVersion;
    [SerializeField] private string roomId;
    public string computerName;

    [SerializeField] private NetworkRunner networkRunnerPrefab;
    private NetworkRunner networkRunner;

    public int PlayerNum;

    private async void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        /*if (!PhotonNetwork.connected) {
            Connect();
        }*/
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

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg) {
        Debug.Log("ルームのrandom入室に失敗しました。 PUN");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)PlayerNum;
        PhotonNetwork.CreateRoom("", roomOptions, TypedLobby.Default);
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer) {
        base.OnPhotonPlayerConnected(newPlayer);
        Debug.Log("player coming" + PhotonNetwork.playerList.Count().ToString());
        if (PhotonNetwork.isMasterClient) {
            if (PhotonNetwork.playerList.Count() == PlayerNum) {
                Debug.Log("go rule");
                GoRuleDelayed(2000).Forget();
            }
        }
    }

    private async UniTask GoRuleDelayed(int delay) {
        await UniTask.Delay(delay);
        photonView.RPC(nameof(RuleViewAppear), PhotonTargets.AllBuffered);
    }

    //PlayerのRoom内ID
    public int MyPlayerId() {
        if (PhotonNetwork.connected && PhotonNetwork.inRoom) {
            return PhotonNetwork.player.ID;
        } else {
            Debug.LogWarning("Out of Connect or Room! PUN");
            return -1;
        }
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
    public void RuleViewAppear() {
        ViewManager.Instance.ruleExplainViewObj.SetActive(true);
        ViewManager.Instance.ruleExplainView.ResetView();
        ViewManager.Instance.launcherViewObj.SetActive(false);
        ViewManager.Instance.matchingView.Disappear().Forget();
    }
}