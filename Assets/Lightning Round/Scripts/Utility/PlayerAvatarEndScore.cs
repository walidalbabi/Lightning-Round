using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerAvatarEndScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private TextMeshProUGUI _playerScoreText;
    [SerializeField] private Image _playerImage;
    [SerializeField] private Image _medalImage;
    [SerializeField] private Sprite[] _medals;

    private string _playerName;
    private int _score;
    private Sprite _playerImgSprite;
 

    public void SetAvatarInfo(string name, int score, Sprite image, int place)
    {
        //Remove If Not Used
        _playerName = name;
        _score = score;
        _playerImgSprite = image;
        //

        _playerNameText.text = name;
        _playerScoreText.text = score.ToString();
        _playerImage.sprite = image;

        _medalImage.sprite = _medals[place];
    }

}
