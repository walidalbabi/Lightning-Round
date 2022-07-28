using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScript : MonoBehaviour
{

    public static LoadingScript instance;


    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private A_ScaleOverTime _scaleAnim;

    private CanvasGroup _canvasGroup;
    private AudioSource _audioSource;

    public bool isActivated;


    void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _canvasGroup = _loadingPanel.GetComponent<CanvasGroup>();
       // _audioSource = GetComponent<AudioSource>();
    }
    public void StartLoading()
    {
        isActivated = true;
     //   _audioSource.PlayOneShot(_audioSource.clip);
        _loadingPanel.SetActive(true);
        _canvasGroup.LeanAlpha(1f,0.3f);
    }

    public void StopLoading()
    {
        StartCoroutine(StopL());
    }

    private IEnumerator StopL()
    {
        if (_canvasGroup != null)
            _canvasGroup.LeanAlpha(0f, 0.3f);
        if (_scaleAnim != null)
            _scaleAnim.ResetScaleToZero();
        yield return new WaitForSeconds(0.3f);
        _loadingPanel.SetActive(false);
        isActivated = false;
    }
}
