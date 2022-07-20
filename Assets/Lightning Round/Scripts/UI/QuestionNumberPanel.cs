using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionNumberPanel : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _questionsNumb;
    [SerializeField] private TextMeshProUGUI _roundsNumb;
    [SerializeField] private QuestionsMarks[] _questionsMark;


    public void OnAnsweredQuestionTrue()
    {
        _questionsNumb.text = (AnswerQuestionManager.instance.currentAnswerIndex + 1) + "/5";
        _roundsNumb.text = "Round " +(GameManager.instance.currentRoundIndex) + "/2";
        _questionsMark[AnswerQuestionManager.instance.currentAnswerIndex].AnsweredTrue();
    }

    public void OnAnsweredQuestionFalse()
    {
        _questionsNumb.text = (AnswerQuestionManager.instance.currentAnswerIndex + 1) + "/5";
        _roundsNumb.text = "Round " + (GameManager.instance.currentRoundIndex) + "/2";
        _questionsMark[AnswerQuestionManager.instance.currentAnswerIndex].AnsweredFalse();
    }

    public void StartNewRound()
    {
        _questionsNumb.text = (AnswerQuestionManager.instance.currentAnswerIndex + 1) + "/5";
        _roundsNumb.text = "Round " + (GameManager.instance.currentRoundIndex) + "/2";
        foreach (var mark in _questionsMark)
        {
            mark.ClearMark();
        }
    }
}
