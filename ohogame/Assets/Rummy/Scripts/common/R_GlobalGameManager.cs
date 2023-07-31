using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using LitJson;
using System;
using UnityEngine.Networking;

public class R_GlobalGameManager : MonoBehaviour {
#if UNITY_EDITOR
    public List<R_SocketEvetns> socketEvents = new List<R_SocketEvetns>();
    public bool CanDebugThis(R_SocketEvetns eventName)
    {
        for (int i = 0; i < socketEvents.Count; i++)
        {
            if (socketEvents[i] == eventName)
            {
                return true;
            }
        }
        return false;
    }
#endif

    public static R_GlobalGameManager instance;

    public GameObject[] gameScens; // prefab of all parent screens in game
    public GameObject loadingScreen;
    public GameObject previousScene = null; // contains current loaded sceneObject

    public bool IsJoiningPreviousGame = false;
    public bool isReJoinGame = false;
    public bool isTokenSent = false, creatingNewTable = false;

    public R_RoomData currentRoomData = new R_RoomData();
    // public List<GamePlayerInfo> gamePlayerInfos;
    [HideInInspector]
    public Sprite userPic;

    public GameObject mainSocketController;


    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        LoadScene(R_Scenes.MainMenuScene);

    }

   public void DestroyScene(R_Scenes gameScene)
    {
        if (previousScene != null)
        {
            StartCoroutine(WaitAndDestroyOldScreen(previousScene));
        }
    }

    public void LoadScene(R_Scenes gameScene)
    {
        if (previousScene != null)
        {
            StartCoroutine(WaitAndDestroyOldScreen(previousScene));
        }
        // previousScene = Instantiate(gameScens[(int)gameScene], Vector3.zero, Quaternion.identity) as GameObject;
        previousScene = Instantiate(gameScens[(int)gameScene], this.transform) as GameObject;
    }

    private IEnumerator WaitAndDestroyOldScreen(GameObject gm)
    {
        //yield return new WaitForSeconds(1);
        
        if (gm != null)
        {
            Destroy(gm);
        }
        else
        {

#if ERROR_LOG
            Debug.LogError("Null reference exception found gm is null in GlobalGameManager.WaitAndDestroyOldScreen ");
#endif
        }

        yield return new WaitForSeconds(0.2f);
        System.GC.Collect();
    }

    private void OnApplicationQuit()
    {
        SaveAllData();
    }

    public void CloseApplication()
    {
        SaveAllData();
        Application.Quit();
    }

    private void SaveAllData()
    {
        //if (null != PlayerManager.instance)
        //{
        //    PlayerGameDetails playerGame = PlayerManager.instance.GetPlayerGameData();
        //    PrefsManager.SetPlayerGameData(PlayerManager.instance.GetPlayerGameData());
        //}
    }

    public void SetRoomData(R_RoomData data)
    {
        currentRoomData = data;
    }

    public R_RoomData GetRoomData()
    {
        return currentRoomData;
    }

    public void ResetGame(string packet)
    {
        LoadScene(R_Scenes.InGame);

        R_Task runAfterWait = new R_Task(WaitForDelay(1f));
        runAfterWait.Finished += delegate(bool manual) {
            Rummy_InGameManager.instance.OnSetSeating(packet);
            runAfterWait.Finished -= delegate(bool manual){};
        };
    }

    public IEnumerator LoadSpriteImageFromUrl(string URL, Image image)
    {
        Debug.Log("Pic url " + URL);
        UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(URL);
        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
        {
            Debug.LogError("Download failed");
        }
        else
        {
            var Text = DownloadHandlerTexture.GetContent(unityWebRequest);
            Sprite sprite = Sprite.Create(Text, new Rect(0, 0, Text.width, Text.height), Vector2.zero);
            image.sprite = sprite;
            Debug.Log("Successfully Set Player Profile");
        }
    }

    public IEnumerator WaitForDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    public IEnumerator RunAfterDelay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action();
    }
    //StartCoroutine(RunAfterDelay(1f, () => { });
}


public enum R_Scenes
{
    MainMenuScene,
    InGame,
}

public enum R_GameMode
{
    _2_Player,
    _4_Player
}

[System.Serializable]
public class R_RoomData
{
    public string roomId;
    public R_GameMode gameMode;
    public int players;
    public string socketTableId;
}

public class R_GamePlayerInfo
{
    public string id;
    public string balance;
    public string userName;
    public string nickName;
    public string avatarURL,FrameUrl,CountryURL;
    public string countryName;
    public float coins;
}

[System.Serializable]
public class CardData
{
    public CardIcon cardIcon;
    public CardNumber cardNumber;
    public Sprite cardsSprite;
}

[System.Serializable]
public enum CardIcon
{
    NONE,
    CLUB,
    SPADES,
    DIAMOND,
    HEART
}

[System.Serializable]
public enum CardNumber
{
    NONE,
    TWO,
    THREE,
    FOUR,
    FIVE,
    SIX,
    SEVEN,
    EIGHT,
    NINE,
    TEN,
    JACK,
    QUEEN,
    KING,
    ACE
}