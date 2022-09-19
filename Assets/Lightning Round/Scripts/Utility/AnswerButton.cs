using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerButton : MonoBehaviour
{

    [SerializeField] private GameObject _strikeIcon;
    [SerializeField] private TextMeshProUGUI _text;

    private Button _currentBtn;
    private bool _isStrike;

    public bool IsTrueAnswer;

    private Questions question;
    private AnswersQuestions answerQuestion;



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

    public void SetQuestionData(Questions question)
    {
        this.question = question;
        _text.text = question.answers[int.Parse(gameObject.name) - 1].name;
    }

    public void AnswerQuestionFalseWhenTimeRunOut()
    {
        GameManager.instance.inGameUiManager.questionNumberPanel.OnAnsweredQuestionFalse();
        GameManager.instance.currentPhotonPlayer.SetIsAnswering(false);
        GameManager.instance.HideAnswerPanel();
        GameManager.instance.currentPhotonPlayer.SetISAnsweredQuestion();
        //     GameManager.instance.currentPhotonPlayer.SetAnswerDataQuestion(question, int.Parse(gameObject.name), 2);
        SetAnswerData("2");
    }

    public void OnClickButton()
    {
        if (IsTrueAnswer)
        {
            GameManager.instance.UpdateScore(int.Parse(GameManager.instance.currentSelectedQuestion.score));
            GameManager.instance.inGameUiManager.questionNumberPanel.OnAnsweredQuestionTrue();
            //   GameManager.instance.currentPhotonPlayer.SetAnswerDataQuestion(question, int.Parse(gameObject.name),1);
            SetAnswerData("1");
        }
        else
        {
            GameManager.instance.inGameUiManager.questionNumberPanel.OnAnsweredQuestionFalse();
            //   GameManager.instance.currentPhotonPlayer.SetAnswerDataQuestion(question, int.Parse(gameObject.name), 0);
            SetAnswerData("0");
        }
        GameManager.instance.currentPhotonPlayer.SetIsAnswering(false);
        GameManager.instance.HideAnswerPanel();
        GameManager.instance.currentPhotonPlayer.SetISAnsweredQuestion();
    }


    private void SetAnswerData(string state)
    {
        answerQuestion = new AnswersQuestions();
        answerQuestion.answer_id = int.Parse(gameObject.name).ToString();
        answerQuestion.question_id = question.answers[0].question_id;
        answerQuestion.status_answer = state;
        GameManager.instance.currentPhotonPlayer.SetAnswerDataQuestion(answerQuestion);
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
