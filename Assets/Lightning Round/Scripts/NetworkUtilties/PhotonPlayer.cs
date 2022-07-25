using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PhotonPlayer : MonoBehaviour
{
    [SerializeField] GameObject _avatarPrefab;

    //PR
    private float _score;
    private PhotonView _pv;
    private bool _isAnswering;
    private bool _isReadyForNextQuesiton;
    private bool _isLeft;
    private PlayerAvatarInfo _playerAvatarInfo;
    //PB
    public string playerName;
    public Sprite playerImage;
    public int answeredQuestionIndex;


    //Properties
    public float score { get { return _score; } }
    public bool isAnswering { get { return _isAnswering; } }
    public bool isReadyForNextQuesiton { get { return _isReadyForNextQuesiton; } }
    public bool isLeft { get { return _isLeft; } }

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
            _pv.RPC("RPC_SetPlayerAvatar", RpcTarget.AllBuffered);
        }

        GameManager.instance.AddPhotonPlayerToList(this);
        SetIsAnswering(false);
        SetIsReadyToAnswer(true);
    }

    [PunRPC]
    private void RPC_SetPlayerAvatar()
    {
        var obj = Instantiate(_avatarPrefab, GameManager.instance.playersTableTransform);
        _playerAvatarInfo = obj.GetComponent<PlayerAvatarInfo>();
        SetPlayerInfo();
    }

    private void SetPlayerInfo()
    {
        playerName = _pv.Owner.NickName;
        playerImage = null;
        _playerAvatarInfo.SetPlayerNameAndImage(playerName, playerImage);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isLeft || GameManager.instance.currentGameState != GameState.Playing) return;

        if (_pv.IsMine)
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
        if (_playerAvatarInfo != null) _playerAvatarInfo.UpdatePlayerScore((int)_score);
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

    public void SetISPlayerLeft()
    {
        _isLeft = true;
    }

}
