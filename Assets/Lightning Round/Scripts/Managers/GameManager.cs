using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;



public enum GameState
{
    Loading,
    Playing,
    GameFinish
}
public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;


    //Inspector Assign
    [SerializeField] private float _maxTimeINQuestion;
    [SerializeField] private float _maxTimeINAnswer;
    [SerializeField] private Transform _playersTableTransform;
    [SerializeField] private InGameUIManager _inGameUiManager;
    [SerializeField] private TextMeshProUGUI _timerInQuestionPanel;
    [SerializeField] private TextMeshProUGUI _timerAnswerPanel;
    [SerializeField] private GameObject _blockPanel;

    //PR
    private Question _currentSelectedQuestion;
    private float _timer;
    private float _currentMaxTime;
    private double _serverStartTime;
    private bool _isChoosingQuestion;
    private PhotonPlayer _currentPhotonPlayer;
    private int _currentRoundIndex = 1;
    private bool _canSetNextRound = true;
    
    private List<PhotonPlayer> _allPhotonPlayersList = new List<PhotonPlayer>();

    //PB
    public GameState currentGameState;
    public List<Question> _questionsList_1_Round_1 = new List<Question>();
    public List<Question> _questionsList_2_Round_1 = new List<Question>();
    public List<Question> _questionsList_3_Round_1 = new List<Question>();
    public List<Question> _questionsList_1_Round_2 = new List<Question>();
    public List<Question> _questionsList_2_Round_2 = new List<Question>();
    public List<Question> _questionsList_3_Round_2 = new List<Question>();

    public List<QuestionScript> _allQuestionsBtnList = new List<QuestionScript>();
    public List<AnswerButton> _allAnswerBtnList = new List<AnswerButton>();


    public QuestionTable[] _tables = new QuestionTable[3];

    //Properties
    public Transform playersTableTransform { get { return _playersTableTransform; } }
    public Question currentSelectedQuestion { get { return _currentSelectedQuestion; } }
    public PhotonPlayer currentPhotonPlayer { get { return _currentPhotonPlayer; } }
    public InGameUIManager inGameUiManager { get { return _inGameUiManager; } }
    public int currentRoundIndex { get { return _currentRoundIndex; } }

    public List<PhotonPlayer> allPhotonPlayersList { get { return _allPhotonPlayersList; } }


    public void SetGameState(GameState state)
    {
        currentGameState = state;
        OnEnterState();
    }

    private void OnEnterState()
    {
        if (currentGameState == GameState.Playing)
        {
            _inGameUiManager.DisableLoadingPanel();
            SetUpQuestionsTable();
        }
    }

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        GameObject playerAvatar = PhotonNetwork.Instantiate("PhotonPlayer", _playersTableTransform.transform.position, Quaternion.identity);
        _isChoosingQuestion = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (currentGameState != GameState.Playing) return;
        TimeFormat();
        UpdateTime();
        CheckForTime();
    }

    private void SetUpQuestionsTable()
    {
        PreparingToQuestionTime();

        foreach (var table in _tables)
        {
            table.SetQuestions();
        }
    }

    public void AddQuestionButtonScriptToList(QuestionScript component)
    {
        _allQuestionsBtnList.Add(component);
    }

    public void ClearQuestionButtonScriptList()
    {
        _allQuestionsBtnList = new List<QuestionScript>();
    }

    public void ShowAnswerPanel(Question data)
    {
        PreparingToAnswerTime();
        _currentSelectedQuestion = data;
        _isChoosingQuestion = false;
        _inGameUiManager.ShowAnswerMenu();
    }

    public void HideAnswerPanel()
    {
        PreparingToQuestionTime();
        _currentSelectedQuestion = null;
        _isChoosingQuestion = true;
        _inGameUiManager.ShowQuestionMenu();
    }

    private void TimeFormat()
    {
        float minutes = Mathf.Floor(_timer / 60);
        float seconds = Mathf.RoundToInt(_timer % 60);
        string minutesS;
        string secondsS;
        if (minutes < 10)
        {
            minutesS = "0" + minutes.ToString();
        }
        else
        {
            minutesS = minutes.ToString();
        }
        if (seconds < 10)
        {
            secondsS = "0" + Mathf.RoundToInt(seconds).ToString();
        }
        else
        {
            secondsS = Mathf.RoundToInt(seconds).ToString();
        }
        _timerAnswerPanel.text = minutesS + ":" + secondsS;
        _timerInQuestionPanel.text = minutesS + ":" + secondsS;
    }

    private void UpdateTime()
    {
        if (_currentPhotonPlayer != null)
            if (!_currentPhotonPlayer.isReadyForNextQuesiton)
            {
                _timer = 0;
                return;
            }
        _timer = _currentMaxTime - ((float)(PhotonNetwork.Time % _serverStartTime));
        _timer = Mathf.Clamp(_timer, 0, _currentMaxTime);
    }

    private void CheckForTime()
    {
        if (_currentPhotonPlayer != null)
            if (!_currentPhotonPlayer.isReadyForNextQuesiton)
            {
                _timer = 1;
                return;
            }

        if (_timer <= 0)
        {
            if (_isChoosingQuestion)
            {
                PreparingToAnswerTime();
                _allQuestionsBtnList[Random.Range(0, _allQuestionsBtnList.Count)].OnClickQuestion();
            }
            else if (!_isChoosingQuestion)
            {
                PreparingToQuestionTime();
                _allAnswerBtnList[Random.Range(0, _allAnswerBtnList.Count)].OnClickButton();
            }
        }
    }   

    private void PreparingToAnswerTime()
    {
        _serverStartTime = PhotonNetwork.Time;
        _currentMaxTime = _maxTimeINAnswer;
        Debug.Log(_timer);
    }

    private void PreparingToQuestionTime()
    {
        _serverStartTime = PhotonNetwork.Time;
        _currentMaxTime = _maxTimeINQuestion;
    }

    public void EnableBlockPanel()
    {
        if (!_blockPanel.activeInHierarchy)
            _blockPanel.SetActive(true);
    }

    public void DisableBlockPanel()
    {
        if (_blockPanel.activeInHierarchy)
        {
            _blockPanel.SetActive(false);
            PreparingToQuestionTime();
        }
    }
    public void AddPhotonPlayerComponent(PhotonPlayer component)
    {
        _currentPhotonPlayer = component;
    }

    public void UpdateScore(int amount)
    {
        if (_currentPhotonPlayer != null) _currentPhotonPlayer.IncreasePlayerScoreByAmount(amount);
    }

    public void AddPhotonPlayerToList(PhotonPlayer component)
    {
        _allPhotonPlayersList.Add(component);
        if (_allPhotonPlayersList.Count == PhotonNetwork.CurrentRoom.PlayerCount) SetGameState(GameState.Playing);
    }

    public void CheckIFCurrentRoundIsFinished()
    {
        if (!_canSetNextRound)
        {
            return;
        }

        if(AnswerQuestionManager.instance.currentAnswerIndex == 5)
        {
            if(_currentRoundIndex == 2)
            {
                //All Round Are Finished

                //To Stop Players Activities
                _currentPhotonPlayer.SetIsAnswering(true);
                _currentPhotonPlayer.SetIsReadyToAnswer(false);
                //
                 Invoke("SetScoreBoardPlayers", 0.5f);
                _canSetNextRound = false;


            }
            else if (_currentRoundIndex == 1)
            {
                //Start Round 2
                Invoke("StartNextRound", 0.5f);
                _canSetNextRound = false;
            }
        }
    }

    private void SetScoreBoardPlayers()
    {
        _inGameUiManager.ShowScorePanel();
        _canSetNextRound = true;
    }

    private void StartNextRound()
    {
        _currentRoundIndex++;
        AnswerQuestionManager.instance.StartNewRound();
        _inGameUiManager.questionNumberPanel.StartNewRound();
        _currentPhotonPlayer.StartNewRoundPlayer();
        SetUpQuestionsTable();
        _canSetNextRound = true;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        foreach (var player in _allPhotonPlayersList)
        {
            if (player.GetComponent<PhotonView>().CreatorActorNr == otherPlayer.ActorNumber)
            {
                player.SetISPlayerLeft();
            }
        }
    }

}

