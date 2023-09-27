using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class EndingView : Photon.PunBehaviour {
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private List<VideoClip> endingVideos = new List<VideoClip>();

    public void Set(int id) {
        videoPlayer.clip = endingVideos[id];
        videoPlayer.Play();
    }
}