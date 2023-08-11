using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class P_TournamentsBlindStructure : MonoBehaviour
{
    public static P_TournamentsBlindStructure instance;

	[SerializeField] Transform scrollContent;
	[SerializeField] GameObject scrollItemPrefab;
	[SerializeField] GameObject noDataText;
	[SerializeField] Button closeBtn;

	string gameId;

	public string GameId
	{
		set
		{
			gameId = value;
		}
	}


	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		//if (!string.IsNullOrEmpty(gameId))
		//	StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/poker/games/" + gameId + "/blind-structure", BlindStructureResponse));

		closeBtn.onClick.AddListener(() =>
		{
			P_LobbySceneManager.instance.DestroyScreen(P_LobbyScreens.LobbyTournamentsBlindStructure);
		});
	}

	//void BlindStructureResponse(string serverResponse, bool isErrorMessage, string errorMessage)
	//{
	//	if (P_GameConstant.enableErrorLog)
	//		Debug.Log("Blind Structure Response : " + serverResponse);

	//	JsonData data = JsonMapper.ToObject(serverResponse);

	//	if (data["statusCode"].ToString() == "200")
	//	{
	//		if (data["data"].Count > 0)
	//		{
	//			noDataText.SetActive(false);
	//			DestroyAllScrollPrefab();

	//			for (int i = 0; i < data["data"].Count; i++)
	//			{
	//				GameObject go = Instantiate(scrollItemPrefab, scrollContent);
	//				go.transform.GetChild(0).GetComponent<Text>().text = data["data"][i]["level"].ToString();
	//				go.transform.GetChild(1).GetComponent<Text>().text = data["data"][i]["small_blind"].ToString() + " / " + data["data"][i]["big_blind"].ToString();
	//				go.transform.GetChild(2).GetComponent<Text>().text = data["data"][i]["ante"].ToString();
	//				go.transform.GetChild(3).GetComponent<Text>().text = data["data"][i]["blinds_up"].ToString() + "m";
	//			}
	//		}
	//		else
	//		{
	//			noDataText.SetActive(true);
	//		}
	//	}
	//	else
	//	{
	//		noDataText.SetActive(true);
	//	}
	//}

	void DestroyAllScrollPrefab()
	{
		for (int i = 0; i < scrollContent.childCount; i++)
		{
			Destroy(scrollContent.GetChild(i).gameObject);
		}
	}
}
