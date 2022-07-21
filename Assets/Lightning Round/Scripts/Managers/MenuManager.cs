using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    //Inspector Assign
    [SerializeField] private List<GameObject> _panels = new List<GameObject>();
    [SerializeField] private TMP_InputField _roomIDField;



    //Private
    private GameObject _currentSelectedPanel;


    private void Start()
    {
        PhotonNetworkScript.instance.SetMainMenuManager(this);
    }

    public void BackToHome()
    {
        ClosePrevioucePanel();
    }

    public void RandomMatch()
    {
        PhotonNetworkScript.instance.StartOnlineGame();
        ShowPanelByItsName("RandomMatchmaking Panel");
    }

    public void HostGame(bool isPublic)
    {
        PhotonNetworkScript.instance.HostGame(isPublic);
    }

    public void JoinRoom()
    {
        string roomId = _roomIDField.text;

        if (roomId != null)
            PhotonNetworkScript.instance.JoinRoomByID(roomId);
        else Debug.LogError("Room ID Is Null");
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

    public void ExitMatchMaking()
    {
        ClosePrevioucePanel();
        PhotonNetworkScript.instance.ExitMatchMaking();
    }

    public void ExitCurrentRoom()
    {
        ClosePrevioucePanel();
        PhotonNetworkScript.instance.ExitCurrentRoom();
    }

    private GameObject FindPanelInPanelList(string name)
    {
        foreach (var item in _panels)
        {
            if (item.name == name) return item;
        }

        return null;
    }
}
