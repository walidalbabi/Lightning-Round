using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionScript : MonoBehaviour
{
    //Inspector Assign
    [SerializeField] private TextMeshProUGUI _score;

    //PR
    private Button _btn;

    //PB
    public Question data;

    private void Awake()
    {
        _btn = GetComponent<Button>();
    }

    public void SetData()
    {
        _score.text = data.score.ToString();
        _btn.interactable = true;
    }

    public void OnClickQuestion()
    {
        _btn.interactable = false;

        GameManager.instance.currentPhotonPlayer.SetIsAnswering(true);
        GameManager.instance.ShowAnswerPanel(data);
        GameManager.instance._allQuestionsBtnList.Remove(this);
    }
}
