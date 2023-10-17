using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomConector : Photon.PunBehaviour
{
    #region Private変数
    string _gameVersion = "test"; 
    #endregion

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Launcher")  // シーン名を確認して、目的のシーンがロードされたときのみ処理を開始します。
        {
            Connect();
        }
    }

    #region Public Methods
    public void Connect()
    {
        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
            Debug.Log("Photonに接続しました。");
            SoundManager.Instance.PlayBgm(SoundManager.Instance.BGM_TitleAndRule);
        }
    }
    #endregion

    #region Photonコールバック

    public override void OnJoinedLobby() {
        Debug.Log("ロビーに入りました。");
        PhotonNetwork.JoinRoom("KanaiWorking");
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg) {
        Debug.Log("ルームの入室に失敗しました。");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.PublishUserId = true;
        PhotonNetwork.CreateRoom("KanaiWorking", roomOptions, TypedLobby.Default);
    }
    #endregion
}
