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
    [SerializeField] private MainLoadingPanel _mainLoadingPanel;

    //PR
    private int _randomCountForTryingToFindARoom;
    private IEnumerator _searchForRoomCoroutine;
    private bool _isNormalGame;

    //PB
    private MenuManager _menuManager;



    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(this);

  
    }

    // Start is called before the first frame update
    void Start()
    {
        _mainLoadingPanel.gameObject.SetActive(true);
        Photon.Pun.PhotonNetwork.ConnectUsingSettings();
        Photon.Pun.PhotonNetwork.NickName = Random.Range(1,9999).ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetMainMenuManager(MenuManager component)
    {
        _menuManager = component;
    }


    #region CallBacks


    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN.");
        Photon.Pun.PhotonNetwork.AutomaticallySyncScene = true;
        Photon.Pun.PhotonNetwork.JoinLobby();
        if (_mainLoadingPanel != null)
            _mainLoadingPanel.IncreaseLoadingBar(50);

        if (LoadingScript.instance.isActivated) LoadingScript.instance.StopLoading();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("in Lobby");
        if (_mainLoadingPanel != null)
            _mainLoadingPanel.IncreaseLoadingBar(50);
        if (LoadingScript.instance.isActivated) LoadingScript.instance.StopLoading();
    }

    public override void OnJoinedRoom()
    {
        _randomCountForTryingToFindARoom = 0;
        if (_searchForRoomCoroutine != null) StopCoroutine(_searchForRoomCoroutine);
        if (Photon.Pun.PhotonNetwork.OfflineMode == true)
        {
            Photon.Pun.PhotonNetwork.LoadLevel(1);
        }

        StopAllCoroutines();

        Debug.Log("IN Room");

        _menuManager.ShowPanelByItsName("Room Panel");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //  Debug.LogError(returnCode + " " + message);
      //  ErrorScript.instance.StartErrorMsg(message, "MatchFailed");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(Photon.Pun.PhotonNetwork.CurrentRoom.PlayerCount == _maxPlayersInRoom)
        {
            Photon.Pun.PhotonNetwork.LoadLevel(1);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
    }

    public override void OnCreatedRoom()
    {
        if (Photon.Pun.PhotonNetwork.OfflineMode == true) Photon.Pun.PhotonNetwork.LoadLevel(1);
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        bool isOffline = Photon.Pun.PhotonNetwork.OfflineMode;
        if (!isOffline) Photon.Pun.PhotonNetwork.OfflineMode = true;
        //CreateOfflineRoom();

        ErrorScript.instance.StartErrorMsg(cause.ToString(), "Photon");
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.JoinLobby();
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
        ErrorScript.instance.StartErrorMsg(message, "MatchFailed");
    }

    #endregion CallBacks


    public void StartOnlineGame(bool isNormal)
    {
        //bool isOffline = Photon.Pun.PhotonNetwork.OfflineMode;
        //if (isOffline) Photon.Pun.PhotonNetwork.OfflineMode = false;

        // _randomCountForTryingToFindARoom = Random.Range(7,25);
        _isNormalGame = isNormal;
        _randomCountForTryingToFindARoom = 3;
        StartCoroutine(SearchForAvailbleRoomToJoin());
        _searchForRoomCoroutine = SearchForAvailbleRoomToJoin();
    }

    //public void StartOfflineGame()
    //{
    //    Photon.Pun.PhotonNetwork.Disconnect();
    //}

    private IEnumerator SearchForAvailbleRoomToJoin()
    {
        _randomCountForTryingToFindARoom--;
        yield return new WaitForSeconds(1.5f);
        if (_randomCountForTryingToFindARoom > 0 && !PhotonNetwork.InRoom)
        {
            JoinRandomMatch(_isNormalGame);
            StartCoroutine(SearchForAvailbleRoomToJoin());
        }
        else
        {
            CreateOnlineRoomAfterFailToJoinOne();
        }
        Debug.Log(_randomCountForTryingToFindARoom.ToString());
    }

    private void JoinRandomMatch(bool isNormal)
    {
        string sqlLobbyFilter;

        if (isNormal)
            sqlLobbyFilter = "gm = 'true'";
        else sqlLobbyFilter = "gm = 'false'";

        Hashtable roomProperties = new Hashtable() { { "gm", isNormal } };
        PhotonNetwork.JoinRandomRoom(roomProperties, 0);
    }

    private void CreateOnlineRoomAfterFailToJoinOne()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = _maxPlayersInRoom;
        roomOptions.CleanupCacheOnLeave = false;

        Hashtable roomProperties = new Hashtable() { { "gm", _isNormalGame } };
        string[] lobbyProperties = { "gm" };

        roomOptions.CustomRoomPropertiesForLobby = lobbyProperties;
        roomOptions.CustomRoomProperties = roomProperties;

        Photon.Pun.PhotonNetwork.JoinOrCreateRoom(Photon.Pun.PhotonNetwork.NickName+Random.Range(0,999), roomOptions , TypedLobby.Default);
    }

    public void HostGame(bool isPublic, bool isNormal)
    {
       
        RoomOptions roomOptions = new RoomOptions();
        if (isPublic)
            roomOptions.IsVisible = true;
        else roomOptions.IsVisible = false;

        Hashtable roomProperties = new Hashtable() { { "gm", isNormal } };
        string[] lobbyProperties = { "gm" };

        roomOptions.CustomRoomPropertiesForLobby = lobbyProperties;
        roomOptions.CustomRoomProperties = roomProperties;

        roomOptions.MaxPlayers = _maxPlayersInRoom;
        Photon.Pun.PhotonNetwork.JoinOrCreateRoom(Photon.Pun.PhotonNetwork.NickName + Random.Range(0, 999), roomOptions, TypedLobby.Default);
    }

    public void JoinRoomByID(string ID)
    {
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

    //private void CreateOfflineRoom()
    //{
    //    RoomOptions roomOptions = new RoomOptions();
    //    roomOptions.IsVisible = false;
    //    roomOptions.MaxPlayers = 1;
    //    Photon.Pun.PhotonNetwork.CreateRoom(Photon.Pun.PhotonNetwork.NickName, roomOptions, TypedLobby.Default);
    //}



}
