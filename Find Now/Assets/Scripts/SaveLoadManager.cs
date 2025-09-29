using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager instance { get; private set; }

    // --- PlayerPrefs Keys ---
    private const string LevelKey = "Level";
    private const string TotalCoinKey = "TotalCoin";
    private const string LivesKey = "CurrentLives";
    private const string LastLifeLostTimeKey = "LastLifeLostTime";

    [Header("UI References")]
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI timerText;

    // --- Life System ---
    private int maxLives = 5;
    [SerializeField] internal int currentLives;
    private TimeSpan regenerationTime = new TimeSpan(0, 30, 0); // 30 minutes per life
    private DateTime lastLifeLostTime;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);

        LoadLives();
        CheckForRegeneration();
        UpdateUI();
    }

    private void Update()
    {
        CheckForRegeneration();

        // UI only updates on MainMenu
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Lifetxt");
            if (obj != null)
            {
                livesText = obj.GetComponent<TextMeshProUGUI>();
                
            }
            GameObject obj1 = GameObject.FindGameObjectWithTag("Timertxt");
            if (obj1 != null)
            {
                timerText = obj1.GetComponent<TextMeshProUGUI>();
            }
            UpdateUI();
        }
    }

    // --- Public Methods ---

    public void SaveLevel(int score)
    {
        PlayerPrefs.SetInt(LevelKey, score);
    }

    public void StoreCoin(int coin)
    {
        PlayerPrefs.SetInt(TotalCoinKey, coin);
    }

    public int GetCoin()
    {
        if (PlayerPrefs.HasKey(TotalCoinKey))
        {
            return PlayerPrefs.GetInt(TotalCoinKey);
        }
        else
        {
            return 100;
        }
    }

    public int GetLevel()
    {
        if (PlayerPrefs.HasKey(LevelKey))
        {
            return PlayerPrefs.GetInt(LevelKey);
        }
        else if (PlayerPrefs.GetInt(LevelKey) > 10)
        {
            SaveLevel(1);
            return 1;
        }
        else
        {
            return 1;
        }
    }

    public void LoseLife()
    {
        if (currentLives > 0)
        {
            currentLives--;

            // Start timer only when lives drop below max and no timer running
            if (currentLives < maxLives && lastLifeLostTime == DateTime.MinValue)
            {
                lastLifeLostTime = DateTime.Now;
            }

            SaveLives();

            if (SceneManager.GetActiveScene().name == "MainMenu")
                UpdateUI();
        }
        else
        {
            // If no lives left → force to Main Menu
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void AddLife(int amount = 1)
    {
        currentLives = Mathf.Min(currentLives + amount, maxLives);

        // Reset timer if full lives reached
        if (currentLives == maxLives)
        {
            lastLifeLostTime = DateTime.MinValue;
        }

        SaveLives();

        if (SceneManager.GetActiveScene().name == "MainMenu")
            UpdateUI();
    }

    // --- Private Methods ---

    private void CheckForRegeneration()
    {
        if (currentLives < maxLives && lastLifeLostTime != DateTime.MinValue)
        {
            TimeSpan timeSinceLost = DateTime.Now - lastLifeLostTime;

            // How many lives should have regenerated?
            int livesToRegenerate = (int)(timeSinceLost.TotalMinutes / regenerationTime.TotalMinutes);

            if (livesToRegenerate > 0)
            {
                currentLives = Mathf.Min(currentLives + livesToRegenerate, maxLives);

                if (currentLives < maxLives)
                {
                    // Reset timer for next life
                    lastLifeLostTime = DateTime.Now - TimeSpan.FromMinutes(
                        timeSinceLost.TotalMinutes % regenerationTime.TotalMinutes
                    );
                }
                else
                {
                    // Full lives → stop timer
                    lastLifeLostTime = DateTime.MinValue;
                }

                SaveLives();
            }
        }
    }

    private void LoadLives()
    {
        currentLives = PlayerPrefs.GetInt(LivesKey, maxLives);

        string savedTime = PlayerPrefs.GetString(LastLifeLostTimeKey, DateTime.MinValue.ToBinary().ToString());
        lastLifeLostTime = DateTime.FromBinary(Convert.ToInt64(savedTime));
    }

    private void SaveLives()
    {
        PlayerPrefs.SetInt(LivesKey, currentLives);
        PlayerPrefs.SetString(LastLifeLostTimeKey, lastLifeLostTime.ToBinary().ToString());
        PlayerPrefs.Save();
    }

    private void UpdateUI()
    {
        if (livesText != null)
            livesText.text = "x" + currentLives;

        if (timerText != null)
        {
            if (currentLives < maxLives && lastLifeLostTime != DateTime.MinValue)
            {
                TimeSpan timeRemaining = regenerationTime - (DateTime.Now - lastLifeLostTime);
                if (timeRemaining.TotalSeconds > 0)
                {
                    // Show minutes + seconds countdown
                    timerText.text = string.Format("{0:D2}:{1:D2}",
                        timeRemaining.Minutes, timeRemaining.Seconds);
                }
                else
                {
                    timerText.text = "Regenerating...";
                }
            }
            else
            {
                // Full lives → just show the max lives number
                timerText.text = "" ;
            }
        }
    }
}
