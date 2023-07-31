using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;

public class R_WebServices : MonoBehaviour
{
    public static R_WebServices instance;

    void Awake()
	{	
		instance = this;
	}

	public bool IsInternetAvailable()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			return false;
		}

		return true;
	}


	//DEV_CODE
    public IEnumerator GETRequestData(R_RequestType requestType, System.Action<string, bool, string> callbackOnFinish)
	{
		Dictionary<string, string> headerData = new Dictionary<string, string>();
		headerData.Add("Authorization", (GlobalGameManager.token.Length > 0 ? "Bearer " + GlobalGameManager.token : ""));

		UnityWebRequest uwr = UnityWebRequest.Get(R_GameConstants.GAME_URLS[(int)requestType]);

		if (R_GameConstants.enableLog)
			Debug.Log("Request : " + R_GameConstants.GAME_URLS[(int)requestType] + " - " + GlobalGameManager.token);

		if (headerData.Count > 0)
		{
			foreach (var param in headerData)
			{
				uwr.SetRequestHeader(param.Key, param.Value);
			}
		}
		yield return uwr.SendWebRequest();

		if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.DataProcessingError)
		{
			callbackOnFinish("", true, uwr.error);
		}
		else
		{
			callbackOnFinish(uwr.downloadHandler.text, false, "");
		}
		headerData.Clear();
		uwr.Dispose();
	}

	public IEnumerator POSTRequestData(R_RequestType requestType, string json, System.Action<string, bool, string> callbackOnFinish)
	{
		Dictionary<string, string> headerData = new Dictionary<string, string>
		{
			//{ "Authorization", (GlobalGameManager.playerToken.Length > 0 ? GlobalGameManager.playerToken : "") }
		};
		if ((requestType == R_RequestType.Register) || (requestType == R_RequestType.Login))
		{
			headerData.Add("token", "masterKey");
			headerData.Add("Authorization", "Bearer masterKey");
        }

		UnityWebRequest uwr = new UnityWebRequest(R_GameConstants.GAME_URLS[(int)requestType], "POST");

		if (R_GameConstants.enableLog)
			Debug.Log("Request : " + R_GameConstants.GAME_URLS[(int)requestType]);

		if (json != "")
		{
			byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
			uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
		}

		uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        //uwr.SetRequestHeader("Authorization", (GlobalGameManager.playerToken.Length > 0 ? GlobalGameManager.playerToken : ""));
        uwr.SetRequestHeader("Content-Type", "application/json");

        if (headerData.Count > 0)
		{
			foreach (var param in headerData)
			{
				uwr.SetRequestHeader(param.Key, param.Value);
			}
		}
		yield return uwr.SendWebRequest();

		if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.DataProcessingError)
		{
			callbackOnFinish("", true, uwr.error);
		}
		else
		{
			callbackOnFinish(uwr.downloadHandler.text, false, "");
		}
	}

	public IEnumerator DeleteRequestData(R_RequestType requestType, System.Action<string, bool, string> callbackOnFinish)
	{
		Dictionary<string, string> headerData = new Dictionary<string, string>
		{
			//{ "Authorization", (GlobalGameManager.playerToken.Length > 0 ? GlobalGameManager.playerToken : "") }
		};

		UnityWebRequest uwr = UnityWebRequest.Delete(R_GameConstants.GAME_URLS[(int)requestType]);
		uwr.downloadHandler = new DownloadHandlerBuffer();
		if (R_GameConstants.enableLog)
			Debug.Log("Request : " + R_GameConstants.GAME_URLS[(int)requestType]);

		if (headerData.Count > 0)
		{
			foreach (var param in headerData)
			{
				uwr.SetRequestHeader(param.Key, param.Value);
			}
		}
		yield return uwr.SendWebRequest();

		if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.DataProcessingError)
		{
			callbackOnFinish("", true, uwr.error);
		}
		else
		{
			callbackOnFinish(uwr.downloadHandler.text, false, "");
		}
	}



	public IEnumerator LoadImageFromUrl(string URL, Image image) 
	{
		UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(URL);
		yield return unityWebRequest.SendWebRequest();

		if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError)
		{
			if (R_GameConstants.enableErrorLog)
				Debug.LogError("Download failed");
		}
		else
		{
			//if (R_GameConstants.enableLog)
				//Debug.Log("Texture Download Successfull");
			var Text = DownloadHandlerTexture.GetContent(unityWebRequest);
			Sprite sprite = Sprite.Create(Text, new Rect(0, 0, Text.width, Text.height), Vector2.zero);

            try
			{
				image.sprite = sprite;
				image.color = new Color32(255, 255, 255, 255);
			}
			catch
			{
				//if (R_GameConstants.enableLog)
					//Debug.Log("Error in set Image");
			}
		}
		unityWebRequest.Dispose();
	}

	public IEnumerator DownloadVideo(string url, string fileName)
	{
		UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
		if (R_GameConstants.enableLog)
			Debug.Log("Downloading Video..");
		yield return unityWebRequest.SendWebRequest();

		if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError)
		{
			if (R_GameConstants.enableLog)
				Debug.Log(unityWebRequest.error);
		}
		else
		{
			if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Videos")))
				Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Videos"));

			File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "Videos", fileName), unityWebRequest.downloadHandler.data);
		}
		unityWebRequest.Dispose();
	}

	public IEnumerator ShowPopUp(Text popUpText, string msg, float delay)
	{
		popUpText.gameObject.SetActive(true);
		popUpText.text = msg;
		//yield return new WaitForSeconds(delay);
		yield return new WaitForSeconds(2f);
		popUpText.gameObject.SetActive(false);
	}
}
