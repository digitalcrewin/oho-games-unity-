using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_SoundManager : MonoBehaviour
{
    public static R_SoundManager instance;

	[SerializeField]
    private GameObject[] soundPrefabs;

    private float[] soundLength;

    public bool sound;
    public int volume;

    private void Awake()
    {
        instance = this;
        if (!PlayerPrefs.HasKey("issound"))
        {
            sound = true;
            volume = 1;
            PlayerPrefs.SetString("issound", "1");
            PlayerPrefs.Save();
        }
        else
        {
            SoundCheck();
        }
    }

    void Start()
    {
        soundLength = new float[soundPrefabs.Length];

        for (int i = 0; i < soundPrefabs.Length; i++)
        {
            soundLength[i] = soundPrefabs[i].GetComponent<AudioSource>().clip.length;
        }
    }

    public void PlaySound(R_SoundType soundType)
    {
        GameObject gm = Instantiate(soundPrefabs[(int)soundType],Vector3.zero,Quaternion.identity) as GameObject;
        gm.GetComponent<AudioSource>().volume = volume;
        Destroy(gm,10);
    }
    public void SoundCheck()
    {
        string soundsCheck = PlayerPrefs.GetString("issound");
        if (soundsCheck == "1")
        {
            sound = true;
            volume = 1;
        }
        else if (soundsCheck == "0")
        {
            sound = false;
            volume = 0;
        }
    }
}






public enum R_SoundType
{
	Click,
	CARD_PICKED,
	CARD_DISCARD,
	TIMER_ON,
    TIMER_LAST_10_SEC,
    WINNER
}