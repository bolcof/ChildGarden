using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Fader : MonoBehaviour {
    [SerializeField] private Image white;
    private void Awake() {
        white.enabled = true;
        white.DOFade(0.0f, 0.8f);
    }

    public void WhiteOut() {
        white.DOFade(1.0f, 1.8f);
    }

    public void Transit(float duration) {
        white.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(white.DOFade(0.999f, duration / 3));
        sequence.Append(white.DOFade(1.0f, duration / 3));
        sequence.Append(white.DOFade(0.0f, duration / 3));
    }
}