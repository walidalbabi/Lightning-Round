using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using System;

public class InGameUIManager : MonoBehaviour
{
    [SerializeField] private QuestionNumberPanel _questionNumberPanel;
    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private GameObject _QuestionMenuPanel;
    [SerializeField] private GameObject _AnswerMenuPanel;
    [SerializeField] private GameObject _scorePanel;
    [SerializeField] private GameObject _nextRoundPanel;
    [SerializeField] private GameObject _quitGamePanel;
    [SerializeField] private TextMeshProUGUI _questionCreator;
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

    public void DisableLoadingPanel()
    {
        _loadingPanel.SetActive(false);
        _QuestionMenuPanel.SetActive(true);
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

    public void EnableNextRoundPanel()
    {
        _nextRoundPanel.SetActive(true);
    }

    public void DisableNextRoundPanel()
    {
        _nextRoundPanel.SetActive(false);
    }

    public void OnTryingToQuitGame()
    {
        _quitGamePanel.SetActive(true);
    }

    public void BackToGame()
    {
        _quitGamePanel.SetActive(false);
    }

    public void BackToMenu()
    {
        PhotonNetwork.LeaveRoom();
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
        _questionCreator.text = _questionCreator.text + GameManager.instance.currentSelectedQuestion.teacher;
        _questionText.text = GameManager.instance.currentSelectedQuestion.title;
        _answer_1Text.text = GameManager.instance.currentSelectedQuestion.answers[0].ToString();
        _answer_2Text.text = GameManager.instance.currentSelectedQuestion.answers[1].ToString();
        _answer_3Text.text = GameManager.instance.currentSelectedQuestion.answers[2].ToString();

        foreach (var button in AnswerBtns)
        {
            if (button.gameObject.name == GameManager.instance.currentSelectedQuestion.correct_answer.ToString())
                button.SetIsTrueAnswer(true);
            else button.SetIsTrueAnswer(false);

            button.SetQuestionData(GameManager.instance.currentSelectedQuestion);
        }
    }


    public void OnExtrasTimePressed()
    {
        GameManager.instance.ExtraTimeBonus();
    }

    public void OnStrikePressed()
    {
       // UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.gameObject.GetComponent<Button>().interactable = false;
        UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.SetActive(false);
        foreach (var button in AnswerBtns)
        {
            if (!button.IsTrueAnswer)
            {
                button.StrikeButton();
                return;
            }

        }
    }
}
