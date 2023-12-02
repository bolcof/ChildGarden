using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

public class AngelSpeaking : MonoBehaviour {
    [SerializeField] VideoPlayer player_nutral, player_sad, player_happy;
    [SerializeField] Image firstCommit, nearToWin, nearToLose, advance, other;
    private bool ableToSpeak;

    private bool advanceAppeared, otherAppeared;

    private void Awake() {
        player_sad.loopPointReached += OnEndVideo_Sad;
        player_happy.loopPointReached += OnEndVideo_Happy;
        ableToSpeak = true;

        player_sad.gameObject.SetActive(false);
        player_happy.gameObject.SetActive(false);
        firstCommit.gameObject.SetActive(false);
        nearToWin.gameObject.SetActive(false);
        nearToLose.gameObject.SetActive(false);
        advance.gameObject.SetActive(false);
        other.gameObject.SetActive(false);

        advanceAppeared = false;
        otherAppeared = false;
    }

    public void OnEndVideo_Sad(VideoPlayer vp) {
        player_sad.time = 0.0f;
        player_sad.gameObject.SetActive(false);
        player_nutral.gameObject.SetActive(true);
        player_nutral.Play();
    }

    public void OnEndVideo_Happy(VideoPlayer vp) {
        player_happy.time = 0.0f;
        player_happy.gameObject.SetActive(false);
        player_nutral.gameObject.SetActive(true);
        player_nutral.Play();
    }

    public async UniTask FirstCommit() {
        if (ableToSpeak) {
            player_nutral.gameObject.SetActive(false);
            player_happy.gameObject.SetActive(true);
            player_happy.Play();
            firstCommit.gameObject.SetActive(true);
            ableToSpeak = false;
            await UniTask.Delay(3500);
            firstCommit.gameObject.SetActive(false);
            ableToSpeak = true;
        }
    }

    public async UniTask NearToWin() {
        if (ableToSpeak) {
            player_nutral.gameObject.SetActive(false);
            player_happy.gameObject.SetActive(true);
            player_happy.Play();
            nearToWin.gameObject.SetActive(true);
            ableToSpeak = false;
            await UniTask.Delay(3500);
            nearToWin.gameObject.SetActive(false);
            ableToSpeak = true;
        }
    }

    public async UniTask NearToLose() {
        if (ableToSpeak) {
            player_nutral.gameObject.SetActive(false);
            player_sad.gameObject.SetActive(true);
            player_sad.Play();
            nearToLose.gameObject.SetActive(true);
            ableToSpeak = false;
            await UniTask.Delay(3500);
            nearToLose.gameObject.SetActive(false);
            ableToSpeak = true;
        }
    }

    public async UniTask Advance() {
        if (ableToSpeak && !advanceAppeared) {
            advance.gameObject.SetActive(true);
            ableToSpeak = false;
            await UniTask.Delay(3500);
            advance.gameObject.SetActive(false);
            ableToSpeak = true;
            advanceAppeared = true;
        }
    }

    public async UniTask OtherOnbutsu() {
        if (ableToSpeak && !otherAppeared) {
            other.gameObject.SetActive(true);
            ableToSpeak = false;
            await UniTask.Delay(3500);
            other.gameObject.SetActive(false);
            ableToSpeak = true;
            otherAppeared = true;
        }
    }
}