using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BackgroundSlider : MonoBehaviour {
    public void StartSlider() {
        transform.DOMoveX(-4.0f, 93f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }
}
