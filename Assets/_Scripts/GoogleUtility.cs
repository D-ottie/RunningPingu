using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames; 

public class GoogleUtility : MonoBehaviour
{
    public GameObject connectedUI; 
    public GameObject disconnectedUI; 
    
    public static GoogleUtility instance;

    private void Awake()
    {
        instance = this;
        PlayGamesPlatform.Activate();
        OnConnectionResponse(PlayGamesPlatform.Instance.localUser.authenticated); 
    }
    public void OnConnectClick()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            OnConnectionResponse(success); 
        }); 
    }
    private void OnConnectionResponse(bool authenticated)
    {
        if (authenticated)
        {
            GameManager.Instance.UnlockAchievement(GPGSPenguRunSDIds.achievement_log_in); 
            connectedUI.SetActive(true);
            disconnectedUI.SetActive(false); 
        }else
        {
            connectedUI.SetActive(false);
            disconnectedUI.SetActive(true); 
        }
    }


}
