﻿using LitJson;
using UnityEngine;
using UnityEngine.UI;

public class AddCash : MonoBehaviour
{
	[SerializeField] Text walletBalanceText, kycStatusText;
	[SerializeField] GameEvents walletGameEvent;
	public InputField amountInputField;

	// Start is called before the first frame update
	void Start()
	{
		if (GlobalGameManager.instance.isKYCDone)
			kycStatusText.text = "<color=green>Completed!</color>";

		MainDashboardScreen.instance.ShowScreen(MainDashboardScreen.MainDashboardScreens.Loading);
		GetWalletDetails();
	}

	private void GetWalletDetails()
	{
		StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/user/get-wallet", WalletDetailsResponse));
	}

	private void WalletDetailsResponse(string serverResponse, bool isErrorMessage, string errorMessage)
	{
		print("Wallet Response : " + serverResponse);
		JsonData data = JsonMapper.ToObject(serverResponse);

		MainDashboardScreen.instance.DestroyScreen(MainDashboardScreen.MainDashboardScreens.Loading);
		if (data["statusCode"].ToString() == "200")
		{
			int totalBalance = int.Parse(data["data"]["real_amount"].ToString().Split('.')[0]) + int.Parse(data["data"]["bonus_amount"].ToString().Split('.')[0]) + int.Parse(data["data"]["win_amount"].ToString().Split('.')[0]);
			walletBalanceText.text = "<size=21>₹</size> " + (totalBalance).ToString();
			//walletGameEvent.RaiseEvent(serverResponse);
		}
		else
		{
			//Debug.LogError(data["error"].ToString());
			MainDashboardScreen.instance.ShowMessage(data["message"].ToString());
		}
	}

	public void OnClickOnButton(string eventName)
	{
		//SoundManager.instance.PlaySound(SoundType.Click);

		switch (eventName)
		{
			case "back":
				MainDashboardScreen.instance.DestroyScreen(MainDashboardScreen.MainDashboardScreens.AddCash);
				break;
			case "Offer":
				{
					//GlobalGameManager.instance.LoadScene(Scenes.InGame);
					//MainMenuController.instance.ShowScreen(MainMenuScreens.Offer);
				}
				break;

			default:
#if ERROR_LOG
            Debug.LogError("unhdnled eventName found in menuHandller = " + eventName);
#endif
				break;
		}
	}

	public void AddUserCash()
	{
		if (string.IsNullOrEmpty(amountInputField.text))
		{
			MainDashboardScreen.instance.ShowMessage("Please enter some amount");
		}
		else if (int.Parse(amountInputField.text) <= 0)
		{
			MainDashboardScreen.instance.ShowMessage("Amount should be greater than 0");
		}
		else
		{
			string requestData = "{\"amount\":\"" + amountInputField.text + "\"," +
								 "\"category\":\"" + "Poker" + "\"}";

			WebServices.instance.SendRequest(RequestType.AddCash, requestData, true, AddCashResponse);
			//GetWalletDetails();
		}
	}

	void AddCashResponse(RequestType requestType, string serverResponse, bool isErrorMessage, string errorMessage)
	{
		Debug.Log("Response => UserDetails: " + serverResponse);
		JsonData data = JsonMapper.ToObject(serverResponse);

		if (data["statusCode"].ToString() == "200")
		{
			//MainDashboardScreen.instance.ShowMessage(data["message"].ToString());
			//amountInputField.text = "";
			////MainMenuController.instance.ShowMessage(data["message"].ToString());
			WalletScreen.instance.InitialiseWebView(data["data"].ToString());
		}
		else
		{
			//Debug.LogError(data["error"].ToString());
			MainDashboardScreen.instance.ShowMessage(data["message"].ToString());
		}
	}
}
