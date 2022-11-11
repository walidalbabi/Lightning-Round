using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    //Inspector Assign
    [SerializeField] private List<GameObject> _mainPanels = new List<GameObject>();
    [SerializeField] private List<GameObject> _panels = new List<GameObject>();


    [Header("Login Fields")]
    [SerializeField] private TMP_InputField _emailIFLogin;
    [SerializeField] private TMP_InputField _passwordIFLogin;

    [Header("Register Fields")]
    [SerializeField] private TMP_InputField _firstNameIF;
    [SerializeField] private TMP_InputField _lastNameIF;
    [SerializeField] private TMP_InputField _emailIF;
    [SerializeField] private TMP_InputField _passwordIF;
    [SerializeField] private Dropdown _countryDropDown;

    [Header("Profile")]
    [SerializeField] private TextMeshProUGUI _firstName;
    [SerializeField] private TextMeshProUGUI _lastName;
    [SerializeField] private TextMeshProUGUI _gameTime;
    [SerializeField] private TextMeshProUGUI _score;
    [SerializeField] private TextMeshProUGUI _wallet;
    [SerializeField] private TextMeshProUGUI _totalQuestions;
    [SerializeField] private TextMeshProUGUI _trueAnswers;
    [SerializeField] private TextMeshProUGUI _falseAnswer;
    [SerializeField] private TextMeshProUGUI _winRatio;

    [Header("Leaderboard")]
    [SerializeField] private Transform _contentList;
    [SerializeField] private LeaderboardListing _leaderboardListing;

    [Header("Forgot Passowrd")]
    [SerializeField] private TMP_InputField _emailIFReset;

    [Header("Join Game By ID")]
    [SerializeField] private TMP_InputField _roomIDField;

    [Header("Join Teacher Game")]
    [SerializeField] private TMP_InputField _gameIDField;

    [Header("Others")]
    [SerializeField] private MainLoadingPanel _mainLoadingPanel;

    [Header("AudioSettings")]
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;

    //Private
   [SerializeField] private int _selectedCountryID;
    private GameObject _currentSelectedPanel;
    private float _masterVolume;
    private float _musicVolume;
    private int _selectedNumbOfPlayers = 2;


    private void Awake()
    {
        AuthManager.instance.SetMenuManagerComponent(this);

        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            _masterVolume = PlayerPrefs.GetFloat("MasterVolume");
            _masterVolumeSlider.value = _masterVolume;
            _audioMixer.SetFloat("MasterVolume", _musicVolume);
        }
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            _musicVolume = PlayerPrefs.GetFloat("MusicVolume");
            _musicVolumeSlider.value = _musicVolume;
            _audioMixer.SetFloat("MusicVolume", _musicVolume);
        }
    }

    private void Start()
    {
        if (AuthManager.instance.isLoggedIn)
        {
            ShowMainPanel(3);
        }
        else
        {
            ShowMainPanel(0);
            AuthManager.instance.GetCountry();
        }

        PhotonNetworkScript.instance.SetMainMenuManager(this);


    }

    public void IncreaseLoadingBar(int amount)
    {
        _mainLoadingPanel.IncreaseLoadingBar(amount);
    }

    public void SetMainLoadingScreen(bool state)
    {
        _mainLoadingPanel.gameObject.SetActive(state);
    }

    public void SetCountryDropDown()
    {
        foreach (var country in AuthManager.instance.countryData.content)
        {
            _countryDropDown.options.Add(new Dropdown.OptionData() { text = country.name });
        }
    }

    public void OnPressRegister()
    {
        AuthManager.instance.Register(_firstNameIF.text, _lastNameIF.text, _emailIF.text, _passwordIF.text, _passwordIF.text, _selectedCountryID);
    }

    public void OnPressLogin()
    {
        AuthManager.instance.Login(_emailIFLogin.text, _passwordIFLogin.text);
    }

    public void OnPressForgotPassword()
    {
        AuthManager.instance.ForgotPassword(_emailIFReset.text);
    }

    public void BackToHome()
    {
        ClosePrevioucePanel();
    }

    public void RandomMatch()
    {
        ShowPanelByItsName("RandomMatch Panel");
    }

    public void HostGamePublic(bool isNormalGame)
    {
        PhotonNetworkScript.instance.HostPublic(isNormalGame);
    }

    public void HostGamePrivate(bool isNormalGame)
    {
        PhotonNetworkScript.instance.HostPrivate(isNormalGame);
    }

    public void JoinRoom()
    {
        string roomId = _roomIDField.text;

        if (roomId != null && roomId != "")
            PhotonNetworkScript.instance.JoinRoomByID(roomId);
        else Debug.LogError("Room ID Is Null");
    }

    public void StartSinglePlayerGame(bool isNormalGame)
    {
        PhotonNetworkScript.instance.StartOfflineGame(isNormalGame);
    }

    public void JoinTeacherRoom()
    {
        string roomId = _gameIDField.text;

        if (roomId != null && roomId != "")
            PhotonNetworkScript.instance.JoinRoomByID(roomId);
    }

    public void StartSerchingForRandomMatch(bool isNormal)
    {
        PhotonNetworkScript.instance.StartOnlineGame(isNormal, _selectedNumbOfPlayers);
        ShowPanelByItsName("SearchingForMatch Panel");
    }

    public void SetNumberOfPlayers(int numb)
    {
        _selectedNumbOfPlayers = numb;
    }

    public void ShowProfilePanel()
    {
        AuthManager.instance.GetUserData();
    }

    public void ShowSettingsPanel()
    {
        AuthManager.instance.GetUserData();
    }

    public void SetUpProfile()
    {
        _firstName.text = AuthManager.instance.userData.content.user.first_name;
        _lastName.text = AuthManager.instance.userData.content.user.last_name;
        _gameTime.text = AuthManager.instance.userData.content.user.game_time;
        _score.text = "Score: " + AuthManager.instance.userData.content.user.score;
        _wallet.text ="Wallet: " + AuthManager.instance.userData.content.user.wallet;
        _totalQuestions.text = "Total Questions: " + AuthManager.instance.userData.content.user.total_questions;
        _trueAnswers.text ="True Answers: "+ AuthManager.instance.userData.content.user.answer_true;
        _falseAnswer.text = "Wrong Answers: " + AuthManager.instance.userData.content.user.answer_false;
        _winRatio.text = "Win Ratio: " + AuthManager.instance.userData.content.user.win_ratio;
    }


    public void ShowLeaderboardPanel()
    {
        AuthManager.instance.GetLeaderboard(100, 1, "", "", "");
    }

    public void SetupLeaderboard()
    {
        foreach (Transform child in _contentList)
        {
            if (child != null) Destroy(child.gameObject);
        }

        foreach (var player in AuthManager.instance.leaderBoardData.content.players)
        {
           var listing = Instantiate(_leaderboardListing, _contentList);
            listing.GetComponent<LeaderboardListing>().SetInfo(player);
        }
    }

    public void ShowPanelByItsName(string name)
    {
        ClosePrevioucePanel();
        _currentSelectedPanel = FindPanelInPanelList(name); ;
        OpenCurrentSelectedPanel();
    }



    private void ClosePrevioucePanel()
    {
        if (_currentSelectedPanel != null)
        {
            _currentSelectedPanel.SetActive(false);
        }
    }

    private void OpenCurrentSelectedPanel()
    {
        _currentSelectedPanel.SetActive(true);
    }

    public void ShowMainPanel(int index)
    {
        foreach (var panel in _mainPanels)
        {
            panel.SetActive(false);
        }

        _mainPanels[index].SetActive(true);
    }

    public void ExitMatchMaking()
    {
        ClosePrevioucePanel();
        PhotonNetworkScript.instance.ExitMatchMaking();
    }

    public void ExitCurrentRoom()
    {
        ClosePrevioucePanel();
        PhotonNetworkScript.instance.ExitCurrentRoom();
        LoadingScript.instance.StartLoading();
    }

    public void Logout()
    {
        AuthManager.instance.DeleteSavedCredential();
        PhotonNetworkScript.instance.OnPlayerLogout();
        Destroy(PhotonNetworkScript.instance.gameObject);
        Destroy(AuthManager.instance.gameObject);
        Destroy(LoadingScript.instance.gameObject);
        Destroy(ErrorScript.instance.gameObject);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private GameObject FindPanelInPanelList(string name)
    {
        foreach (var item in _panels)
        {
            if (item.name == name) return item;
        }

        return null;
    }

    public void OnDropdownValueChange()
    {
        _selectedCountryID = _countryDropDown.value + 1;
    }

    public void OnVolumeChangeMaster()
    {
        _masterVolume = _masterVolumeSlider.value;
        PlayerPrefs.SetFloat("MasterVolume", _masterVolume);
        _audioMixer.SetFloat("MasterVolume", _musicVolume);
    }

    public void OnVolumeChangeMusic()
    {
        _musicVolume = _musicVolumeSlider.value;
        PlayerPrefs.SetFloat("MusicVolume", _musicVolume);
        _audioMixer.SetFloat("MusicVolume", _musicVolume);
    }
}
