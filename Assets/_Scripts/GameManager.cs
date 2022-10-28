using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using TMPro;

public class GameManager : MonoBehaviour
{
    private int COIN_SCORE_VALUE = 5;
    public static GameManager Instance { get; set; }
    public bool GameStarted { get; internal set; }
    public bool IsDead { get; internal set; }

    private PlayerMotor _playerMotor;

    // for death
    public GameObject deadAnimPanel;
    public TextMeshProUGUI deadScoreText, deadCoinText;



    // UI fields, we could migrate to an actual UI gameobject. 
    [SerializeField] public Text scoreText;
    [SerializeField] public Text coinsText;
    [SerializeField] public TextMeshProUGUI highScoreText;
    [SerializeField] public Text modifierText;
    [SerializeField] public Animator gameMenu;
    [SerializeField] public Animator MenuPanel;
    private float score;
    private int coins;
    private float modifier;
    private int lastScore;

    private void Awake()
    {
        IsDead = false;
        Instance = this;
        modifier = 1;
        highScoreText.text = PlayerPrefs.GetInt("HiScore").ToString();
    }
    internal void GetCoin()
    {
        gameMenu.SetTrigger("Pick_Coin");
        coins++;
        // check if achievement is unlocked. 

        switch (coins)
        {
            case 50:
                UnlockAchievement(GPGSPenguRunSDIds.achievement_collect_50_coins); 
                break;
            case 100:
                UnlockAchievement(GPGSPenguRunSDIds.achievement_collect_100_coins); 
                break;
            case 150:
                UnlockAchievement(GPGSPenguRunSDIds.achievement_collect_150_coins);
                break;
            case 200:
                UnlockAchievement(GPGSPenguRunSDIds.achievement_collect_200_coins);
                break; 
        }

        coinsText.text = coins.ToString();
        score += COIN_SCORE_VALUE;
        scoreText.text = score.ToString("0");
    }
    internal void UpdateModifier(float v)
    {
        modifier = 1.0f + v;
        modifierText.text = "x" + modifier.ToString("0.0"); // modifying the string to have only 1 decimal place. 
    }
    private void Update()
    {
        if (GameStarted && !IsDead)
        {
            MenuPanel.SetTrigger("Hide");
            FindObjectOfType<GlacierSpawner>().IsScrolling = true;
            FindObjectOfType<CameraMotor>().IsMoving = true;
            // only update the score if it's difference from the last score. but honestly just use events. 
            score += Time.deltaTime * modifier;
            if (lastScore != (int)score)
            {
                //print("Score Update");
                lastScore = (int)score;
                scoreText.text = score.ToString("0");
            }
        }
    }

    public void OnPlayButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void OnDeath()
    {
        IsDead = true;
        FindObjectOfType<GlacierSpawner>().IsScrolling = false;
        //deadAnim.SetTrigger("Dead"); 
        deadAnimPanel.SetActive(true);
        deadScoreText.SetText(score.ToString("0"));
        deadCoinText.SetText(coins + "");

        if (score > PlayerPrefs.GetInt("HiScore"))
        {
            ReportScore((int)score); 
            float s = score;
            if (s % 1 == 0)
            {
                s += 1;
            }
            PlayerPrefs.SetInt("HiScore", System.Convert.ToInt32(s));
        }
    }

    public void OnAchievementClick()
    {
        if (Social.localUser.authenticated)
        {
            Social.ShowAchievementsUI();
        }
    }
    public void OnLeaderboardClick()
    {
        if (Social.localUser.authenticated)
        {
            Social.ShowLeaderboardUI();
        }
    }

    public void UnlockAchievement(string achievementID)
    {
        // if it is an incremental achievement, we increment the value here gradually, 
        // until it meets 100%. but if its an instant achievement, we just skip progress
        // to 100%. 
        Social.ReportProgress(achievementID, 100.0f, (bool success) =>
        {
            Debug.Log("Achievement Unlocked");
        });
    }

    public void ReportScore(int score)
    {
        // if the score is low, the leaderboard won't update it. 
        // because the current existing one will already be higher. 
        Social.ReportScore(score, GPGSPenguRunSDIds.leaderboard_pengurunsd_leaderboard,
            (bool success) =>
            {
                Debug.Log("Reported score to leaderboard" + success);
            });
    }
}
