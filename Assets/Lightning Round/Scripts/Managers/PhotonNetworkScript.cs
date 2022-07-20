using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

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
        _mainLoadingPanel.IncreaseLoadingBar(50);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("in Lobby");
        _mainLoadingPanel.IncreaseLoadingBar(50);
    }

    public override void OnJoinedRoom()
    {
        _randomCountForTryingToFindARoom = 0;
        if (_searchForRoomCoroutine != null) StopCoroutine(_searchForRoomCoroutine);
        if (Photon.Pun.PhotonNetwork.OfflineMode == true)
        {
            Photon.Pun.PhotonNetwork.LoadLevel(1);
        }

        Debug.Log("IN Room");

        _menuManager.ShowPanelByItsName("Room Panel");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
      //  Debug.LogError(returnCode + " " + message);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(Photon.Pun.PhotonNetwork.CurrentRoom.PlayerCount == _maxPlayersInRoom)
        {
            Photon.Pun.PhotonNetwork.LoadLevel(1);
        }
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


    }
    #endregion CallBacks


    public void StartOnlineGame()
    {
        bool isOffline = Photon.Pun.PhotonNetwork.OfflineMode;
        if (isOffline) Photon.Pun.PhotonNetwork.OfflineMode = false;

       // _randomCountForTryingToFindARoom = Random.Range(7,25);
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
            PhotonNetwork.JoinRandomRoom();
            StartCoroutine(SearchForAvailbleRoomToJoin());
        }
        else
        {
            CreateOnlineRoomAfterFailToJoinOne();
        }
        Debug.Log(_randomCountForTryingToFindARoom.ToString());
    }

    private void CreateOnlineRoomAfterFailToJoinOne()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = _maxPlayersInRoom;
        Photon.Pun.PhotonNetwork.JoinOrCreateRoom(Photon.Pun.PhotonNetwork.NickName+Random.Range(0,999), roomOptions , TypedLobby.Default);
    }

    public void HostGame(bool isPublic)
    {
       
        RoomOptions roomOptions = new RoomOptions();
        if (isPublic)
            roomOptions.IsVisible = true;
        else roomOptions.IsVisible = false;
       
        roomOptions.MaxPlayers = _maxPlayersInRoom;
        Photon.Pun.PhotonNetwork.JoinOrCreateRoom(Photon.Pun.PhotonNetwork.NickName + Random.Range(0, 999), roomOptions, TypedLobby.Default);
    }

    public void JoinRoomByID(string ID)
    {
        Photon.Pun.PhotonNetwork.JoinRoom(ID);
    }

    //private void CreateOfflineRoom()
    //{
    //    RoomOptions roomOptions = new RoomOptions();
    //    roomOptions.IsVisible = false;
    //    roomOptions.MaxPlayers = 1;
    //    Photon.Pun.PhotonNetwork.CreateRoom(Photon.Pun.PhotonNetwork.NickName, roomOptions, TypedLobby.Default);
    //}



}
