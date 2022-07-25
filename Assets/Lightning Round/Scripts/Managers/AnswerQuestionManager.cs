using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AnswerQuestionManager : MonoBehaviourPunCallbacks
{

    public static AnswerQuestionManager instance;

    //PB
    public int currentAnswerIndex;
    public int playersAnsweredQuestion;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {

        if (GameManager.instance.currentGameState != GameState.Playing) return;

        if(playersAnsweredQuestion == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            currentAnswerIndex++;
            RPC_ClearPlayerAnsweredQuestions();
        }
    }

    public void IncreasePlayerAnsweredCount()
    {
        photonView.RPC("RPC_IncreasePlayerAnsweredQuestions", RpcTarget.AllBuffered);
    }


    public void StartNewRound()
    {
        photonView.RPC("RPC_ClearCurrentAnswerIndexAndPlayerAnswerIndex", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_IncreasePlayerAnsweredQuestions()
    {
        playersAnsweredQuestion++;
    }

    [PunRPC]
    private void RPC_ClearCurrentAnswerIndexAndPlayerAnswerIndex()
    {
        currentAnswerIndex = 0;
    }

    [PunRPC]
    private void RPC_ClearPlayerAnsweredQuestions()
    {
        playersAnsweredQuestion = 0;
    }
}
