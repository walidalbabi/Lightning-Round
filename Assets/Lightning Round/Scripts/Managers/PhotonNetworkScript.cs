using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class PhotonNetworkScript : MonoBehaviourPunCallbacks
{
    public static PhotonNetworkScript instance;


    //Var's
    //Inspector Assign
    [SerializeField] private byte _maxPlayersInRoom;

    //PR
    private int _randomCountForTryingToFindARoom;
    private IEnumerator _searchForRoomCoroutine;
    private bool _isNormalGame;
    private MenuManager _menuManager;
    private string _teacherRoomCode;
    //PB
    public bool isNormalGame => _isNormalGame;



    public byte maxPlayersInRoom => _maxPlayersInRoom;


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(this);

  
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartPhotonAuth()
    {
        Photon.Pun.PhotonNetwork.ConnectUsingSettings();
        Photon.Pun.PhotonNetwork.NickName = AuthManager.instance.userData.content.user.name;
    }

    public void SetMainMenuManager(MenuManager component)
    {
        _menuManager = component;
    }

    public void OnPlayerLogout()
    {
        PhotonNetwork.Disconnect();
    }


    #region CallBacks


    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN.");

        _menuManager.IncreaseLoadingBar(20);

        if (LoadingScript.instance.isActivated) LoadingScript.instance.StopLoading();

        Photon.Pun.PhotonNetwork.AutomaticallySyncScene = true;
        Photon.Pun.PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        _menuManager.IncreaseLoadingBar(20);
        if (LoadingScript.instance.isActivated) LoadingScript.instance.StopLoading();
    }

    public override void OnJoinedRoom()
    {
        _randomCountForTryingToFindARoom = 0;
        if (_searchForRoomCoroutine != null) StopCoroutine(_searchForRoomCoroutine);

        StopAllCoroutines();

        Debug.Log("IN Room");

        _menuManager.ShowPanelByItsName("Room Panel");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.OfflineMode == false)
        {
            if (Photon.Pun.PhotonNetwork.CurrentRoom.PlayerCount == _maxPlayersInRoom)
            {
                Photon.Pun.PhotonNetwork.LoadLevel(1);
            }
        }
        else if (PhotonNetwork.OfflineMode == true)
        {
            Photon.Pun.PhotonNetwork.LoadLevel(1);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        if(otherPlayer.IsMasterClient && SceneManager.sceneCount == 0)
        {
            _menuManager.ExitCurrentRoom();
            ErrorScript.instance.StartErrorMsg("Host Left The Game , Please try Again", "");
        }
    }

    public override void OnCreatedRoom()
    {
        if (Photon.Pun.PhotonNetwork.OfflineMode == true) Photon.Pun.PhotonNetwork.LoadLevel(1);
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        if (cause != DisconnectCause.DisconnectByClientLogic)
            ErrorScript.instance.StartErrorMsg(cause.ToString(), "Photon");
        else
        {
            //Offline Mode
            bool isOffline = PhotonNetwork.OfflineMode;
            if (!isOffline) PhotonNetwork.OfflineMode = true;
            if (_isNormalGame) AuthManager.instance.GetMatch("SLOW", "", true);
            else if (!_isNormalGame) AuthManager.instance.GetMatch("FAST", "", true);
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        if (PhotonNetwork.OfflineMode == false) PhotonNetwork.JoinLobby();
        else StartPhotonAuth();

        if(SceneManager.sceneCount == 1)
        {
            if (!LoadingScript.instance.isActivated) LoadingScript.instance.StartLoading();
            SceneManager.LoadScene(0);
        }
        Debug.Log("Left Room");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        LoadingScript.instance.StartLoading();
        AuthManager.instance.GetMatch("FAST", _teacherRoomCode, false);
    }

    #endregion CallBacks

    public void HostPrivate(bool isNormal)
    {
        if (isNormal) AuthManager.instance.GetMatch("SLOW", "", false);
        else if (!isNormal) AuthManager.instance.GetMatch("FAST", "", false);
    }

    public void HostPublic(bool isNormal)
    {
        _isNormalGame = isNormal;
        CreateOnlineRoomAfterFailToJoinOne();
    }

    public void StartOnlineGame(bool isNormal, int numberOfPlayers)
    {
        _maxPlayersInRoom = (byte)numberOfPlayers;
        _isNormalGame = isNormal;
        _randomCountForTryingToFindARoom = 3;
        StartCoroutine(SearchForAvailbleRoomToJoin());
        _searchForRoomCoroutine = SearchForAvailbleRoomToJoin();
    }

    public void StartOfflineGame(bool isNormal)
    {
        PhotonNetwork.Disconnect();
        _isNormalGame = isNormal;
    }


    private IEnumerator SearchForAvailbleRoomToJoin()
    {
        _randomCountForTryingToFindARoom--;
        yield return new WaitForSeconds(1.5f);
        if (_randomCountForTryingToFindARoom > 0 && !PhotonNetwork.InRoom)
        {
            JoinRandomMatch(_isNormalGame , _maxPlayersInRoom);
            StartCoroutine(SearchForAvailbleRoomToJoin());
        }
        else
        {
            CreateOnlineRoomAfterFailToJoinOne();
        }
        Debug.Log(_randomCountForTryingToFindARoom.ToString());
    }

    private void JoinRandomMatch(bool isNormal, int numberOfPlayers)
    {
        string sqlLobbyFilter;
        _maxPlayersInRoom = (byte)numberOfPlayers;
        //Simple Checker On Max Players, Incase it got overrited by offline mode
        _maxPlayersInRoom = _maxPlayersInRoom == 1 ? (byte)2 : _maxPlayersInRoom;

        if (isNormal)
            sqlLobbyFilter = "gm = 'true'";
        else sqlLobbyFilter = "gm = 'false'";

        Hashtable roomProperties = new Hashtable() { { "gm", isNormal }, { "pl", numberOfPlayers } };
        PhotonNetwork.JoinRandomRoom(roomProperties, 0);
    }

    private void CreateOnlineRoomAfterFailToJoinOne()
    {

        if(_isNormalGame) AuthManager.instance.GetMatch("SLOW","", true);
        else if(!_isNormalGame) AuthManager.instance.GetMatch("FAST","", true);
    }

    public void HostGame(bool isPublic, bool isNormal, int numberOfPlayers)
    {
        RoomOptions roomOptions = new RoomOptions();
        //Simple Checker On Max Players, Incase it got overrited by offline mode
        numberOfPlayers = numberOfPlayers == 1 ? (byte)2 : numberOfPlayers;
        _maxPlayersInRoom = PhotonNetwork.OfflineMode == false ? (byte)numberOfPlayers : (byte)1;

        if(PhotonNetwork.OfflineMode == false)
        {
            if (isPublic)
                roomOptions.IsVisible = true;
            else roomOptions.IsVisible = false;
        }
        else
        {
            roomOptions.IsVisible = false;
        }

        roomOptions.MaxPlayers = _maxPlayersInRoom;
        roomOptions.CleanupCacheOnLeave = PhotonNetwork.OfflineMode == false ? false : true;

        Hashtable roomProperties = new Hashtable() { { "gm", isNormal } };
        string[] lobbyProperties = { "gm" };

        roomOptions.CustomRoomPropertiesForLobby = lobbyProperties;
        roomOptions.CustomRoomProperties = roomProperties;

        Photon.Pun.PhotonNetwork.CreateRoom(AuthManager.instance.matchData.content.match.room_code, roomOptions, TypedLobby.Default);
    }

    public void JoinRoomByID(string ID)
    {
        _teacherRoomCode = ID;
        Photon.Pun.PhotonNetwork.JoinRoom(ID);
    }

    public void ExitMatchMaking()
    {
        _randomCountForTryingToFindARoom = 0;
        StopAllCoroutines();
    }

    public void ExitCurrentRoom()
    {
        PhotonNetwork.LeaveRoom();
    }


}
