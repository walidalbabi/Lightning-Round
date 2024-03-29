using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public enum GameState
{
    Loading,
    CheckingGameMode,
    Playing,
    GameFinish
}
public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;


    //Inspector Assign
    [Header("Game Settings")]
    [SerializeField] private float _maxTimeINQuestion;
    [SerializeField] private float _maxTimeINAnswer;
    [SerializeField] private float _maxTimeShowIndicator = 5;
    [SerializeField] private double _extraTimeBonusAmount;

    [Header("References")]
    [SerializeField] private Transform _playersTableTransform;
    [SerializeField] private InGameUIManager _inGameUiManager;
    [SerializeField] private QuestionNumberPanel _questionNumberPanel;
    [SerializeField] private TextMeshProUGUI _timerInQuestionPanel;
    [SerializeField] private TextMeshProUGUI _timerAnswerPanel;
    [SerializeField] private GameObject _blockPanel;

    [Header("Sounds")]
    [SerializeField] private AudioClip[] _questionAnsweredTrueSound;
    [SerializeField] private AudioClip[] _questionAnsweredFalseSound;



    //PR
    private Questions _currentSelectedQuestion;
    private float _timer;
    private float _currentMaxTime;
    private double _serverStartTime;
    private bool _isChoosingQuestion;
    private PhotonPlayer _currentPhotonPlayer;
    private int _currentRoundIndex = 1;
    private float _totalGameTime;
    private bool _canSetNextRound = true;
    private AudioSource _audioSourcer;

    private List<PhotonPlayer> _allPhotonPlayersList = new List<PhotonPlayer>();

    //PB
    [Header("--------------------------------------------Data-----------------------------------------------")]
    public GameState currentGameState;
    public bool isNormalGameMode;
    public List<Questions> _questionsList_1_Round_1 = new List<Questions>();
    public List<Questions> _questionsList_2_Round_1 = new List<Questions>();
    public List<Questions> _questionsList_3_Round_1 = new List<Questions>();
    public List<Questions> _questionsList_1_Round_2 = new List<Questions>();
    public List<Questions> _questionsList_2_Round_2 = new List<Questions>();
    public List<Questions> _questionsList_3_Round_2 = new List<Questions>();

    public List<QuestionScript> _allQuestionsBtnList = new List<QuestionScript>();
    public List<AnswerButton> _allAnswerBtnList = new List<AnswerButton>();
    public List<TableTitleSetter> _tableTitles = new List<TableTitleSetter>();

    public float totalGameTime { get { return _totalGameTime; } }

    public QuestionTable[] _tables = new QuestionTable[3];

    public string title;

    //Properties
    public Transform playersTableTransform { get { return _playersTableTransform; } }
    public float timer { get { return _timer; } }
    public float maxTimeShowIndicator { get { return _maxTimeShowIndicator; } }
    public float currentMaxTime { get { return _currentMaxTime; } }
    public double serverStartTime { get { return _serverStartTime; } }
    public Questions currentSelectedQuestion { get { return _currentSelectedQuestion; } }
    public PhotonPlayer currentPhotonPlayer { get { return _currentPhotonPlayer; } }
    public InGameUIManager inGameUiManager { get { return _inGameUiManager; } }
    public QuestionNumberPanel questionNumberPanel { get {return _questionNumberPanel; }  }
    public int currentRoundIndex { get { return _currentRoundIndex; } }

    public List<PhotonPlayer> allPhotonPlayersList { get { return _allPhotonPlayersList; } }


    public void SetGameState(GameState state)
    {
        currentGameState = state;
        OnEnterState();
    }

    private void OnEnterState()
    {

        if (currentGameState == GameState.CheckingGameMode)
        {
            //Set Game Mode
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue(true))
            {
                isNormalGameMode = true;
                _questionNumberPanel.SetGameMode(true);
            }
            else
            {
                isNormalGameMode = false;
                _questionNumberPanel.SetGameMode(false);
            }
        }
        else if (currentGameState == GameState.Playing)
        {
            _inGameUiManager.DisableLoadingPanel();
            SetUpQuestionsTable();
        }
        else if (currentGameState == GameState.GameFinish)
        {
            _currentPhotonPlayer.answersData.user.user_id = AuthManager.instance.userData.content.user.userId;
            _currentPhotonPlayer.answersData.user.score = _currentPhotonPlayer.score.ToString();
            _currentPhotonPlayer.GetTotalTime();
        }

    }

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        _audioSourcer = GetComponent<AudioSource>();
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

        foreach (var table in _tableTitles)
        {
            table.SetTitleCategory();
        }
    }

    public void TotalGameTimeFormat(float time)
    {
        float minutes = Mathf.Floor(time / 60);
        float seconds = Mathf.RoundToInt(time % 60);
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
        SetAnswerData(minutesS, secondsS);
    }

    private void SetAnswerData(string minutes, string seconds)
    {
        currentPhotonPlayer.answersData.match_id = PhotonNetwork.CurrentRoom.Name;
        currentPhotonPlayer.answersData.match_game_time = "00:" +minutes+":"+seconds;
        var jasonDataToSend = JsonUtility.ToJson(currentPhotonPlayer.answersData);
        Debug.Log(jasonDataToSend);
        AuthManager.instance.SendAnswers(jasonDataToSend);
    }


    public void AddQuestionButtonScriptToList(QuestionScript component)
    {
        _allQuestionsBtnList.Add(component);
    }

    public void ClearQuestionButtonScriptList()
    {
        _allQuestionsBtnList = new List<QuestionScript>();
    }

    public void ShowAnswerPanel(Questions data)
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
    public void ExtraTimeBonus()
    {
        _serverStartTime -= _extraTimeBonusAmount;
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
                _allAnswerBtnList[Random.Range(0, _allAnswerBtnList.Count)].AnswerQuestionFalseWhenTimeRunOut();
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
        if (_allPhotonPlayersList.Count == PhotonNetwork.CurrentRoom.PlayerCount) SetGameState(GameState.CheckingGameMode);
    }

    public void CheckIFCurrentRoundIsFinished()
    {
        if (!_canSetNextRound)
        {
            return;
        }

        if(AnswerQuestionManager.instance.currentAnswerIndex == _questionNumberPanel.questionsMark.Length)
        {
            if(_currentRoundIndex == 2)
            {
                //All Round Are Finished
                SetGameState(GameState.GameFinish);
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
                _inGameUiManager.EnableNextRoundPanel();
                Invoke("StartNextRound", 2.5f);
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
        _inGameUiManager.DisableNextRoundPanel();
        _currentPhotonPlayer.StartNewRoundPlayer();
        SetUpQuestionsTable();
        _canSetNextRound = true;
    }

    public void FindandSetTheTrueQuestionIndicator()
    {
        foreach (var answer in _allAnswerBtnList)
        {
            if (answer.IsTrueAnswer) answer.SetTrueIndicator();
        }
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

    public void PlaySound(bool istrue)
    {
        if (istrue)
        {
            int randomValue = Random.Range(0, _questionAnsweredTrueSound.Length);
            _audioSourcer.clip = _questionAnsweredTrueSound[randomValue];
            _audioSourcer.Play();
        }
        else
        {
            int randomValue = Random.Range(0, _questionAnsweredFalseSound.Length);
            _audioSourcer.clip = _questionAnsweredFalseSound[randomValue];
            _audioSourcer.Play();
        }
    }

}

