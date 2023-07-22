using LitJson;
using UnityEngine;
using UnityEngine.UI;

public class WithdrawCash : MonoBehaviour
{
	[SerializeField] Text walletBalanceText, kycStatusText;
	[SerializeField] GameEvents walletGameEvent;
	public InputField amountInputField;

	// Start is called before the first frame update
	void Start()
	{
		if (GlobalGameManager.instance.isKYCDone)
			kycStatusText.text = "<color=green>Completed!</color>";

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

		if (data["statusCode"].ToString() == "200")
		{
			int totalBalance = int.Parse(data["data"]["real_amount"].ToString().Split('.')[0]) + int.Parse(data["data"]["bonus_amount"].ToString().Split('.')[0]) + int.Parse(data["data"]["win_amount"].ToString().Split('.')[0]);
			walletBalanceText.text = "<size=21>₹</size> " + (totalBalance).ToString();
			walletGameEvent.RaiseEvent(serverResponse);
		}
		else
		{
			Debug.LogError(data["error"].ToString());
		}
	}

	public void OnClickOnButton(string eventName)
	{
		//SoundManager.instance.PlaySound(SoundType.Click);

		switch (eventName)
		{
			case "back":
				MainDashboardScreen.instance.DestroyScreen(MainDashboardScreen.MainDashboardScreens.WithdrawCash);
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

	public void WithdrawUserCash()
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
			string requestData = "{\"amount\":\"" + amountInputField.text + "\"}";

			WebServices.instance.SendRequest(RequestType.WithdrawRequest, requestData, true, WithdrawCashResponse);
			//GetWalletDetails();
		}
	}

	void WithdrawCashResponse(RequestType requestType, string serverResponse, bool isErrorMessage, string errorMessage)
	{
		Debug.Log("Response => Withdraw: " + serverResponse);
		JsonData data = JsonMapper.ToObject(serverResponse);

			MainDashboardScreen.instance.ShowMessage(data["message"].ToString());
		if (data["statusCode"].ToString() == "200")
		{
			WalletScreen.instance.GetWalletDetails();
			MainDashboardScreen.instance.DestroyScreen(MainDashboardScreen.MainDashboardScreens.WithdrawCash);
		}
		
	}
}
