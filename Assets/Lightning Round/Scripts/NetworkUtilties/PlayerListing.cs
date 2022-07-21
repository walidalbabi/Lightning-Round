using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;


public class PlayerListing : MonoBehaviour
{
    //Inspector Assign
    [SerializeField] TextMeshProUGUI _playerName;
    [SerializeField] Image _avatarImage;

    private Sprite _playerImg;

    //PB
    public Player Player { get; private set; }

    public void SetInfo(Player player)
    {
        Player = player;
        _playerName.text = player.NickName;
        _avatarImage.sprite = _playerImg;
    }
}
