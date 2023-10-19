using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ZizouMovie : Photon.PunBehaviour {
    private ViewManager viewManager;

    [SerializeField] VideoPlayer myVideoPlayer;
    [SerializeField] List<VideoClip> zizouVideoList = new List<VideoClip>();

    private int hasWin;

    private void Awake() {
        myVideoPlayer.loopPointReached += ZizouMovieEnd;
    }

    public void Set(int isWinner) {
        if (PhotonNetwork.isMasterClient) {
            int id = -1;
            if (RoundManager.Instance.currentRound == RoundManager.Instance.RoundNum) {
                id = zizouVideoList.Count - 1;
            } else {
                id = Random.Range(0, zizouVideoList.Count - 1);
            }
            photonView.RPC(nameof(SetZizouMovieId), PhotonTargets.All, id);
        }
        hasWin = isWinner;
        myVideoPlayer.Play();
    }

    //���ꂪ�����Ɠ������ǃG���[���o��
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            // �����ɃI�u�W�F�N�g�̏�Ԃ𑗐M����R�[�h�������܂�
        } else {
            // �����ɃI�u�W�F�N�g�̏�Ԃ���M���čX�V����R�[�h�������܂�
        }
    }

    public void ZizouMovieEnd(VideoPlayer vp) {
        ViewManager.Instance.playingView.CloseGateAndGoNext().Forget();
    }

    [PunRPC]
    public void SetZizouMovieId(int id) {
        myVideoPlayer.clip = zizouVideoList[id];
    }
}