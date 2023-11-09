using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class RoomConector : Photon.PunBehaviour {
    public static RoomConector Instance;

    [SerializeField] private string _gameVersion;
    [SerializeField] private string roomId;
    public string computerName;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        Connect();
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
    }

    public void Connect() {
        if (!PhotonNetwork.connected) {
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
            Debug.Log("Photonに接続しました。");
            SoundManager.Instance.PlayBgm(SoundManager.Instance.BGM_Title);
        }
    }

    public void PushJoin() {
        Debug.Log("try to join random room");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)MatchingStateManager.instance.PlayerNum;
        roomOptions.PublishUserId = true;
        PhotonNetwork.JoinOrCreateRoom(roomId, roomOptions, TypedLobby.Default);
    }

    public void AppearRule() {
        //change to view manager task

    }

    public override void OnJoinedLobby() {
        Debug.Log("ロビーに入りました。");
        GameObject.Find("Launcher_Canvas").GetComponent<LauncherView>().ActivateStartButton();
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg) {
        Debug.Log("ルームの入室に失敗しました。");
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer) {
        base.OnPhotonPlayerConnected(newPlayer);
        Debug.Log("player coming" + PhotonNetwork.playerList.Count().ToString());
        if (PhotonNetwork.isMasterClient) {
            if (PhotonNetwork.playerList.Count() == MatchingStateManager.instance.PlayerNum) {
                Debug.Log("go rule");
                GoRuleDelayed(2000).Forget();
            }
        }
    }

    private async UniTask GoRuleDelayed(int delay) {
        await UniTask.Delay(delay);
        photonView.RPC(nameof(RuleViewAppear), PhotonTargets.AllBuffered);
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
        //TODO:change to View
        PhotonNetwork.LoadLevel("RuleScene");
    }

    /*[PunRPC]
    void IncreaseMemberCount() {
        count++;
        Debug.Log("Ready is called. Count is now: " + count);
        // カウントが1のときだけカウントダウンを開始
        if (count == 1) {
            StartCoroutine(StartCountdown());
        }
    }*/
}