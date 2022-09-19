using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region AuthData

[System.Serializable]
public class LoginData
{
    public string result;
    public string content;
    public string email;
    public ErrorDesc error_des;
    public string error_validation;
    public string error_code;
    public string date;
}

[System.Serializable]
public class ErrorDescriptionOnly
{
    public string error_des;
}

[System.Serializable]
public class RegisterData
{
    public string result;
    public string content;
    public string email;
    public ErrorDesc error_des;
    public string error_validation;
    public string error_code;
    public string date;

}

[System.Serializable]
public class ErrorDesc
{
    public List<string> email;
    public List<string> password;
    public string message;
}

[System.Serializable]
public class CountryData
{
    public List<CountryContent> content;
}

[System.Serializable]
public class CountryContent
{
    public string id;
    public string code;
    public string name;
}

[System.Serializable]
public class UserContent
{
    public string result;
    public UserData content;
}

[System.Serializable]
public class UserData
{
    public string token;
    public User user;
}


[System.Serializable]
public class User 
{
    public string id;
    public string userId;
    public string first_name;
    public string last_name;
    public string email;
    public string phone_number;
    public string avatar;
    public string post_code;
    public string country_id;
    public string game_time;
    public string firebase_token;
    public string mobile_verified_at;
    public string reset_token;
    public string score;
    public string wallet;
    public string reset_verified;
    public string app_notification_status;
    public string location;
    public string name;
    public string answer_true;
    public string answer_false;
    public string answer_non;
    public string total_questions;
    public string win_ratio;
}

#endregion AuthData

#region UserMatchData

[System.Serializable]
public class MatchSaveData
{
    private string match_id;
    private string match_game_time;
    public List<UsersMatchData> users;
}
[System.Serializable]
public class UsersMatchData
{
    public string user_id;
    public string score;
    public List<QuetionsUserAnswers> questions;
}
[System.Serializable]
public class QuetionsUserAnswers
{
    public string question_id;
    public string answer_id;
    public string level_id;
    public string status_answer;
}

#endregion UserMatchData

#region MatchData

[System.Serializable]
public class MatchData
{
    public Data content;

}
[System.Serializable]
public class Data
{
    public Match match;

}
[System.Serializable]
public class Match
{
    public string mode_type;
    public string room_code;
    public List<Questions> questions;
}

[System.Serializable]
public class Questions
{
    public string score;
    public string title;
    public string teacher;
    public string correct_answer;
    public Subcategory subcategory;
    public ParentCategory parent_category;
    public List<Answers> answers;
}

[System.Serializable]
public class Subcategory
{
    public string name;
}

[System.Serializable]
public class ParentCategory
{
    public string name;
}


[System.Serializable]
public class Answers
{
    public string id;
    public string question_id;
    public string name;
}


#endregion MatchData

#region Match Answers

[System.Serializable]
public class AnswersData
{
    public string match_id;
    public string match_game_time;
    public AnswersUsers user;

}

[System.Serializable]
public class AnswersUsers
{
    public string user_id;
    public string score;
    public List<AnswersQuestions> questions;
}


[System.Serializable]
public class AnswersQuestions
{
    public string question_id;
    public string answer_id;
    public string status_answer;
}

#endregion Match Answers

#region Leaderboard

[System.Serializable]
public class LeaderboardData
{
    public string result;
    public LeaderboardContent content;
}

[System.Serializable]
public class LeaderboardContent
{
    public List<LeaderboardPlayers> players;
}

[System.Serializable]
public class LeaderboardPlayers
{
    public string id;
    public string name;
    public string score;
}


#endregion Leaderboard