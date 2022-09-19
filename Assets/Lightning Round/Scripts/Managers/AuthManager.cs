using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AuthManager : MonoBehaviour
{
    public static AuthManager instance;

    public string TOKEN;

    public LoginData logData;
    public RegisterData registerData;
    public ErrorDescriptionOnly errorDescriptionMessage;
    public CountryData countryData;
    public UserContent userData;
    public MatchData matchData;
    public LeaderboardData leaderBoardData;

    [SerializeField] private string loginURL; 
    [SerializeField] private string registerURL; 
    [SerializeField] private string countryURL;
    [SerializeField] private string forgotPasswordURL;
    [SerializeField] private string getMatchURL;
    [SerializeField] private string postAsnwersURL;
    [SerializeField] private string getUserDataURL;
    [SerializeField] private string getLeaderboardURL;


    public bool isLoggedIn;

    private string errorText;
    private MenuManager _menuManager;

    private string userEmail;
    private string userPassword;


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        if (PlayerPrefs.HasKey("userEmail")) userEmail = PlayerPrefs.GetString("userEmail");
        if (PlayerPrefs.HasKey("userPassword")) userPassword = PlayerPrefs.GetString("userPassword");
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("userEmail") && PlayerPrefs.HasKey("userPassword"))
            Login(userEmail, userPassword);
    }

    public void DeleteSavedCredential()
    {
        if (PlayerPrefs.HasKey("userEmail")) PlayerPrefs.DeleteKey("userEmail");
        if (PlayerPrefs.HasKey("userPassword")) PlayerPrefs.DeleteKey("userPassword");
    }

    #region Register

    public void Register(string firstName, string lastName, string email , string password, string ConfirmationPassword, int countryID )
    {
        var country = countryID == 0 ? 1 : countryID;
        _menuManager.SetMainLoadingScreen(true);
        errorDescriptionMessage = new ErrorDescriptionOnly();
        registerData = new RegisterData();
        errorText = "";
        StartCoroutine(CallRegister(firstName, lastName, email, password, ConfirmationPassword, country.ToString()));
    }

    IEnumerator CallRegister(string firstName, string lastName, string email, string password, string ConfirmationPassword, string countryID)
    {
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("first_name", firstName);
        wwwForm.AddField("last_name", lastName);
        wwwForm.AddField("email", email);
        wwwForm.AddField("password", password);
        wwwForm.AddField("password_confirmation", ConfirmationPassword);
        wwwForm.AddField("country_id", countryID);


        UnityWebRequest www = UnityWebRequest.Post(registerURL, wwwForm);


        www.SetRequestHeader("Accept", "application/json");

        yield return www.SendWebRequest();

        if (www.error != null)
        {
            errorText = "";
            var text = www.downloadHandler.text;
            ProccessErrorMessageRegister(text);

            if (registerData.error_des.email.Count > 0)
                foreach (var item in registerData.error_des.email)
                {
                    errorText += item + "";
                }


            if (registerData.error_des.password.Count > 0)
                foreach (var item in registerData.error_des.password)
                {
                    errorText += item + "";
                }

            if (errorText != "")
                ErrorScript.instance.StartErrorMsg(errorText, "");
            else if (errorDescriptionMessage.error_des != "")
            {
                ErrorScript.instance.StartErrorMsg(errorDescriptionMessage.error_des, "");
            }
            else ErrorScript.instance.StartErrorMsg("Invalid Data", "");

            _menuManager.SetMainLoadingScreen(false);
        }
        else
        {
            var text = www.downloadHandler.text;
            ProcessSuccessLogin_Register(text);
            PlayerPrefs.SetString("userEmail", email);
            PlayerPrefs.SetString("userPassword", password);
            _menuManager.ShowMainPanel(3);
            isLoggedIn = true;
        }
    }

    private void ProccessErrorMessageRegister(string text)
    {
        RegisterData jasonDataClass = JsonUtility.FromJson<RegisterData>(text);
        ErrorDescriptionOnly jasonDataClass_1 = JsonUtility.FromJson<ErrorDescriptionOnly>(text);
        registerData = jasonDataClass;
        errorDescriptionMessage = jasonDataClass_1;
    }

    #endregion Register

    #region Login

    public void Login( string email, string password)
    {
        _menuManager.SetMainLoadingScreen(true);
        errorDescriptionMessage = new ErrorDescriptionOnly();
        logData = new LoginData();
        errorText = "";
        StartCoroutine(CallLogin(email, password));
    }

    IEnumerator CallLogin(string Email, string Password)
    {
        byte[] payload = new byte[1024];

        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("email", Email);
        wwwForm.AddField("password", Password);

        UnityWebRequest www = UnityWebRequest.Post(loginURL, wwwForm);
        www.SetRequestHeader("Accept" , "application/json");

        yield return www.SendWebRequest();

        if(www.error != null)
        {
            errorText = "";
            var text = www.downloadHandler.text;
            ProcessErrorMessageLogin(text);

            if(logData.error_des.email.Count > 0)
                foreach (var item in logData.error_des.email)
                {
                    errorText += item + "";
                }


            if (logData.error_des.password.Count > 0)
                foreach (var item in logData.error_des.password)
                {
                    errorText += item + "";
                }
            errorText += logData.error_des.message + "";

            if (errorText != "")
                ErrorScript.instance.StartErrorMsg(errorText, "");
            else if (errorDescriptionMessage.error_des != "") {
                ErrorScript.instance.StartErrorMsg(errorDescriptionMessage.error_des, "");
            }
            else ErrorScript.instance.StartErrorMsg("Invalid Data", "");

            _menuManager.SetMainLoadingScreen(false);
            PlayerPrefs.DeleteKey("userEmail");
            PlayerPrefs.DeleteKey("userPassword");
        }
        else
        {
            var text = www.downloadHandler.text;
            ProcessSuccessLogin_Register(text);
            _menuManager.ShowMainPanel(3);
            PlayerPrefs.SetString("userEmail",Email);
            PlayerPrefs.SetString("userPassword", Password);
            isLoggedIn = true;
        }

    }

    private void ProcessErrorMessageLogin(string text)
    {
        LoginData jasonDataClass = JsonUtility.FromJson<LoginData>(text);
        ErrorDescriptionOnly jasonDataClass_1 = JsonUtility.FromJson<ErrorDescriptionOnly>(text);
        logData = jasonDataClass;
        errorDescriptionMessage = jasonDataClass_1;
    }

    #endregion Login

    #region GetCountries

    public void GetCountry()
    {
        StartCoroutine(GetCountryData());
    }

    private IEnumerator GetCountryData()
    {
        UnityWebRequest request = UnityWebRequest.Get(countryURL);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.DataProcessingError)
        {
            Debug.LogError(request.error);
            GetCountry();
        }
        else
        {
            var text = request.downloadHandler.text;
            Debug.Log(text);
            processJasonDataCountry(text);
            _menuManager.SetCountryDropDown();
        }
    }

    private void processJasonDataCountry(string text)
    {
        CountryData jasonDataClass = JsonUtility.FromJson<CountryData>(text);
        countryData = jasonDataClass;
    }

    #endregion GetCountries

    #region FogotPassword

    public void ForgotPassword(string Email)
    {
        StartCoroutine(GetForgotPasswordResult(Email));
    }

    private IEnumerator GetForgotPasswordResult(string Email)
    {
        var ForgotPWFullURL = forgotPasswordURL + Email;
        UnityWebRequest request = UnityWebRequest.Get(ForgotPWFullURL);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.DataProcessingError)
        {
            Debug.LogError(request.error);
            var text = request.downloadHandler.text;
            processJasonDataForgotPassowrd(text);

            if (errorDescriptionMessage.error_des != "") ErrorScript.instance.StartErrorMsg(errorDescriptionMessage.error_des, "");
            else ErrorScript.instance.StartErrorMsg("Invalid Email", "");
        }
        else
        {
            var text = request.downloadHandler.text;
            Debug.Log(text);
        }
    }

    private void processJasonDataForgotPassowrd(string text)
    {
        ErrorDescriptionOnly jasonDataClass = JsonUtility.FromJson<ErrorDescriptionOnly>(text);
        errorDescriptionMessage = jasonDataClass;
    }


    #endregion FogotPassword

    #region SaveAnswers

    public void SendAnswers(string data)
    {
        StartCoroutine(CallSendAnswers(data));
    }

    IEnumerator CallSendAnswers(string data)
    {
        byte[] payload = new byte[1024];

        UnityWebRequest www = UnityWebRequest.Post(postAsnwersURL,"POST");
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Accept", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + TOKEN);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(data);
        www.uploadHandler = new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www.error != null)
        {
            errorText = "";
            var text = www.downloadHandler.text;
            if (text != null)
                ErrorScript.instance.StartErrorMsg(text, "");
            else ErrorScript.instance.StartErrorMsg("Failed To Update Data", "");
            Debug.LogError(www.error + text);
           
        }
        else
        {
            Debug.Log("Data sent Successful");
        }

    }
    #endregion SaveAnswers

    #region GetMatch

    public void GetMatch(string mode_type, string room_code, bool isPublic)
    {
        matchData = new MatchData();
        StartCoroutine(CallGetMatch(mode_type, room_code, isPublic));
    }

    IEnumerator CallGetMatch(string mode_type, string room_code, bool isPublic)
    {
        UnityWebRequest www;
        if (room_code == "")
            www = UnityWebRequest.Get(getMatchURL + "?mode_type=" + mode_type);
        else www = UnityWebRequest.Get(getMatchURL + "?mode_type=" + mode_type + "&room_code=" + room_code);

        www.SetRequestHeader("Accept", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + TOKEN);

        yield return www.SendWebRequest();

        LoadingScript.instance.StopLoading();

        if (www.error != null)
        {
            Debug.LogError("Failed To Get Match");
            var text = www.downloadHandler.text;
            Debug.Log(text);
            ErrorScript.instance.StartErrorMsg("Match isn't availble","");
        }
        else
        {
            Debug.Log("Login Successful");
            var text = www.downloadHandler.text;
            ProcessSuccessfulGetMatch(text);
            PhotonNetworkScript.instance.HostGame(isPublic, matchData.content.match.mode_type == "FAST" ? false : true);
            Debug.Log(text);
        }

    }

    private void ProcessSuccessfulGetMatch(string text)
    {
        MatchData jasonDataClass = JsonUtility.FromJson<MatchData>(text);
        matchData = jasonDataClass;
    }


    #endregion GetMatch

    #region GetUserData


    public void GetUserData()
    {
        LoadingScript.instance.StartLoading();
        StartCoroutine(CallGetGetUserData());
    }

    IEnumerator CallGetGetUserData()
    {
        UnityWebRequest www;

        www = UnityWebRequest.Get(getUserDataURL);

        www.SetRequestHeader("Accept", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + TOKEN);

        yield return www.SendWebRequest();

        LoadingScript.instance.StopLoading();

        if (www.error != null)
        {
            var text = www.downloadHandler.text;
            Debug.LogError(text);
            ErrorScript.instance.StartErrorMsg(text, "");
        }
        else
        {
            Debug.Log("Login Successful");
            var text = www.downloadHandler.text;
            ProcessSuccessfulGetUsedData(text);
            _menuManager.ShowPanelByItsName("Profile Panel");
            _menuManager.SetUpProfile();
            Debug.Log(text);
        }

    }

    private void ProcessSuccessfulGetUsedData(string text)
    {
        UserContent jasonDataClass = JsonUtility.FromJson<UserContent>(text);
        userData = jasonDataClass; ;
    }

    #endregion GetUserData

    #region GetLeadeboard

    public void GetLeaderboard(int limit, int page, string id , string name, string status)
    {
        LoadingScript.instance.StartLoading();
        StartCoroutine(CallGetLeaderboard(limit, page , id, name, status));
    }

    IEnumerator CallGetLeaderboard(int limit, int page, string id, string name, string status)
    {
        UnityWebRequest www;
        www = UnityWebRequest.Get(getLeaderboardURL + "?limit=" + limit.ToString() + "&page=" + page.ToString() + "&id=" + id 
            + "&name=" + name + "&status=" + status);

        www.SetRequestHeader("Accept", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + TOKEN);

        yield return www.SendWebRequest();

        LoadingScript.instance.StopLoading();

        if (www.error != null)
        {
            var text = www.downloadHandler.text;
            Debug.LogError(text);
            ErrorScript.instance.StartErrorMsg("Failed To Get Leaderboard", "");
        }
        else
        {
            Debug.Log("Login Successful");
            var text = www.downloadHandler.text;
            ProcessSuccessfulGetLeaderboard(text);
            _menuManager.ShowPanelByItsName("Leaderboard Panel");
            _menuManager.SetupLeaderboard();
        }

    }

    private void ProcessSuccessfulGetLeaderboard(string text)
    {
        LeaderboardData jasonDataClass = JsonUtility.FromJson<LeaderboardData>(text);
        leaderBoardData = jasonDataClass;
    }


    #endregion GetLeadeboard

    private void ProcessSuccessLogin_Register(string text)
    {
        UserContent jasonDataClass = JsonUtility.FromJson<UserContent>(text);
        userData = jasonDataClass;
        PhotonNetworkScript.instance.StartPhotonAuth();
        _menuManager.IncreaseLoadingBar(60);
        TOKEN = userData.content.token;
    }

    public void SetMenuManagerComponent(MenuManager component)
    {
        _menuManager = component;
    }
}
