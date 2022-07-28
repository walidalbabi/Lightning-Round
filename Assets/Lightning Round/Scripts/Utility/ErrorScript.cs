using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ErrorScript : MonoBehaviour
{
    public static ErrorScript instance;


    [SerializeField] private TextMeshProUGUI _errorText;
    [SerializeField] private GameObject _errorPanel;
    [SerializeField] private A_ScaleOverTime _scaleAnim;

    private CanvasGroup _canvasGroup;
    private AudioSource _audioSource;
    private string _reconnectTo;


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


        _canvasGroup = _errorPanel.GetComponent<CanvasGroup>();
      //  _audioSource = GetComponent<AudioSource>();
    }
    public void StartErrorMsg(string txtError, string errorType)
    {
      //  _audioSource.PlayOneShot(_audioSource.clip);
        _reconnectTo = errorType;
        _canvasGroup.LeanAlpha(1f, 0.3f);

        _errorText.text = txtError;
        _errorPanel.SetActive(true);

        //if (isRestart)
        //    RestartGameBtn.SetActive(true);
        //else
        //    RestartGameBtn.SetActive(false);

        //if (isReconnect)
        //    ReconnectGameBtn.SetActive(true);
        //else
        //    ReconnectGameBtn.SetActive(false);

    }

    public void StopErrorMsg()
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
        _errorPanel.SetActive(false);
    }

    public void RestartGame()
    {

        //NetworkManager.instance.DestroyObj();
        //PlayFabLogin.instance.DestroyObj();
        //VivoxManager.instance.DestroyObj();

        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        StopErrorMsg();
    }

    public void Reconnect()
    {
        if (_reconnectTo == "photon")
        {
            Photon.Pun.PhotonNetwork.Reconnect();
        }
        else if (_reconnectTo == "vivox")
        {
            Invoke("LoginToVivox", 2f);
        }
        else
        {
            StopErrorMsg();
        }
           
    }
}
