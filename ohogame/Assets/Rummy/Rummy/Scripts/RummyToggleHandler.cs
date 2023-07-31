using UnityEngine;
using UnityEngine.UI;

public class RummyToggleHandler : MonoBehaviour
{
	[SerializeField] Text label;
	[SerializeField] Color targetColor;

	// Start is called before the first frame update
	void Start()
	{

	}

	public void ToggleHandler(Toggle toggle)
	{
		if (toggle.isOn)
		{
			label.color = targetColor;
		}
		else
		{
			label.color = Color.white;
		}
	}
}
