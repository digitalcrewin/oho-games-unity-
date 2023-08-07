using System;
using System.Collections.Generic;
using LitJson;
//using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class WalletScreen : MonoBehaviour
{
	public static WalletScreen instance;

	[SerializeField] Text walletBalanceText, winAmountText, depositAmountText, bonusAmountText, kycStatusText, practiceAmountText;
	[SerializeField] GameObject transactionsPanel;
	[SerializeField] Transform transactionScrollContent;
	[SerializeField] GameObject depositTransactionItemPrefab, withdrawalTransactionItemPrefab, cashbackTransactionItemPrefab;
	[SerializeField] GameEvents walletGameEvent;
	[SerializeField] Button claimPracticeAmtBtn;

	List<TransactionItem> transactionItems = new List<TransactionItem>();
	public GameObject UniWebViewObject;
	UniWebView UniWebView;


	private void Awake()
    {
		instance = this;
		UniWebView = UniWebViewObject.GetComponent<UniWebView>();
	}

    public void InitialiseWebView(string payLink)
	{
		UniWebViewObject.SetActive(true);
		UniWebView.SetShowSpinnerWhileLoading(true);
		UniWebView.SetSpinnerText("Loading. Please do not close or go back.");
		UniWebView.Load(payLink);
		UniWebView.Show(true);
	}

	public void CloseWebView()
	{
		UniWebViewObject.SetActive(false);
		UniWebView.Hide(true);
	}

	private void OnEnable()
	{
		if (GlobalGameManager.instance.isKYCDone)
			kycStatusText.text = "<color=green>KYC Completed!</color>";

		UniWebView.OnMessageReceived += (view, message) => {
			Debug.Log("Message " + message.Path);
			MainDashboardScreen.instance.DestroyScreen(MainDashboardScreen.MainDashboardScreens.AddCash);
			MainDashboardScreen.instance.DestroyScreen(MainDashboardScreen.MainDashboardScreens.WithdrawCash);
			Debug.Log("Message AddCashScreenDestroyed");
			if (message.Path.Equals("close"))
			{
				GetWalletDetails();
				UniWebViewObject.SetActive(false);
			}
		};

		transactionsPanel.SetActive(false);
		GetWalletDetails();
		walletGameEvent.OnGameEventInvoked += OnWalletEventInvoked;
	}

	private void OnDisable()
	{
		walletGameEvent.OnGameEventInvoked -= OnWalletEventInvoked;
	}

	private void OnWalletEventInvoked(string serverResponse)
	{
		JsonData data = JsonMapper.ToObject(serverResponse);

		walletBalanceText.text = depositAmountText.text = (data["data"]["real_amount"]).ToString();
		winAmountText.text = (data["data"]["win_amount"]).ToString();
		bonusAmountText.text = (data["data"]["bonus_amount"]).ToString();
		
	}

	public void GetWalletDetails()
	{
		MainDashboardScreen.instance.ShowScreen(MainDashboardScreen.MainDashboardScreens.Loading);
		StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/user/get-wallet", WalletDetailsResponse));
	}

	private void WalletDetailsResponse(string serverResponse, bool isErrorMessage, string errorMessage)
	{
		print("Wallet Response : " + serverResponse);
		JsonData data = JsonMapper.ToObject(serverResponse);

		MainDashboardScreen.instance.DestroyScreen(MainDashboardScreen.MainDashboardScreens.Loading);
		if (data["statusCode"].ToString() == "200")
		{
			Debug.Log("Real Amount " + data["data"]["real_amount"].ToString());
			int totalBalance = int.Parse(data["data"]["real_amount"].ToString().Split('.')[0]) + int.Parse(data["data"]["bonus_amount"].ToString().Split('.')[0]) + int.Parse(data["data"]["win_amount"].ToString().Split('.')[0]);
			walletBalanceText.text = "<size=21>₹</size> " + totalBalance.ToString();
			MainDashboardScreen.instance.balanceText.text = totalBalance.ToString();
			depositAmountText.text = "<size=21>₹</size> " + data["data"]["real_amount"].ToString();
			winAmountText.text = "<size=21>₹</size> " + (data["data"]["win_amount"]).ToString();
			bonusAmountText.text = "<size=21>₹</size> " + (data["data"]["bonus_amount"]).ToString();
			practiceAmountText.text = "<size=21>₹</size> " + data["data"]["practice_amount"].ToString();
			string is_claim = data["data"]["is_claim"].ToString();
			if (is_claim == "1")
			{
				ClaimPracticeAmtBtnDisable();
			}
			else
			{
				ClaimPracticeAmtBtnEnable();
			}
		}
		else
		{
			//Debug.LogError(data["error"].ToString());
			MainDashboardScreen.instance.ShowMessage(data["message"].ToString());
		}
	}

	public void OnClickClaimPracticeAmtBtn()
    {
		StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/user/claim-practice-amount", ClaimPracticeAmountResponse));
		ClaimPracticeAmtBtnDisable();
	}

	private void ClaimPracticeAmountResponse(string serverResponse, bool isErrorMessage, string errorMessage)
	{
		ClaimPracticeAmtBtnEnable();

		Debug.Log("ClaimPracticeAmount Response : " + serverResponse);
		JsonData data = JsonMapper.ToObject(serverResponse);

		if (data["statusCode"].ToString() == "200")
		{
			MainDashboardScreen.instance.MenuSelection(1);
		}
		else
		{
			MainDashboardScreen.instance.ShowMessage(data["message"].ToString());
		}
	}

	void ClaimPracticeAmtBtnEnable()
    {
		claimPracticeAmtBtn.interactable = true;
		claimPracticeAmtBtn.transform.GetChild(0).GetComponent<Text>().color = new Color32(255, 255, 255, 255);
	}

	void ClaimPracticeAmtBtnDisable()
	{
		claimPracticeAmtBtn.interactable = false;
		claimPracticeAmtBtn.transform.GetChild(0).GetComponent<Text>().color = new Color32(200, 200, 200, 128);
	}

	public void RemovePreviousTransactionItems()
	{
		for (int i = 0; i < transactionScrollContent.childCount; i++)
		{
			Destroy(transactionScrollContent.GetChild(i).gameObject);
		}

		transactionItems.Clear();
	}

	public void GetTransactionDetails()
	{
		StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/user/my-transactions", TransactionDetailsResponse));
	}

	private void TransactionDetailsResponse(string serverResponse, bool isErrorMessage, string errorMessage)
	{
		print("Transaction Details Response : " + serverResponse);
		JsonData data = JsonMapper.ToObject(serverResponse);

		if (data["statusCode"].ToString() == "200")
		{
			for (int i = 0; i < data["data"].Count; i++)
			{
				TransactionItem transactionItem = CreateTransactionItems((data["data"][i]["type"]).ToString());
				transactionItems.Add(transactionItem);
				string cat = "";
				if (data["data"][i]["category"] != null)
					cat = data["data"][i]["category"].ToString();
				Debug.Log("Category " + cat);
				transactionItem.SetTransactionDetails((int)data["data"][i]["transaction_id"],
						(int)data["data"][i]["user_id"],
						data["data"][i]["other_type"].ToString(),
						data["data"][i]["type"].ToString(),
						data["data"][i]["amount"].ToString(),
						/*data["data"][i]["category"].ToString()*/cat,
						data["data"][i]["createdAt"].ToString(),
						data["data"][i]["updatedAt"].ToString());

				transactionItem.ShowTransactionDetails();

			}
		}
	}

	private TransactionItem CreateTransactionItems(string typeOfTransaction)
	{
		TransactionItem transactionItem;
		switch (typeOfTransaction) 
		{
			case "CR":
				transactionItem = Instantiate(depositTransactionItemPrefab, transactionScrollContent).
					GetComponent<TransactionItem>();
				break;
			case "DR":
				transactionItem = Instantiate(withdrawalTransactionItemPrefab, transactionScrollContent).
					GetComponent<TransactionItem>();
				break;
			case "Cashback":
				transactionItem = Instantiate(cashbackTransactionItemPrefab, transactionScrollContent).
					GetComponent<TransactionItem>(); 
				break;
			default:
				transactionItem = Instantiate(depositTransactionItemPrefab, transactionScrollContent).
					GetComponent<TransactionItem>();
				break;

		}
		
		return transactionItem;
	}

	public void OnDepositToggleChanged(Toggle toggle)
	{
		foreach (var item in transactionItems)
		{
			if (item.type.Equals("CR"))
			{
				item.gameObject.SetActive(toggle.isOn);
			}
		}
	}

	public void OnWithdrawToggleChanged(Toggle toggle)
	{
		foreach (var item in transactionItems)
		{
			if (item.type.Equals("DR"))
			{
				item.gameObject.SetActive(toggle.isOn);
			}
		}
	}

	public void OnCashbackToggleChanged(Toggle toggle)
	{
		foreach (var item in transactionItems)
		{
			if (item.otherType.Equals("Bonus"))
			{
				item.gameObject.SetActive(toggle.isOn);
			}
		}
	}
}
