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
        if(Photon.Pun.PhotonNetwork.CurrentRoom.PlayerCount == _maxPlayersInRoom)
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

    public void StartOnlineGame(bool isNormal)
    {
        _isNormalGame = isNormal;
        _randomCountForTryingToFindARoom = 3;
        StartCoroutine(SearchForAvailbleRoomToJoin());
        _searchForRoomCoroutine = SearchForAvailbleRoomToJoin();
    }


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

        if(_isNormalGame) AuthManager.instance.GetMatch("SLOW","", true);
        else if(!_isNormalGame) AuthManager.instance.GetMatch("FAST","", true);
    }

    public void HostGame(bool isPublic, bool isNormal)
    {
        RoomOptions roomOptions = new RoomOptions();
        if (isPublic)
            roomOptions.IsVisible = true;
        else roomOptions.IsVisible = false;
        roomOptions.MaxPlayers = _maxPlayersInRoom;
        roomOptions.CleanupCacheOnLeave = false;

        Hashtable roomProperties = new Hashtable() { { "gm", isNormal } };
        string[] lobbyProperties = { "gm" };

        roomOptions.CustomRoomPropertiesForLobby = lobbyProperties;
        roomOptions.CustomRoomProperties = roomProperties;

        Photon.Pun.PhotonNetwork.JoinOrCreateRoom(AuthManager.instance.matchData.content.match.room_code, roomOptions, TypedLobby.Default);
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
