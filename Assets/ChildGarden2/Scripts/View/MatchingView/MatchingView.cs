using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class MatchingView : MonoBehaviour {

    [SerializeField] private Image background;
    [SerializeField] private Image playerCount;
    [SerializeField] private List<Sprite> countSprites = new List<Sprite>();

    [SerializeField] private List<Image> contents = new List<Image>();

    public async UniTask Set() {
        background.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        background.DOFade(1.0f, 1.5f);
        foreach (var c in contents) {
            c.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        }
        await UniTask.Delay(1500);
        foreach (var c in contents) {
            c.DOFade(1.0f, 1.5f);
        }
    }

    private void Update() {
        switch (GetCurrentRoomPlayerCount()) {
            case 1:
                playerCount.sprite = countSprites[1];
                break;
            case 2:
                playerCount.sprite = countSprites[2];
                break;
            case 3:
                playerCount.sprite = countSprites[3];
                break;
            default:
                playerCount.sprite = countSprites[0];
                break;
        }
    }

    private int GetCurrentRoomPlayerCount() {
        if (RoomConector.Instance.networkRunner != null && RoomConector.Instance.networkRunner.SessionInfo.IsValid) {
            return RoomConector.Instance.networkRunner.SessionInfo.PlayerCount;
        }
        return 0;
    }

    public async UniTask Disappear() {
        GameObject.Find("FaderCanvas").GetComponent<Fader>().Transit(1.5f);
        await UniTask.Delay(500);
        this.gameObject.SetActive(false);
    }
}