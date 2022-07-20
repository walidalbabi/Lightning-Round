using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PhotonPlayer : MonoBehaviour
{
    //Inspector Assign
    [SerializeField] private TextMeshProUGUI _playerName; 
    [SerializeField] private TextMeshProUGUI _playerScore;

    //PR
    private float _score;
    private PhotonView _pv;
    private bool _isAnswering;
    public bool _isReadyForNextQuesiton;
    //PB
    public int answeredQuestionIndex;


    //Properties
    public bool isAnswering { get { return _isAnswering; } }
    public bool isReadyForNextQuesiton { get { return _isReadyForNextQuesiton; } }

    private void Awake()
    {
        _pv = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_pv.IsMine)
        {
            GameManager.instance.AddPhotonPlayerComponent(this);
        }

        _playerName.text = _pv.Owner.NickName;
        transform.SetParent(GameManager.instance.playersTableTransform);
        GameManager.instance.AddPhotonPlayerToList(this);
        SetIsAnswering(false);
        SetIsReadyToAnswer(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(_pv.IsMine)
            if (!_isAnswering)
                CheckIfCanAnswer();
    }


    public void SetIsAnswering(bool isAnswering)
    {
        _isAnswering = isAnswering;
    } 
    
    public void SetIsReadyToAnswer(bool isReady)
    {
        _isReadyForNextQuesiton = isReady;
    }

    private void CheckIfCanAnswer()
    {

        if (answeredQuestionIndex == AnswerQuestionManager.instance.currentAnswerIndex)
        {
            SetIsReadyToAnswer(true);
            GameManager.instance.DisableBlockPanel();
            GameManager.instance.CheckIFCurrentRoundIsFinished();
            Debug.Log("true");
        }
        else
        {
            SetIsReadyToAnswer(false);
            GameManager.instance.EnableBlockPanel();
            Debug.Log("false");
        }
    }

    public void SetISAnsweredQuestion()
    {
        if (!_pv.IsMine) return;
        _pv.RPC("RPC_IncreaseAnsweredQuestionIndex", RpcTarget.AllBuffered);
        AnswerQuestionManager.instance.IncreasePlayerAnsweredCount();
    }

    [PunRPC]
    private void RPC_IncreaseAnsweredQuestionIndex()
    {
        answeredQuestionIndex = answeredQuestionIndex + 1;
    }

    public void ClearAnswerIndex()
    {
        answeredQuestionIndex = 0;
    }

    public void IncreasePlayerScoreByAmount(float amount)
    {
        _pv.RPC("RPC_IncreasePlayerScoreByAmount", RpcTarget.AllBuffered, amount);
    }

    [PunRPC]
    private void RPC_IncreasePlayerScoreByAmount(float amount)
    {
        _score += amount;
        _playerScore.text = _score.ToString() + " pts";
    }


    public void StartNewRoundPlayer()
    {
        _pv.RPC("RPC_StartNewRoundPlayer", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_StartNewRoundPlayer()
    {
        SetIsAnswering(false);
        SetIsReadyToAnswer(true);
        answeredQuestionIndex = 0;
    }
}
