using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class R_Login : MonoBehaviour
{
    public static R_Login instance;

    [SerializeField] InputField username, password, regName, regEmail, regPassword;
    [SerializeField] Text loginMessageText, regMessageText;
    [SerializeField] Button loginBtn, registerBtn;

    void Awake()
    {
        instance = this;
    }

    public void OnClickOnButton(string eventName)
    {
        //SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "login":
                LoginCheck();
                break;

            case "register":
                RegisterCheck();
                break;

            case "loginLink":
            case "registerLink":
                ClearTextFields();
                break;

            default:
#if ERROR_LOG
                Debug.LogError("unhdnled eventName found in Login = " + eventName);
#endif
                break;
        }

    }

    void LoginCheck()
    {
        /*if (string.IsNullOrEmpty(username.text))
        {
            loginMessageText.text = "Please enter username";
        }
        else if (string.IsNullOrEmpty(password.text))
        {
            loginMessageText.text = "Please enter password";
        }
        else*/
        {
            loginBtn.interactable = false;
            //string json = "{\"email\": \""+username.text+"\", \"password\": \""+password.text+ "\", \"role\": \"user\", \"username\": \"" + username.text + "\"}";

            string json = "{\"mobile\":\"" + "2222222222" + "\"," +
                               "\"password\":\"" + "Test@123" + "\"," +
                               "\"type\":\"" + "2" + "\"," +
                               "\"device_id\":\"" + "NA" + "\"," +
                               "\"os_version\":\"" + SystemInfo.operatingSystem + "\"," +
                               "\"mac_address\":\"" + "NA" + "\"," +
                               "\"app_version\":\"" + "0.0.1" + "\"," +
                               "\"device_type\":\"" + "Android" + "\"}";

            Debug.Log("login json: "+json);
            StartCoroutine(R_WebServices.instance.POSTRequestData(R_RequestType.Login, json, OnLoginResponse));
        }
    }

    void OnLoginResponse(string serverResponse, bool isErrorMessage, string errorMessage)
    {
        if (isErrorMessage)
        {
            Debug.Log("error=" + errorMessage);
            loginMessageText.text = "Login response error";
            loginBtn.interactable = true;
        }
        else
        {
            Debug.Log("Response => OnLoginResponse: " + serverResponse);

            JsonData userData = JsonMapper.ToObject(serverResponse);
            
            IDictionary iUserData = userData as IDictionary;
            if (userData["statusCode"].ToString() == "200")
            {
                R_PlayerGameDetails loginPlayerData = new R_PlayerGameDetails();
                loginPlayerData.userId = userData["data"]["user_id"].ToString();
                loginPlayerData.name = userData["data"]["full_name"].ToString();
                loginPlayerData.userName = userData["data"]["username"].ToString();
                loginPlayerData.email = userData["data"]["email"].ToString();
                GlobalGameManager.token = loginPlayerData.token = userData["data"]["token"].ToString();
                //loginPlayerData.password = password.text;
                //loginPlayerData.avatarURL = userData["user"]["picture"].ToString();
                R_PlayerManager.instance.SetPlayerGameData(loginPlayerData);
                R_PrefsManager.SetData(R_PrefsKey.R_PlayerGameData, JsonUtility.ToJson(loginPlayerData));

                GameObject.Find("Canvas/Horizontal Scroll Snap/Content/Page1/Rummy_Home").gameObject.SetActive(true);
                RummyMainMenuController.instance.DestroyScreen(RummyMainMenuScreens.Login);
            }
            else
            {
                Debug.Log("error in userData");
                loginMessageText.text = "Login response data error";
                loginBtn.interactable = true;
            }
        }
    }

    void RegisterCheck()
    {
        if (string.IsNullOrEmpty(regName.text))
        {
            regMessageText.text = "Please enter name";
        }
        else if (string.IsNullOrEmpty(regEmail.text))
        {
            regMessageText.text = "Please enter username";
        }
        else if (string.IsNullOrEmpty(regPassword.text))
        {
            regMessageText.text = "Please enter password";
        }
        else
        {
            registerBtn.interactable = false;
            string json = "{\"name\": \""+regName.text+"\", \"email\": \""+regEmail.text+"\", \"password\": \""+regPassword.text+ "\", \"role\": \"user\"}";
            Debug.Log("register json: "+json);
            StartCoroutine(R_WebServices.instance.POSTRequestData(R_RequestType.Register, json, OnRegisterResponse));
        }
    }

    void OnRegisterResponse(string serverResponse, bool isErrorMessage, string errorMessage)
    {
        if (isErrorMessage)
        {
            Debug.Log("error=" + errorMessage);
            regMessageText.text = "Register response error";
            registerBtn.interactable = true;
        }
        else
        {
            Debug.Log("Response => OnRegisterResponse: " + serverResponse);

            JsonData userData = JsonMapper.ToObject(serverResponse);

            //if (userData["_id"] != null)
            //{
            //    R_PlayerGameDetails loginPlayerData = new R_PlayerGameDetails();
            //    loginPlayerData.userId = userData["_id"].ToString();
            //    loginPlayerData.name = userData["name"].ToString();
            //    loginPlayerData.emailId = userData["email"].ToString();
            //    loginPlayerData.token = userData["token"].ToString();
            //    loginPlayerData.password =regPassword.text;
            //    R_PlayerManager.instance.SetPlayerGameData(loginPlayerData);
            //    R_PrefsManager.SetData(R_PrefsKey.PlayerGameData, JsonUtility.ToJson(loginPlayerData));

            //    RummyMainMenuController.instance.DestroyScreen(RummyMainMenuScreens.Login);
            //    GameObject.Find("Canvas/Horizontal Scroll Snap/Content/Page1/R_Dashboard").gameObject.SetActive(true);
            //}
            IDictionary iUserData = userData as IDictionary;
            if (iUserData.Contains("token") && iUserData.Contains("user"))
            {
                R_PlayerGameDetails loginPlayerData = new R_PlayerGameDetails();
                loginPlayerData.userId = userData["user"]["id"].ToString();
                loginPlayerData.name = userData["user"]["name"].ToString();
                loginPlayerData.userName = userData["user"]["email"].ToString();
                loginPlayerData.email = userData["user"]["email"].ToString();
                loginPlayerData.token = userData["token"].ToString();
                loginPlayerData.password = regPassword.text;
                loginPlayerData.avatarURL = userData["user"]["picture"].ToString();
                R_PlayerManager.instance.SetPlayerGameData(loginPlayerData);
                R_PrefsManager.SetData(R_PrefsKey.R_PlayerGameData, JsonUtility.ToJson(loginPlayerData));

                RummyMainMenuController.instance.DestroyScreen(RummyMainMenuScreens.Login);
                GameObject.Find("Canvas/Horizontal Scroll Snap/Content/Page1/Rummy_Home").gameObject.SetActive(true);
            }
            else if (iUserData.Contains("valid") && iUserData.Contains("message"))
            {
                Debug.Log("error in userData");
                regMessageText.text = userData["message"].ToString();
                registerBtn.interactable = true;
            }
            else
            {
                Debug.Log("error in userData");
                regMessageText.text = "Register response data error";
                registerBtn.interactable = true;
            }
        }
    }

    void ClearTextFields()
    {
        username.text = string.Empty;
        password.text = string.Empty;
        regName.text = string.Empty;
        regEmail.text = string.Empty;
        regPassword.text = string.Empty;
        loginMessageText.text = string.Empty;
        regMessageText.text = string.Empty;
    }
}
