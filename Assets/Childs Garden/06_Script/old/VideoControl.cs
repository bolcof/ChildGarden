using UnityEngine;
using UnityEngine.Video;

public class VideoControl : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.enabled = false; // 最初はSpriteを非表示にする

        videoPlayer.loopPointReached += VideoStarted;
        videoPlayer.Play();
    }

    void VideoStarted(VideoPlayer source)
    {
        spriteRenderer.enabled = true; // ビデオが再生されたらSpriteを表示
    }
}
