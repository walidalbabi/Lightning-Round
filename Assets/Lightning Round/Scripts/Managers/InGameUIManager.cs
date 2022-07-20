using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameUIManager : MonoBehaviour
{
    [SerializeField] private QuestionNumberPanel _questionNumberPanel;
    [SerializeField] private GameObject _QuestionMenuPanel;
    [SerializeField] private GameObject _AnswerMenuPanel;
    [SerializeField] private GameObject _scorePanel;
    [SerializeField] private TextMeshProUGUI _questionText;
    [SerializeField] private TextMeshProUGUI _answer_1Text;
    [SerializeField] private TextMeshProUGUI _answer_2Text;
    [SerializeField] private TextMeshProUGUI _answer_3Text;
    [SerializeField] private AnswerButton[] AnswerBtns = new AnswerButton[3];

    public QuestionNumberPanel questionNumberPanel { get { return _questionNumberPanel; } }

    public void ShowQuestionMenu()
    {
        _AnswerMenuPanel.SetActive(false);
        _QuestionMenuPanel.SetActive(true);
    }

    public void ShowAnswerMenu()
    {
        _AnswerMenuPanel.SetActive(true);
        _QuestionMenuPanel.SetActive(false);

        SetAnswerPanelData();
    }

    public void ShowScorePanel()
    {
        _scorePanel.SetActive(true);
    }


    private void SetAnswerPanelData()
    {
        _questionText.text = GameManager.instance.currentSelectedQuestion.questionData;
        _answer_1Text.text = GameManager.instance.currentSelectedQuestion.answer_1;
        _answer_2Text.text = GameManager.instance.currentSelectedQuestion.answer_2;
        _answer_3Text.text = GameManager.instance.currentSelectedQuestion.answer_3;

        foreach (var button in AnswerBtns)
        {
            if (button.gameObject.name == GameManager.instance.currentSelectedQuestion.trueAnswerIndex.ToString())
                button.SetIsTrueAnswer(true);
            else button.SetIsTrueAnswer(false);
        }
    }
}
