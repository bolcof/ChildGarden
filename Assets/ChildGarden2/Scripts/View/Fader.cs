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
}