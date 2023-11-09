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

    public int PlayerNum = 2;

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
        Debug.Log("try to connect");
        if (!PhotonNetwork.connected) {
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
            Debug.Log("Photonに接続しました。");
            SoundManager.Instance.PlayBgm(SoundManager.Instance.BGM_Title);
        }
    }

    public override void OnJoinedLobby() {
        Debug.Log("ロビーに入りました。");
        ViewManager.Instance.launcherView.ActivateStartButton();
    }

    public void PushJoin() {
        Debug.Log("try to join random room");
        PhotonNetwork.JoinRandomRoom();
        /*RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)PlayerNum;
        roomOptions.PublishUserId = true;
        PhotonNetwork.JoinOrCreateRoom(roomId, roomOptions, TypedLobby.Default);*/
    }

    /*public override void OnPhotonJoinRoomFailed(object[] codeAndMsg) {
        Debug.Log("ルームの入室に失敗しました。");
    }*/

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg) {
        Debug.Log("ルームのrandom入室に失敗しました。");
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
            Debug.LogWarning("Out of Connect or Room!");
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
    }
}