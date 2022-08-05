using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerButton : MonoBehaviour
{

    [SerializeField] private GameObject _strikeIcon;

    private Button _currentBtn;
    private bool _isStrike;

    public bool IsTrueAnswer;



    private void OnEnable()
    {
        ResetStrikeIfNeeded();
    }

    private void Awake()
    {
        _currentBtn = GetComponent<Button>();
    }

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

    //Get Called whem player activate his strike ability to remove a answer
    public void StrikeButton()
    {
        _isStrike = true;
        _strikeIcon.SetActive(true);
        _currentBtn.interactable = false;
    }

    private void ResetStrikeIfNeeded()
    {
        if (_isStrike)
        {
            _isStrike = false;
            _strikeIcon.SetActive(false);
            _currentBtn.interactable = true;
        }
    }
}
