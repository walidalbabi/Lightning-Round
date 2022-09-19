using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TableTitleSetter : MonoBehaviour
{

    [SerializeField] QuestionScript _question;
    [SerializeField] TextMeshProUGUI _textTitle;

    public void SetTitleCategory()
    {
        _textTitle.text = _question.data.parent_category.name;
    }

}

