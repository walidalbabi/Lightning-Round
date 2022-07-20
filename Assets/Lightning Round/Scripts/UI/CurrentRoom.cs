using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class CurrentRoom : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update

    [SerializeField] private Transform _currentPlayersTable;
    [SerializeField] private PlayerListing _playerListing;
    [SerializeField] private TextMeshProUGUI _playerNumbText;

    private List<PlayerListing> _listings = new List<PlayerListing>();

    public override void OnEnable()
    {
        GetCurrentRoomPlayer();
    }

    public override void OnDisable()
    {
        for (int i = 0; i < _listings.Count; i++)
            Destroy(_listings[i].gameObject);

        _listings.Clear();

    }


    private void GetCurrentRoomPlayer()
    {
        if (!PhotonNetwork.IsConnected)
            return;
        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
            return;

        foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
        {
            AddPlayerListing(playerInfo.Value);
        }
    }

    private void AddPlayerListing(Player player)
    {
        int index = _listings.FindIndex(x => x.Player == player);

        if (index != -1)
        {
            _listings[index].SetInfo(player);
        }
        else
        {
            PlayerListing listing = Instantiate(_playerListing, _currentPlayersTable);

            if (listing != null)
            {
                listing.SetInfo(player);
                _listings.Add(listing);
            }
        }

        _playerNumbText.text = PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerListing(newPlayer);
        _playerNumbText.text = PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int index = _listings.FindIndex(x => x.Player == otherPlayer);

        if (index != -1)
        {
            Destroy(_listings[index].gameObject);
            _listings.RemoveAt(index);
        }
        _playerNumbText.text = PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

}
