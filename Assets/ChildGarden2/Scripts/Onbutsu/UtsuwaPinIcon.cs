using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UtsuwaPinIcon : MonoBehaviour {
    [SerializeField] private float duration;
    [SerializeField] private SpriteRenderer pin, anata;
    public void StartAnimation() {
        pin.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        pin.DOFade(endValue: 0f, duration: duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuint);
    }
}
