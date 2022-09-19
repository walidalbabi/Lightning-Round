using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PhotonPlayer : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject _avatarPrefab;

    //PR
    private float _score;
    private PhotonView _pv;
    private bool _isAnswering;
    private bool _isReadyForNextQuesiton;
    private bool _isLeft;
    private PlayerAvatarInfo _playerAvatarInfo;
   [SerializeField] private double _totalGameTime;
    [SerializeField]private double startTime;
    private bool startTimer;
    //PB
    public string playerName;
    public Sprite playerImage;
    public int answeredQuestionIndex;
    public MatchData matchData;
    public AnswersData answersData;
    public List<Questions> matchQuestions;
    public string title;


    //Properties
    public float score { get { return _score; } }
    public bool isAnswering { get { return _isAnswering; } }
    public bool isReadyForNextQuesiton { get { return _isReadyForNextQuesiton; } }
    public bool isLeft { get { return _isLeft; } }

    private void Awake()
    {
        _pv = GetComponent<PhotonView>();

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable { { "StartTime", PhotonNetwork.Time } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
            startTimer = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_pv.IsMine)
        {
            GameManager.instance.AddPhotonPlayerComponent(this);
            _pv.RPC("RPC_SetPlayerAvatar", RpcTarget.AllBuffered);
            if (PhotonNetwork.IsMasterClient)
            {
                matchData = AuthManager.instance.matchData;
                matchQuestions = matchData.content.match.questions;
                for (int i = 1; i < 4; i++)
                {
                    SetQuestionTitleList(i);
                    SetQuestionSubCategoryList(i);
                    SetQuestionParentCategoryList(i);
                    SetQuestionAnswersList(i);
                } 

            }
              
        }

        GameManager.instance.AddPhotonPlayerToList(this);
        SetIsAnswering(false);
        SetIsReadyToAnswer(true);

        Invoke("SetGameManagerStateAfterDelay", 3f);
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        object propsTime;
        if (propertiesThatChanged.TryGetValue("StartTime", out propsTime))
        {
            startTime = (double)propsTime;
        }
        startTimer = true;
    }

    private void SetGameManagerStateAfterDelay()
    {
        GameManager.instance.SetGameState(GameState.Playing);
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

        if (!startTimer) return;
        if (GameManager.instance.currentGameState == GameState.GameFinish) return;

        _totalGameTime = PhotonNetwork.Time - startTime;
    }

    public void GetTotalTime()
    {
        GameManager.instance.TotalGameTimeFormat((float)_totalGameTime);
    }


    public void SetIsAnswering(bool isAnswering)
    {
        _isAnswering = isAnswering;
    }

    [SerializeField] private int index;
    public void SetAnswerDataQuestion(AnswersQuestions question)
    {
      
        if (GameManager.instance.currentRoundIndex == 1) index = answeredQuestionIndex;
        else index = answeredQuestionIndex + 3;

        answersData.user.questions.Add(question);
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
        }
        else
        {
            SetIsReadyToAnswer(false);
            GameManager.instance.EnableBlockPanel();
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

    #region Match Data
    private void SetQuestionTitleList(int CategoryIndex)
    {
        if(CategoryIndex == 1)
        {
            for (int i = 0; i < 3; i++)
            {
                SendAllQuetsionToAllPlayers_Title(matchQuestions[i].title, matchQuestions[i].score, matchQuestions[i].teacher, matchQuestions[i].correct_answer, 1, i, CategoryIndex);
                SendAllQuetsionToAllPlayers_Title(matchQuestions[i + 3].title, matchQuestions[i + 3].score, matchQuestions[i + 3].teacher, matchQuestions[i + 3].correct_answer, 2, i, CategoryIndex);
            }

        }else if(CategoryIndex == 2)
        {
            for (int i = 6; i < 9; i++)
            {
                SendAllQuetsionToAllPlayers_Title(matchQuestions[i].title, matchQuestions[i].score, matchQuestions[i].teacher, matchQuestions[i].correct_answer, 1, i - 6, CategoryIndex);
                SendAllQuetsionToAllPlayers_Title(matchQuestions[i +3].title, matchQuestions[i + 3].score, matchQuestions[i + 3].teacher, matchQuestions[i + 3].correct_answer, 2, i - 6, CategoryIndex);
            }
        }
        else if (CategoryIndex == 3)
        {
            for (int i = 12; i < 15; i++)
            {
                SendAllQuetsionToAllPlayers_Title(matchQuestions[i].title, matchQuestions[i].score, matchQuestions[i].teacher, matchQuestions[i].correct_answer, 1, i- 12, CategoryIndex);
                SendAllQuetsionToAllPlayers_Title(matchQuestions[i + 3].title, matchQuestions[i + 3].score, matchQuestions[i + 3].teacher, matchQuestions[i + 3].correct_answer, 2, i - 12, CategoryIndex);
            }
             
        }

    }

    private void SetQuestionSubCategoryList(int CategoryIndex)
    {
        if (CategoryIndex == 1)
        {
            for (int i = 0; i < 3; i++)
            {
                SendAllQuetsionToAllPlayers_SubCategory( matchQuestions[i].subcategory.name, 1, i, CategoryIndex);
                SendAllQuetsionToAllPlayers_SubCategory( matchQuestions[i + 3].subcategory.name, 2, i, CategoryIndex);
            }

        }
        else if (CategoryIndex == 2)
        {
            for (int i = 6; i < 9; i++)
            {
                SendAllQuetsionToAllPlayers_SubCategory( matchQuestions[i].subcategory.name, 1, i - 6, CategoryIndex);
                SendAllQuetsionToAllPlayers_SubCategory( matchQuestions[i + 3].subcategory.name, 2, i - 6, CategoryIndex);
            }
        }
        else if (CategoryIndex == 3)
        {
            for (int i = 12; i < 15; i++)
            {
                SendAllQuetsionToAllPlayers_SubCategory( matchQuestions[i].subcategory.name, 1, i - 12, CategoryIndex);
                SendAllQuetsionToAllPlayers_SubCategory(matchQuestions[i + 3].subcategory.name, 2, i - 12, CategoryIndex);
            }

        }

    }

    private void SetQuestionParentCategoryList(int CategoryIndex)
    {
        if (CategoryIndex == 1)
        {
            for (int i = 0; i < 3; i++)
            {
                SendAllQuetsionToAllPlayers_ParentCategory(matchQuestions[i].parent_category.name, 1, i, CategoryIndex);
                SendAllQuetsionToAllPlayers_ParentCategory(matchQuestions[i + 3].parent_category.name, 2, i, CategoryIndex);
            }

        }
        else if (CategoryIndex == 2)
        {
            for (int i = 6; i < 9; i++)
            {
                SendAllQuetsionToAllPlayers_ParentCategory( matchQuestions[i].parent_category.name, 1, i - 6, CategoryIndex);
                SendAllQuetsionToAllPlayers_ParentCategory( matchQuestions[i + 3].parent_category.name, 2, i - 6, CategoryIndex);
            }
        }
        else if (CategoryIndex == 3)
        {
            for (int i = 12; i < 15; i++)
            {
                SendAllQuetsionToAllPlayers_ParentCategory( matchQuestions[i].parent_category.name, 1, i - 12, CategoryIndex);
                SendAllQuetsionToAllPlayers_ParentCategory( matchQuestions[i + 3].parent_category.name, 2, i - 12, CategoryIndex);
            }

        }

    }

    private void SetQuestionAnswersList(int CategoryIndex)
    {
        if (CategoryIndex == 1)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    SendAllQuetsionToAllPlayers_Answers(matchQuestions[i].answers[j].id, matchQuestions[i].answers[j].question_id, matchQuestions[i].answers[j].name , 1, i,j, CategoryIndex);
                    SendAllQuetsionToAllPlayers_Answers(matchQuestions[i + 3].answers[j].id, matchQuestions[i + 3].answers[j].question_id, matchQuestions[i + 3].answers[j].name, 2, i, j, CategoryIndex);
                }
            }
        }
        else if (CategoryIndex == 2)
        {
            for (int i = 6; i < 9; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    SendAllQuetsionToAllPlayers_Answers(matchQuestions[i].answers[j].id, matchQuestions[i].answers[j].question_id, matchQuestions[i].answers[j].name, 1, i - 6, j, CategoryIndex);
                    SendAllQuetsionToAllPlayers_Answers(matchQuestions[i + 3].answers[j].id, matchQuestions[i + 3].answers[j].question_id, matchQuestions[i + 3].answers[j].name, 2, i - 6, j, CategoryIndex);
                }
            }
        }
        else if (CategoryIndex == 3)
        {
            for (int i = 12; i < 15; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    SendAllQuetsionToAllPlayers_Answers(matchQuestions[i].answers[j].id, matchQuestions[i].answers[j].question_id, matchQuestions[i].answers[j].name, 1, i - 12, j, CategoryIndex);
                    SendAllQuetsionToAllPlayers_Answers(matchQuestions[i + 3].answers[j].id, matchQuestions[i + 3].answers[j].question_id, matchQuestions[i + 3].answers[j].name, 2, i - 12, j, CategoryIndex);
                }
            }
        }
    }
    

    public void SendAllQuetsionToAllPlayers_Title(string title, string score, string teacher, string correctAnswer, int round, int Index, int CategoryIndex)
    {
        _pv.RPC("RPC_SendAllQuetsionToAllPlayers_Title", RpcTarget.AllBuffered, title, score, teacher, correctAnswer, round, Index, CategoryIndex);
    }

    public void SendAllQuetsionToAllPlayers_SubCategory(string Name, int round, int Index, int CategoryIndex)
    {
        _pv.RPC("RPC_SendAllQuetsionToAllPlayers_SubCategory", RpcTarget.AllBuffered, Name, round, Index, CategoryIndex);
    }

    public void SendAllQuetsionToAllPlayers_ParentCategory(string Name, int round, int Index, int CategoryIndex)
    {
        _pv.RPC("RPC_SendAllQuetsionToAllPlayers_ParentCategory", RpcTarget.AllBuffered, Name, round, Index, CategoryIndex);
    }

    public void SendAllQuetsionToAllPlayers_Answers(string id, string questionID,string Name, int round, int IndexI, int IndexJ, int CategoryIndex)
    {
        _pv.RPC("RPC_SendAllQuetsionToAllPlayers_Answers", RpcTarget.AllBuffered, id, questionID, Name, round, IndexI, IndexJ, CategoryIndex);
    }

    [PunRPC]
    private void RPC_SendAllQuetsionToAllPlayers_Title(string title, string score, string teacher, string correctAnswer, int round,int Index, int CategoryIndex)
    {
        if(CategoryIndex == 1)
        {
            if (round == 1)
            {
                GameManager.instance._questionsList_1_Round_1[Index].title = title;
                GameManager.instance._questionsList_1_Round_1[Index].score = score;
                GameManager.instance._questionsList_1_Round_1[Index].teacher = teacher;
                GameManager.instance._questionsList_1_Round_1[Index].correct_answer = correctAnswer;
            }
            else if (round == 2)
            {
                GameManager.instance._questionsList_1_Round_2[Index].title = title;
                GameManager.instance._questionsList_1_Round_2[Index].score = score;
                GameManager.instance._questionsList_1_Round_2[Index].teacher = teacher;
                GameManager.instance._questionsList_1_Round_2[Index].correct_answer = correctAnswer;
            }
        }else if (CategoryIndex == 2)
        {
            if (round == 1)
            {
                GameManager.instance._questionsList_2_Round_1[Index].title = title;
                GameManager.instance._questionsList_2_Round_1[Index].score = score;
                GameManager.instance._questionsList_2_Round_1[Index].teacher = teacher;
                GameManager.instance._questionsList_2_Round_1[Index].correct_answer = correctAnswer;
            }
            else if (round == 2)
            {
                GameManager.instance._questionsList_2_Round_2[Index].title = title;
                GameManager.instance._questionsList_2_Round_2[Index].score = score;
                GameManager.instance._questionsList_2_Round_2[Index].teacher = teacher;
                GameManager.instance._questionsList_2_Round_2[Index].correct_answer = correctAnswer;
            }
        }
        else if (CategoryIndex == 3)
        {
            if (round == 1)
            {
                GameManager.instance._questionsList_3_Round_1[Index].title = title;
                GameManager.instance._questionsList_3_Round_1[Index].score = score;
                GameManager.instance._questionsList_3_Round_1[Index].teacher = teacher;
                GameManager.instance._questionsList_3_Round_1[Index].correct_answer = correctAnswer;
            }
            else if (round == 2)
            {
                GameManager.instance._questionsList_3_Round_2[Index].title = title;
                GameManager.instance._questionsList_3_Round_2[Index].score = score;
                GameManager.instance._questionsList_3_Round_2[Index].teacher = teacher;
                GameManager.instance._questionsList_3_Round_2[Index].correct_answer = correctAnswer;
            }
        }
    }

    [PunRPC]
    private void RPC_SendAllQuetsionToAllPlayers_SubCategory(string Name, int round, int Index, int CategoryIndex)
    {
        if (CategoryIndex == 1)
        {
            if (round == 1)
            {
                GameManager.instance._questionsList_1_Round_1[Index].subcategory.name = Name;

            }
            else if (round == 2)
            {
                GameManager.instance._questionsList_1_Round_2[Index].subcategory.name = Name;
            }
        }
        else if (CategoryIndex == 2)
        {
            if (round == 1)
            {
                GameManager.instance._questionsList_2_Round_1[Index].subcategory.name = Name;
            }
            else if (round == 2)
            {
                GameManager.instance._questionsList_2_Round_2[Index].subcategory.name = Name;
            }
        }
        else if (CategoryIndex == 3)
        {
            if (round == 1)
            {
                GameManager.instance._questionsList_3_Round_1[Index].subcategory.name = Name;
            }
            else if (round == 2)
            {
                GameManager.instance._questionsList_3_Round_2[Index].subcategory.name = Name;
            }
        }
    }

    [PunRPC]
    private void RPC_SendAllQuetsionToAllPlayers_ParentCategory(string Name, int round, int Index, int CategoryIndex)
    {
        if (CategoryIndex == 1)
        {
            if (round == 1)
            {
                GameManager.instance._questionsList_1_Round_1[Index].parent_category.name = Name;

            }
            else if (round == 2)
            {
                GameManager.instance._questionsList_1_Round_2[Index].parent_category.name = Name;
            }
        }
        else if (CategoryIndex == 2)
        {
            if (round == 1)
            {
                GameManager.instance._questionsList_2_Round_1[Index].parent_category.name = Name;
            }
            else if (round == 2)
            {
                GameManager.instance._questionsList_2_Round_2[Index].parent_category.name = Name;
            }
        }
        else if (CategoryIndex == 3)
        {
            if (round == 1)
            {
                GameManager.instance._questionsList_3_Round_1[Index].parent_category.name = Name;
            }
            else if (round == 2)
            {
                GameManager.instance._questionsList_3_Round_2[Index].parent_category.name = Name;
            }
        }
    }

    [PunRPC]
    private void RPC_SendAllQuetsionToAllPlayers_Answers(string id, string questionID, string Name, int round, int IndexI, int IndexJ, int CategoryIndex)
    {
        if (CategoryIndex == 1)
        {
            if (round == 1)
            {
                GameManager.instance._questionsList_1_Round_1[IndexI].answers[IndexJ].id = id;
                GameManager.instance._questionsList_1_Round_1[IndexI].answers[IndexJ].question_id = questionID;
                GameManager.instance._questionsList_1_Round_1[IndexI].answers[IndexJ].name = Name;

            }
            else if (round == 2)
            {
                GameManager.instance._questionsList_1_Round_2[IndexI].answers[IndexJ].id = id;
                GameManager.instance._questionsList_1_Round_2[IndexI].answers[IndexJ].question_id = questionID;
                GameManager.instance._questionsList_1_Round_2[IndexI].answers[IndexJ].name = Name;
            }
        }
        else if (CategoryIndex == 2)
        {
            if (round == 1)
            {
                GameManager.instance._questionsList_2_Round_1[IndexI].answers[IndexJ].id = id;
                GameManager.instance._questionsList_2_Round_1[IndexI].answers[IndexJ].question_id = questionID;
                GameManager.instance._questionsList_2_Round_1[IndexI].answers[IndexJ].name = Name;

            }
            else if (round == 2)
            {
                GameManager.instance._questionsList_2_Round_2[IndexI].answers[IndexJ].id = id;
                GameManager.instance._questionsList_2_Round_2[IndexI].answers[IndexJ].question_id = questionID;
                GameManager.instance._questionsList_2_Round_2[IndexI].answers[IndexJ].name = Name;
            }
        }
        else if (CategoryIndex == 3)
        {
            if (round == 1)
            {
                GameManager.instance._questionsList_3_Round_1[IndexI].answers[IndexJ].id = id;
                GameManager.instance._questionsList_3_Round_1[IndexI].answers[IndexJ].question_id = questionID;
                GameManager.instance._questionsList_3_Round_1[IndexI].answers[IndexJ].name = Name;

            }
            else if (round == 2)
            {
                GameManager.instance._questionsList_3_Round_2[IndexI].answers[IndexJ].id = id;
                GameManager.instance._questionsList_3_Round_2[IndexI].answers[IndexJ].question_id = questionID;
                GameManager.instance._questionsList_3_Round_2[IndexI].answers[IndexJ].name = Name;
            }
        }
    }
    #endregion Match Data
}
