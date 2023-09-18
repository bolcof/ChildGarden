using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayingView : MonoBehaviour {
    [SerializeField] Image background;
    [SerializeField] TMP_Text purposeLabel;
    [SerializeField] List<Image> roundResults;
    [SerializeField] TMP_Text timerLabel;
    [SerializeField] Image progressBar;
}
