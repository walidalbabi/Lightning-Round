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

    public QuestionsMarks[] questionsMark { get { return _questionsMark; } }


    public void SetGameMode(bool isNormalGame)
    {
        if (isNormalGame)
        {
            foreach (var mark in _questionsMark)
            {
                mark.gameObject.SetActive(false);
            }
            _questionsMark = new QuestionsMarks[9];
        }


        GameManager.instance.SetGameState(GameState.Playing);
    }

    public void OnAnsweredQuestionTrue()
    {
        _questionsNumb.text = (AnswerQuestionManager.instance.currentAnswerIndex + 1) + "/" + _questionsMark.Length;
        _roundsNumb.text = "Round " +(GameManager.instance.currentRoundIndex) + "/2";

        if (_questionsMark.Length > 0 && _questionsMark[AnswerQuestionManager.instance.currentAnswerIndex] != null)
            _questionsMark[AnswerQuestionManager.instance.currentAnswerIndex].AnsweredTrue();
    }

    public void OnAnsweredQuestionFalse()
    {
        _questionsNumb.text = (AnswerQuestionManager.instance.currentAnswerIndex + 1) + "/" + _questionsMark.Length;
        _roundsNumb.text = "Round " + (GameManager.instance.currentRoundIndex) + "/2";

        if (_questionsMark.Length > 0 && _questionsMark[AnswerQuestionManager.instance.currentAnswerIndex] != null)
            _questionsMark[AnswerQuestionManager.instance.currentAnswerIndex].AnsweredFalse();
    }

    public void StartNewRound()
    {
        _questionsNumb.text = (AnswerQuestionManager.instance.currentAnswerIndex + 1) + "/" + _questionsMark.Length;
        _roundsNumb.text = "Round " + (GameManager.instance.currentRoundIndex) + "/2";

        if (!GameManager.instance.isNormalGameMode)
            foreach (var mark in _questionsMark)
            {
                mark.ClearMark();
            }
    }
}
