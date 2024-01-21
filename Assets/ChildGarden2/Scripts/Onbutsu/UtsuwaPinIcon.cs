using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UtsuwaPinIcon : MonoBehaviour {
    [SerializeField] private float duration;
    [SerializeField] private SpriteRenderer pin, anata;
    public void StartAnimation() {
        Debug.Log("pin Start");
        pin.DOFade(endValue: 0f, duration: duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuint);
        anata.DOFade(endValue: 0f, duration: duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuint);
    }
}
