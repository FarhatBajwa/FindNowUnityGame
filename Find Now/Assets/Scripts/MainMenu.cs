
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject InformationAboutLife;
    public Image SoundSprite;
    public Sprite SoundOn, SoundOff;
    public TextMeshProUGUI CoinUi;

    [Header("Life Panel")]
    public int FullLifeCoin, OneLifeCoin;
    public TextMeshProUGUI FullLifeCoinUi, OneLifeCoinUi;

    private void Start()
    {
        FullLifeCoinUi.text = $"{FullLifeCoin}";
        OneLifeCoinUi.text = $"{OneLifeCoin}";
        SoundManager.instance.BGmusicSource.PlayOneShot(SoundManager.instance.Bgmusic);
    }
    public void StartGame()
    {
       if(SaveLoadManager.instance.currentLives > 0)
       {
            int currentlevel = SaveLoadManager.instance.GetLevel();
            SceneManager.LoadScene(currentlevel);
       }
       else
       {
           InformationAboutLife.SetActive(true);
       }
       
    }
    
    public void OnApplicationQuit()
    {
        Application.Quit();
    }
    private void Update()
    {

        CoinUi.text = $"{SaveLoadManager.instance.GetCoin()}";
        if(SaveLoadManager.instance.currentLives >0)
        {
            InformationAboutLife.SetActive(false);
        }
        if (SoundManager.instance.Active)
        {
            SoundSprite.sprite = SoundOn;
        }
        else
        {
            SoundSprite.sprite = SoundOff;
        }
    }
    public void SoundChange()
    {
       SoundManager.instance.Active = !SoundManager.instance.Active;
        if (SoundManager.instance.Active)
        {
            SoundManager.instance.BGmusicSource.volume = 0.4f;
            SoundManager.instance.MainPlayingSource.volume = 1;
           
        }
        else
        {
            SoundManager.instance.BGmusicSource.volume = 0;
            SoundManager.instance.MainPlayingSource.volume = 0;
            
        }
       
        
    }

    public void FullLife()
    {
        int coin = SaveLoadManager.instance.GetCoin();  
        if(coin >= FullLifeCoin)
        {
            coin -= FullLifeCoin;
            SaveLoadManager.instance.StoreCoin(coin);
            SaveLoadManager.instance.AddLife(5);
        }
    }

    public void OneLife()
    {
        int coin = SaveLoadManager.instance.GetCoin();
        if (coin >= OneLifeCoin)
        {
            coin -= OneLifeCoin;
            SaveLoadManager.instance.StoreCoin(coin);
            SaveLoadManager.instance.AddLife(1);
        }
    }

    public void ClosePanel()
    {
        InformationAboutLife.SetActive(false);
    }
    
}
