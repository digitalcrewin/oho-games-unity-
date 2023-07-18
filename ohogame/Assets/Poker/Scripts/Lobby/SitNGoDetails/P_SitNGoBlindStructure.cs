using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_SitNGoBlindStructure : MonoBehaviour
{
    public static P_SitNGoBlindStructure instance;

    void Awake()
    {
        instance = this;
    }

    public void OnClickOnBackButton()
    {
        P_LobbySceneManager.instance.DestroyScreen(P_LobbyScreens.LobbySitNGoBlindStructure);
    }
}
