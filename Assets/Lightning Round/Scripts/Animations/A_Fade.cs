using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Fade : MonoBehaviour
{

    [SerializeField] private float _time;
    [SerializeField] private bool _isFadeIn;

    private CanvasGroup _canvasGroup;


    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        if (_isFadeIn)
            StartFadeIn();
        else StartFadeOut();
    }

    private void StartFadeIn()
    {
        _canvasGroup.LeanAlpha(1f, _time);
    }

    private void StartFadeOut()
    {
        _canvasGroup.LeanAlpha(0f, _time);
    }
}
