using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerButton : MonoBehaviour
{
    public bool IsTrueAnswer;

    public void SetIsTrueAnswer(bool isTrue)
    {
        IsTrueAnswer = isTrue;
    }

    public void AnswerQuestionFalseWhenTimeRunOut()
    {
        GameManager.instance.inGameUiManager.questionNumberPanel.OnAnsweredQuestionFalse();
        GameManager.instance.currentPhotonPlayer.SetIsAnswering(false);
        GameManager.instance.HideAnswerPanel();
        GameManager.instance.currentPhotonPlayer.SetISAnsweredQuestion();
    }

    public void OnClickButton()
    {
        if (IsTrueAnswer)
        {
            GameManager.instance.UpdateScore(GameManager.instance.currentSelectedQuestion.score);
            GameManager.instance.inGameUiManager.questionNumberPanel.OnAnsweredQuestionTrue();
        }
        else
        {
            GameManager.instance.inGameUiManager.questionNumberPanel.OnAnsweredQuestionFalse();
        }
        GameManager.instance.currentPhotonPlayer.SetIsAnswering(false);
        GameManager.instance.HideAnswerPanel();
        GameManager.instance.currentPhotonPlayer.SetISAnsweredQuestion();
    }
}
