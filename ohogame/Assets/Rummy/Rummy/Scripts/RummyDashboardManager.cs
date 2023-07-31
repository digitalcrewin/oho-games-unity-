using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BestHTTP.SocketIO3;
using LitJson;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class RummyDashboardManager : MonoBehaviour
{
	[SerializeField] UIHorizontalScroller horizontalScroller;
	[SerializeField] Toggle poolToggle, dealToggle, pointToggle;

	[Header("Point Rummy")]
	[SerializeField] Text pointCardPlayerNumberText;
	[SerializeField] Text pointCardEntryFeeText;

	Dictionary<string, List<RummyPointGameData>> rummyPointGameData = new Dictionary<string, List<RummyPointGameData>>();

	SocketManager socketManager;

	private int currentPointRummyCardPlayerIndex;
	private int currentPointRummyCardEntryFeeIndex;

	[SerializeField]
	Transform contentObj;
	[SerializeField]
	GameObject rummyLobbyCardItem;
	[SerializeField]
	Sprite tabSelected, tabUnselected;
	[SerializeField]
	Color selectedColor, unselectedColor;
	[SerializeField]
	GameObject pointTab, poolTab, dealTab;

	JsonData rummyLobbyCardsData = null;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
		GetAllPlayerTypes();

		//SetupSocketConnection();
    }

	private void SetupSocketConnection()
	{
		SocketOptions options = new SocketOptions();
		options.AutoConnect = false;

		PlayerGameDetails playerGameDetails = PlayerManager.instance.GetPlayerGameData();
		print(playerGameDetails.userId);
		socketManager = new SocketManager(new Uri("http://65.0.179.149:5001/?id=" + playerGameDetails.userId), options);
		socketManager.Socket.On("connect", () =>
		{
			Debug.Log(socketManager.Handshake.Sid);
		});
	}

	private void TurnOnSocketConnection()
	{
		PlayerGameDetails playerGameDetails = PlayerManager.instance.GetPlayerGameData();
		print(playerGameDetails.userId);
		socketManager = new SocketManager(new Uri("http://65.0.179.149:5001/?id=" + playerGameDetails.userId));
		socketManager.Socket.On("connect", () => Debug.Log(socketManager.Handshake.Sid));
	}

    public void OnPressedBackButton()
    {
		UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
	}

	public void OnclickOnButton(string eventName)
    {
		Debug.Log("EventName " + eventName);
		switch(eventName)
        {
			case "1":
				pointTab.GetComponent<Image>().sprite = tabSelected;
				poolTab.GetComponent<Image>().sprite = tabUnselected;
				dealTab.GetComponent<Image>().sprite = tabUnselected;
				pointTab.transform.GetChild(0).GetComponent<Text>().color = selectedColor;
				poolTab.transform.GetChild(0).GetComponent<Text>().color = unselectedColor;
				dealTab.transform.GetChild(0).GetComponent<Text>().color = unselectedColor;
				break;
			case "2":
				pointTab.GetComponent<Image>().sprite = tabUnselected;
				poolTab.GetComponent<Image>().sprite = tabSelected;
				dealTab.GetComponent<Image>().sprite = tabUnselected;
				pointTab.transform.GetChild(0).GetComponent<Text>().color = unselectedColor;
				poolTab.transform.GetChild(0).GetComponent<Text>().color = selectedColor;
				dealTab.transform.GetChild(0).GetComponent<Text>().color = unselectedColor;
				break;
			case "3":
				pointTab.GetComponent<Image>().sprite = tabUnselected;
				poolTab.GetComponent<Image>().sprite = tabUnselected;
				dealTab.GetComponent<Image>().sprite = tabSelected;
				pointTab.transform.GetChild(0).GetComponent<Text>().color = unselectedColor;
				poolTab.transform.GetChild(0).GetComponent<Text>().color = unselectedColor;
				dealTab.transform.GetChild(0).GetComponent<Text>().color = selectedColor;
				break;
		}
		PopulateLobbyCards(eventName);
    }

    public void GetAllPlayerTypes()
    {
		//StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/rummy/player/type", GetAllPlayerTypesResponse));
		StartCoroutine(R_WebServices.instance.GETRequestData(R_RequestType.Games, GetAllPlayerTypesResponse));
	}

	private void GetAllPlayerTypesResponse(string serverResponse, bool isErrorMessage, string errorMessage)
	{
		Debug.Log("GetAllPlayerTypesResponse " + serverResponse);
		JsonData data = JsonMapper.ToObject(serverResponse);

		if (data["message"].ToString() == "Success")
		{
			Debug.Log("Total " + data["data"].Count);
			rummyLobbyCardsData = data;
			PopulateLobbyCards("1");
			/*for (int i = 0; i < data["data"].Count; i++)
			{
				
				contentObj.GetChild(i).GetChild(9).GetComponent<Text>().text = data["data"][i]["maximum_player"].ToString();
				contentObj.GetChild(i).GetChild(11).GetComponent<Text>().text = data["data"][i]["entry_fee"].ToString() + " Rs";

				int index = i;
				contentObj.GetChild(i).GetChild(6).GetComponent<Button>().onClick.AddListener(() => OnClickOnTableItem(index, data["data"][index]["game_id"].ToString(), data["data"][index]));
			}*/
		}
		else
		{
			Debug.LogError(data["error"].ToString());
		}
	}

	void PopulateLobbyCards(string gameType)
    {
		for (int i = 0; i < contentObj.childCount; i++)
		{
			Destroy(contentObj.GetChild(i).gameObject);
		}

		horizontalScroller.enabled = false;
		List<GameObject> lobbyCard = new List<GameObject>();
		for (int i = 0; i < rummyLobbyCardsData["data"].Count; i++)
		{
			if (rummyLobbyCardsData["data"][i]["rummy_code"].ToString() == gameType)
			{
				GameObject obj = Instantiate(rummyLobbyCardItem, contentObj);
				obj.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = rummyLobbyCardsData["data"][i]["name"].ToString();
				obj.transform.GetChild(9).GetComponent<Text>().text = rummyLobbyCardsData["data"][i]["maximum_player"].ToString();
				obj.transform.GetChild(11).GetComponent<Text>().text = rummyLobbyCardsData["data"][i]["entry_fee"].ToString() + " Rs";

				int index = i;
				obj.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(() => OnClickOnTableItem(index, rummyLobbyCardsData["data"][index]["game_id"].ToString(), rummyLobbyCardsData["data"][index]));

				lobbyCard.Add(obj);
			}
		}
		horizontalScroller.arrayOfElements = new GameObject[lobbyCard.Count];
		horizontalScroller.arrayOfElements = lobbyCard.ToArray();
		horizontalScroller.enabled = true;
		horizontalScroller.UpdateChildren(-1, horizontalScroller.arrayOfElements);

		/*switch (gameType)
        {
			case "1":				
				break;
			case "2":
				break;
			case "3":
				break;
		}*/
	}

	void OnClickOnTableItem(int index, string gameType, JsonData selectedRow)
	{
		Debug.Log(gameType + " selected currentUserId=" + R_PlayerManager.instance.GetPlayerGameData().userId);

		if(selectedRow["rummy_code"].ToString() == "2" || selectedRow["rummy_code"].ToString() == "3")
        {
			RummyMainMenuController.instance.ShowMessage("Comming soon!");
			return;
        }

		R_GlobalGameManager.instance.LoadScene(R_Scenes.InGame);
		Debug.Log("heyyy=" + R_GlobalGameManager.instance.mainSocketController.name);
		R_GlobalGameManager.instance.mainSocketController.GetComponent<R_SocketController>().enabled = true;
		R_SocketController.instance.rummyUserId = R_PlayerManager.instance.GetPlayerGameData().userId; //PlayerManager.instance.
		R_SocketController.instance.rummyGameType = gameType;
		R_SocketController.instance.selectedRow = selectedRow;
		if (R_GlobalGameManager.instance.isReJoinGame)
			R_SocketController.instance.ReStart();
	}


	private void GetEntryFee(string playerType)
    {
		StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/rummy/entryFee/Name/info/" + playerType, GetEntryFeeResponse));
	}

	private void GetEntryFeeResponse(string serverResponse, bool isErrorMessage, string error)
	{
		Debug.Log("Response => GetEntryFeeResponse: " + serverResponse);
		JsonData data = JsonMapper.ToObject(serverResponse);

		if (data["statusCode"].ToString() == "200")
		{
			List<RummyPointGameData> existingRummyPointGames;

			for (int i = 0; i < data["data"].Count; i++)
			{
				bool isValueAvailable = rummyPointGameData.TryGetValue(data["data"][i]["playerType"].ToString(), out existingRummyPointGames);

				if (isValueAvailable == false)
				{
					existingRummyPointGames = new List<RummyPointGameData>();
					rummyPointGameData[data["data"][i]["playerType"].ToString()] = existingRummyPointGames;
				}

				existingRummyPointGames.Add(new RummyPointGameData(data["data"][i]["entry_fee"].ToString(), data["data"][i]["Name"].ToString()));

				if (i == 0)
				{
					UpdatePointRummyEntryFee();
				}
			}
		}
		else
		{
			Debug.LogError(data["error"].ToString());
		}
	}

	private void GetGameIDByName(string gameName)
	{
		StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/rummy/get/gameId/" + gameName, GetGameIDByNameResponse));
	}

	private void GetGameIDByNameResponse(string serverResponse, bool isErrorMessage, string error)
	{
		Debug.Log("Response => GetAllPlayerTypes: " + serverResponse);
	}

	public void OnPressedPointRummyIncPlayerButton()
	{
		if (currentPointRummyCardPlayerIndex + 1 <= rummyPointGameData.Count - 1)
		{
			pointCardPlayerNumberText.text = rummyPointGameData.ElementAt(++currentPointRummyCardPlayerIndex).Key;

			currentPointRummyCardEntryFeeIndex = 0;
			UpdatePointRummyEntryFee();
		}
	}

	public void OnPressedPointRummyDecPlayerButton()
	{
		if (currentPointRummyCardPlayerIndex - 1 >= 0)
		{
			pointCardPlayerNumberText.text = rummyPointGameData.ElementAt(--currentPointRummyCardPlayerIndex).Key;
			
			currentPointRummyCardEntryFeeIndex = 0;
			UpdatePointRummyEntryFee();
		}
	}

	public void OnPressedPointRummyIncEntryFessButton()
	{
		if (currentPointRummyCardEntryFeeIndex != rummyPointGameData.ElementAt(currentPointRummyCardPlayerIndex).Value.Count - 1)
		{
			currentPointRummyCardEntryFeeIndex++;
			UpdatePointRummyEntryFee();
		}
	}

	public void OnPressedPointRummyDecEntryFessButton()
	{
		if (currentPointRummyCardEntryFeeIndex != 0)
		{
			currentPointRummyCardEntryFeeIndex--;
			UpdatePointRummyEntryFee();
		}	
	}

	private void UpdatePointRummyEntryFee()
	{
		List<RummyPointGameData> pointRummyGameValue;
		rummyPointGameData.TryGetValue(rummyPointGameData.ElementAt(currentPointRummyCardPlayerIndex).Key,
			out pointRummyGameValue);

		pointCardEntryFeeText.text = pointRummyGameValue[currentPointRummyCardEntryFeeIndex].entryFee;
	}

	public void OnPressedPoolToggle(Toggle toggle)
	{
		if (toggle.isOn)
		{
			horizontalScroller.SnapToElement(0);
		}
	}
	
	public void OnPressedDealToggle(Toggle toggle)
	{
		if (toggle.isOn)
		{
			horizontalScroller.SnapToElement(1);
		}
	}
	
	public void OnPressedPointToggle(Toggle toggle)
	{
		if (toggle.isOn)
		{
			horizontalScroller.SnapToElement(2);
		}
	}

	public void OnScrollerFocusChanged(Int32 index)
	{
		switch (index) 
		{
			case 0: poolToggle.isOn = true; break;
			case 1: dealToggle.isOn = true; break;
			case 2: pointToggle.isOn = true; break;
				default: break;
		}
	}

	public void OnPressedRummyPointCardPlayNowButton()
	{
		TurnOnSocketConnection();

		//if (MainDashboardScreen.instance != null)
		//{
		//	MainDashboardScreen.instance.ShowScreen(MainDashboardScreen.MainDashboardScreens.RummyGameplay);

		//	if (Screen.orientation == ScreenOrientation.Portrait)
		//	{
		//		Screen.orientation = ScreenOrientation.LandscapeLeft;
		//	}
		//}
	}

	class RummyPointGameData
	{
		public string entryFee;
		public string gameName;

		public RummyPointGameData(string entryFee, string gameName)
		{
			this.entryFee = entryFee;
			this.gameName = gameName;
		}
	}

}
