using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardListing : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _score;

    public void SetInfo(LeaderboardPlayers player)
    {
        _name.text = player.name;
        _score.text = player.score + " pts";
    }
}
