using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionsMarks : MonoBehaviour
{

    [SerializeField] private Sprite _enabledImg;
    [SerializeField] private Sprite _disabledImg;
    [SerializeField] private GameObject _trueMark;
    [SerializeField] private GameObject _falseMark;


    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void AnsweredTrue()
    {
        _image.sprite = _enabledImg;
        _trueMark.SetActive(true);
    }

    public void AnsweredFalse()
    {
        _image.sprite = _enabledImg;
        _falseMark.SetActive(true);
    }

    public void ClearMark()
    {
        _image.sprite = _disabledImg;
        _trueMark.SetActive(false);
        _falseMark.SetActive(false);
    }
}
