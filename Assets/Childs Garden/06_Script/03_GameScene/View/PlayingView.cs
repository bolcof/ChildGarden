using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayingView : MonoBehaviour {
    [SerializeField] Image background;
    [SerializeField] TMP_Text purposeLabel;

    [SerializeField] List<Image> roundResults;
    [SerializeField] List<Sprite> roundResultImage;/*0:lose 1:win*/

    [SerializeField] TMP_Text timerLabel;
    [SerializeField] Image progressBar;

    [SerializeField] GameObject winObject, loseObject;
    private bool hasWin;
    [SerializeField] GameObject toRuleSelectButton;

    public void RoundStart(int round, RuleManager.Rule currentRule) {
        purposeLabel.text = currentRule.explainText;
        for (int i = 0; i < 4; i++) {
            roundResults[i].sprite = null;
        }
        for (int i = 0; i < round - 1; i++) {
            //TODO:Result Hannei
            roundResults[i].sprite = roundResultImage[1];
        }
    }

    public void AppearWinObject() {
        winObject.SetActive(true);
        hasWin = true;
        toRuleSelectButton.SetActive(true);
    }

    public void AppearLoseObject() {
        loseObject.SetActive(true);
        hasWin = false;
    }

    public void ToRuleSelect() {

    }
}
