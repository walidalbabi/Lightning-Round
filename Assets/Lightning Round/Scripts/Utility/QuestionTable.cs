using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionTable : MonoBehaviour
{
    [SerializeField] private int _tableIndex;
    [SerializeField] private string QuestionCategory;
    [SerializeField] private List<QuestionScript> _questions = new List<QuestionScript>();


    public void SetQuestions()
    {

        if (GameManager.instance.currentRoundIndex == 1)
        {
            SetQuestionsOfFirstRound();
        }
        else if (GameManager.instance.currentRoundIndex == 2)
        {
            SetQuestionsOfSecondRound();
        }

    }

    private void SetQuestionsOfFirstRound()
    {
       // GameManager.instance.ClearQuestionButtonScriptList();

        for (int i = 0; i < _questions.Count; i++)
        {
            if (_tableIndex == 1)
                _questions[i].data = GameManager.instance._questionsList_1_Round_1[i];
            else if (_tableIndex == 2)
                _questions[i].data = GameManager.instance._questionsList_2_Round_1[i];
            else if (_tableIndex == 3)
                _questions[i].data = GameManager.instance._questionsList_3_Round_1[i];

            _questions[i].SetData();
            GameManager.instance.AddQuestionButtonScriptToList(_questions[i]);
        }
    }

    private void SetQuestionsOfSecondRound()
    {
     //   GameManager.instance.ClearQuestionButtonScriptList();

        for (int i = 0; i < _questions.Count; i++)
        {
            if (_tableIndex == 1)
                _questions[i].data = GameManager.instance._questionsList_1_Round_2[i];
            else if (_tableIndex == 2)
                _questions[i].data = GameManager.instance._questionsList_2_Round_2[i];
            else if (_tableIndex == 3)
                _questions[i].data = GameManager.instance._questionsList_3_Round_2[i];

            _questions[i].SetData();
            GameManager.instance.AddQuestionButtonScriptToList(_questions[i]);
        }
    }
}
