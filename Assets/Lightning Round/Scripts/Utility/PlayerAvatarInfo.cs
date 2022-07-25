using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerAvatarInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerName;
    [SerializeField] private TextMeshProUGUI _playerScore;
    [SerializeField] private Image _playerImage;

    public void SetPlayerNameAndImage(string name, Sprite image)
    {
        _playerName.text = name;
        _playerImage.sprite = image;
    }

    public void UpdatePlayerScore(int score)
    {
        _playerScore.text = score + " pts";
    }
}
