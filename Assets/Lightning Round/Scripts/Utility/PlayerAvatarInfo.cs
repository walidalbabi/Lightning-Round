using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class PlayerAvatarInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerName;
    [SerializeField] private TextMeshProUGUI _playerScore;
    [SerializeField] private Image _playerImage;
    [SerializeField] private Image _playerTimerImage;
    public float _timer;
    public float _currentMaxTime;
    public double _serverTime;


    private PhotonView _pv;



    private void Awake()
    {
        _pv = GetComponent<PhotonView>();
    }

    private void Update()
    {
      //  UpdateTime();
    }

    public void SetPlayerNameAndImage(string name, Sprite image)
    {
        _playerName.text = name;
        _playerImage.sprite = image;
    }

    public void UpdatePlayerScore(int score)
    {
        _playerScore.text = score + " pts";
    }

    public void UpdateTimer(float timer, double serverTime, float currentMaxTime)
    {
        _timer = timer;
        _serverTime = serverTime;
        _currentMaxTime = currentMaxTime;
    }

    private void UpdateTime()
    {
        if (_timer <= 0) return;
        _timer = GameManager.instance.currentMaxTime - ((float)(PhotonNetwork.Time % _serverTime));
        _timer = _timer / _currentMaxTime;
        _playerTimerImage.fillAmount = _timer;
    }
}
