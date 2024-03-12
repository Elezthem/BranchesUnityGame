using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;


#if EASY_MOBILE
using EasyMobile;
#endif

public class UIManager_Branches : MonoBehaviour
{
    [Header("Object References")]
    public GameManager_Branches gameManager;
    public GameObject header;
    public Text score;
    public Text bestScore;
    public Text coinText;
    public Text title;
    public GameObject tapToStart;
    public GameObject characterSelectBtn;
    public GameObject menuButtons;
    public GameObject dailyRewardBtn;
    public Text dailyRewardBtnText;
    public GameObject rewardUI;

    Animator scoreAnimator;
    Animator dailyRewardAnimator;
    public GameObject panel_loading;


    void OnEnable()
    {
        GameManager_Branches.GameState_BRChanged += GameManager_GameState_BRChanged;
        ScoreManager_BR.ScoreUpdated += OnScoreUpdated;
    }

    void OnDisable()
    {
        GameManager_Branches.GameState_BRChanged -= GameManager_GameState_BRChanged;
        ScoreManager_BR.ScoreUpdated -= OnScoreUpdated;
    }

    // Use this for initialization
    void Start()
    {
        scoreAnimator = score.GetComponent<Animator>();
        dailyRewardAnimator = dailyRewardBtn.GetComponent<Animator>();

        Reset();
        ShowStartUI();
    }

    // Update is called once per frame
    void Update()
    {
        score.text = ScoreManager_BR.Instance.Score.ToString();
        bestScore.text = ScoreManager_BR.Instance.HighScore.ToString();
        coinText.text = CoinManager_BR.Instance.Coins.ToString();

        if (!DailyRewardController_BR.Instance.disable && dailyRewardBtn.gameObject.activeSelf)
        {
            if (DailyRewardController_BR.Instance.CanRewardNow())
            {
                dailyRewardBtnText.text = "ПОЛУЧИТЕ СВОЮ НАГРАДУ!";
                dailyRewardAnimator.SetTrigger("activate");
            }
            else
            {
                TimeSpan timeToReward = DailyRewardController_BR.Instance.TimeUntilReward;
                dailyRewardBtnText.text = string.Format("ВОЗНАГРАЖДЕНИЕ В {0:00}:{1:00}:{2:00}", timeToReward.Hours, timeToReward.Minutes, timeToReward.Seconds);
                dailyRewardAnimator.SetTrigger("deactivate");
            }
        }

        
    }

    void GameManager_GameState_BRChanged(GameState_BR newState, GameState_BR oldState)
    {
        if (newState == GameState_BR.Playing)
        {              
            ShowGameUI();
        }
        else if (newState == GameState_BR.PreGameOver)
        {
            // Before game over, i.e. game potentially will be recovered
        }
        else if (newState == GameState_BR.GameOver)
        {
            Invoke("ShowGameOverUI", 0.5f);
        }
    }

    void OnScoreUpdated(int newScore)
    {
        scoreAnimator.Play("NewScore");
    }

    void Reset()
    {

        header.SetActive(false);
        title.gameObject.SetActive(false);
        score.gameObject.SetActive(false);
        tapToStart.SetActive(false);
        characterSelectBtn.SetActive(false);
        menuButtons.SetActive(false);
        dailyRewardBtn.SetActive(false);


    }

    public void StartGame()
    {
        gameManager.StartGame();
    }

    public void EndGame()
    {
        gameManager.GameOver();
    }

    public void RestartGame()
    {
        gameManager.RestartGame(0.2f);
    }

    public void ShowStartUI()
    {
    

        header.SetActive(true);
        title.gameObject.SetActive(true);
        tapToStart.SetActive(true);
        characterSelectBtn.SetActive(true);

        // If first launch: show "WatchForCoins" and "DailyReward" buttons if the conditions are met
        if (GameManager_Branches.GameCount == 0)
        {
            ShowDailyRewardBtn();
        }
    }

    public void ShowGameUI()
    {
        header.SetActive(true);
        title.gameObject.SetActive(false);
        score.gameObject.SetActive(true);
        tapToStart.SetActive(false);
        characterSelectBtn.SetActive(false);
        dailyRewardBtn.SetActive(false);
    }

    public void ShowGameOverUI()
    {
        header.SetActive(true);
        title.gameObject.SetActive(false);
        score.gameObject.SetActive(true);
        tapToStart.SetActive(false);
        menuButtons.SetActive(true);

        // Show "WatchForCoins" and "DailyReward" buttons if the conditions are met
        ShowDailyRewardBtn();
        //AdManager.instance.show_ads_ingames();
    }

    void ShowWatchForCoinsBtn()
    {
        // Only show "watch for coins button" if a rewarded ad is loaded and premium features are enabled
        #if EASY_MOBILE
        if (IsPremiumFeaturesEnabled() && AdDisplayer.Instance.CanShowRewardedAd() && AdDisplayer.Instance.watchAdToEarnCoins)
        {
        watchForCoinsBtn.SetActive(true);
        watchForCoinsBtn.GetComponent<Animator>().SetTrigger("activate");
        }
        else
        {
        watchForCoinsBtn.SetActive(false);
        }
        #endif
    }

    void ShowDailyRewardBtn()
    {
        // Not showing the daily reward button if the feature is disabled
        if (!DailyRewardController_BR.Instance.disable)
        {
            dailyRewardBtn.SetActive(true);
        }
    }

   
   

    void OnCompleteRewardedAdToEarnCoins()
    {
        #if EASY_MOBILE
        // Unsubscribe
        AdDisplayer.CompleteRewardedAdToEarnCoins -= OnCompleteRewardedAdToEarnCoins;

        // Give the coins!
        ShowRewardUI(AdDisplayer.Instance.rewardedCoins);
        #endif
    }

    public void GrabDailyReward()
    {
        if (DailyRewardController_BR.Instance.CanRewardNow())
        {
            int reward = DailyRewardController_BR.Instance.GetRandomReward();

            // Round the number and make it mutiplies of 5 only.
            int roundedReward = (reward / 5) * 5;

            // Show the reward UI
            ShowRewardUI(roundedReward);

            // Update next time for the reward
            DailyRewardController_BR.Instance.ResetNextRewardTime();
        }
    }

    public void ShowRewardUI(int reward)
    {
        rewardUI.SetActive(true);
        rewardUI.GetComponent<RewardUIController_BR>().Reward(reward);
    }

    public void HideRewardUI()
    {
        rewardUI.GetComponent<RewardUIController_BR>().Close();
    }

    public void exitGame() {
        panel_loading.SetActive(true);
        SceneManager.LoadSceneAsync(0 , LoadSceneMode.Single);  
    }


    public void ShowCharacterSelectionScene()
    {
        SceneManager.LoadScene("CharacterSelection");
    }

  
}
