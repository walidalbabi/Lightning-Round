using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

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
    [SerializeField] private PlayerAvatarEndScore _playerAvatarEndScore;
    [SerializeField] private Transform _endGameScoreBoard;


    public PhotonPlayer[] _playersListInOrder;
    
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
        if (_scorePanel.activeInHierarchy) return;

        Debug.Log("Get Called");
        CalculatePlaces();

        for (int i = 0; i < _playersListInOrder.Length; i++)
        {
            var obj = Instantiate(_playerAvatarEndScore, _endGameScoreBoard);
            obj.SetAvatarInfo(_playersListInOrder[i].playerName, (int)_playersListInOrder[i].score, _playersListInOrder[i].playerImage, i);
        }

        _scorePanel.SetActive(true);
    }

    private void CalculatePlaces()
    {

        _playersListInOrder = new PhotonPlayer[GameManager.instance.allPhotonPlayersList.Count];

        for (int i = 0; i < GameManager.instance.allPhotonPlayersList.Count; i++)
        {
            _playersListInOrder[i] = GameManager.instance.allPhotonPlayersList[i];

        }

        Array.Sort(_playersListInOrder, (player1, player2) =>
        {
            return player1.score.CompareTo(player2.score);
        });

        Array.Reverse(_playersListInOrder);

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
