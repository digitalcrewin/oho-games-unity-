using UnityEngine;
using UnityEngine.UI;

public class TransactionItem : MonoBehaviour
{
	int transactionId, userId;
	string amount, category, createdAt, updatedAt;
	public string otherType, type;
	[SerializeField] Text categoryText, amountText, dateText, titleText;

	// Start is called before the first frame update
	void Start()
	{

	}

	public void SetTransactionDetails(int transactionId, int userId, string otherType, string type,
		string amount, string category, string createdAt, string updatedAt)
	{
		this.transactionId = transactionId;
		this.userId = userId;
		this.otherType = otherType;
		this.type = type;
		this.amount = amount;
		this.category = category;
		this.createdAt = createdAt;
		this.updatedAt = updatedAt;
	}

	public void ShowTransactionDetails()
	{
		categoryText.text = category;
		titleText.text = otherType;
		if (type.Equals("CR") || otherType.Equals("Bonus"))
		{
			amountText.text = "+Rs." + amount;
		}
		else if (type.Equals("DR"))
		{
			amountText.text = "-Rs." + amount;
		}
		dateText.text = createdAt.Substring(0, 10);
	}
}
