using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameGui : MonoBehaviour
{
    [Header("GamePanel")]
    public GameObject WinPanel;
    public GameObject LosePanel;
    public GameObject PausePanel;
    private bool gameover;
    public TextMeshProUGUI Leveltxt;
    [Header("Total Time")]
    public float TotalTime;
    public TextMeshProUGUI TotalTimeUi;
    public int MaxCoinForLevel;
    private float Timeleft;
    [Header("MiniMap")]
    public GameObject MiniMapCameraUi;
    public GameObject MiniMapCamera;

    private bool isDragging;
    private Vector3 lastMousePosition;
    private bool isHorizontalMovement;

    [Header("MiniMap Camera Movement")]
    public float moveSpeed = 1.0f; // Speed of movement
    public float minY = 10f; // Minimum height of the minimap camera
    public float maxY = 30f; // Maximum height of the minimap camera
    private float cameraY; // Current Y position of the minimap camera
    
    [Header("TotalCoin")]
    public TextMeshProUGUI TotalCoinUi;
    private int TotalCoin;
    
    
    [Header("Freeze Time")]
    public int FreezeCoin;
    public TextMeshProUGUI FreezeCoinUi;


    [Header("Hint")]
    public int HintCoin;
    public TextMeshProUGUI HintCoinUi;
    public float TimeforEveryHint;
    public GameObject Arrow;
    public GameObject AllObjectsHint;

    int currentobj;
    [Header("information")]
    public GameObject InformationObj;

    private void Start()
    {
        SoundManager.instance.BGmusicSource.Stop();
        InformationObj.SetActive(true);
        WinPanel.SetActive(false);
        PausePanel.SetActive(false);
        LosePanel.SetActive(false);

        SoundManager.instance.BGmusicSource.PlayOneShot(SoundManager.instance.Bgmusic);
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        SaveLoadManager.instance.SaveLevel(currentLevel);
        Leveltxt.text = $"Level {currentLevel}";
        Time.timeScale = 1f;
        Timeleft = TotalTime;
        FreezeCoinUi.text = $"{FreezeCoin}";
        HintCoinUi.text = $"{HintCoin}";
        int totalcoin = SaveLoadManager.instance.GetCoin();
        TotalCoinUi.text = $"{totalcoin}";
        cameraY = MiniMapCamera.transform.position.y; // Initialize camera Y position
    }

    private void Update()
    {

        if(!gameover )
        {
            SetTime();
        }
        if(TotalTime - Timeleft > 3f && InformationObj.activeInHierarchy)
        {
           CloseInformationWindow();
        }

        if (HoldAllItems.instance.Hideitems.Count == 0 && !gameover)
        {
          StartCoroutine(WinLevel());
        }

        if (HoldAllItems.instance.miniMapOpen)
        {
            if (Input.GetMouseButtonDown(0)) // Left-click to start dragging
            {
                isDragging = true;
                lastMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0)) // Stop dragging on release
            {
                isDragging = false;
            }

            if (isDragging)
            {
                Vector3 currentMousePosition = Input.mousePosition;
                Vector3 deltaMouse = currentMousePosition - lastMousePosition;

                isHorizontalMovement = Mathf.Abs(deltaMouse.x) > Mathf.Abs(deltaMouse.y); // Detect movement direction

                if (!isHorizontalMovement) // If vertical movement (adjusting Y position)
                {
                    cameraY += deltaMouse.y * moveSpeed * Time.deltaTime; // Move up/down
                    cameraY = Mathf.Clamp(cameraY, minY, maxY); // Clamp Y position

                    MiniMapCamera.transform.position = new Vector3(
                        MiniMapCamera.transform.position.x,
                        cameraY,
                        MiniMapCamera.transform.position.z
                    );
                }

                lastMousePosition = currentMousePosition; // Update last mouse position
            }
        }
    }
    public void CloseInformationWindow()
    {
        InformationObj.SetActive(false);
        LevelCompleteCoinAnimation.instance.MoveAnimalSprites();
        AllObjectsHint.SetActive(false);
    }
    IEnumerator HideAllObjectHintObj()
    {
        yield return new WaitForSeconds(2.5f);
        AllObjectsHint.SetActive(false);
    }
    public void CheckToOpenMap()
    {
        // Ensure SaveLoadManager is not null
        if (SaveLoadManager.instance == null)
        {
            Debug.LogError("SaveLoadManager instance is null!");
            return;
        }

        int n1 = SaveLoadManager.instance.GetCoin();

        // Ensure HoldAllItems is not null
        if (HoldAllItems.instance == null)
        {
            Debug.LogError("HoldAllItems instance is null!");
            return;
        }

        if (!HoldAllItems.instance.miniMapOpen && n1 >= HintCoin)
        {
            if (HoldAllItems.instance.Hideitems.Count > 0)
            {
                GameObject obj = null;
                int count = -1;

                // Find a valid hidden item
                for (int i = 0; i < HoldAllItems.instance.Hideitems.Count; i++)
                {
                    if (HoldAllItems.instance.Hideitems[i] != null)
                    {
                        ShowItem showItem = HoldAllItems.instance.Hideitems[i].GetComponent<ShowItem>();
                        if (showItem != null && !showItem.Hint)
                        {
                            count = i;
                            break;
                        }
                    }
                }

                if (count != -1)
                {
                    currentobj = count;
                    obj = HoldAllItems.instance.Hideitems[count];
                }
                else
                {
                    AllObjectsHint.SetActive(true);
                    StartCoroutine(HideAllObjectHintObj());
                    return;
                }

                // Deduct hint coin and update UI
                n1 -= HintCoin;
                SaveLoadManager.instance.StoreCoin(n1);
                int totalcoin = SaveLoadManager.instance.GetCoin();
                TotalCoinUi.text = $"{totalcoin}";

                // Ensure obj is not null before accessing components
                if (obj != null)
                {
                    ShowItem showItem = obj.GetComponent<ShowItem>();
                    if (showItem != null)
                    {
                        OpenMap();
                        
                    }
                    else
                    {
                        Debug.LogError($"ShowItem component is missing on {obj.name}!");
                    }
                }
            }
        }
    }


    public void OpenMap()
    {
        cameraY = minY;
        Vector3 targetPos = HoldAllItems.instance.Hideitems[currentobj].transform.position;
        MiniMapCamera.transform.position = new Vector3(targetPos.x, cameraY, targetPos.z);
        HoldAllItems.instance.miniMapOpen = true;
        MiniMapCameraUi.SetActive(true);
        HoldAllItems.instance.Hideitems[currentobj].GetComponent<ShowItem>().Hint = true;
        int totalcoin = SaveLoadManager.instance.GetCoin();
        TotalCoinUi.text = $"{totalcoin}";
    }

    public void SetTime()
    {
        Timeleft -= Time.deltaTime;

        if (Timeleft > 0)
        {
            int minutes = Mathf.FloorToInt(Timeleft / 60);  // Get minutes
            int seconds = Mathf.FloorToInt(Timeleft % 60);  // Get remaining seconds

            TotalTimeUi.text = $"Time: {minutes}:{seconds:D2}";  // Display as MM:SS
        }
        else
        {
            TotalTimeUi.text = "0:00";
            LevelLoss();
        }
    }


    public void CloseMap()
    {
        GameObject obj = Instantiate(Arrow);
        obj.transform.SetParent(HoldAllItems.instance.Parent.transform);
        GameObject obj1 = HoldAllItems.instance.Hideitems[currentobj];
        obj.transform.localPosition = obj1.transform.localPosition  + new Vector3(0,(obj1.GetComponent<BoxCollider>().bounds.size.y/2)+2f,0);
        obj.transform.SetParent(HoldAllItems.instance.Hideitems[currentobj].transform);
        HoldAllItems.instance.miniMapOpen = false;
        MiniMapCameraUi.SetActive(false);
        AllObjectsHint.SetActive(false);
    }

    public void FreezeTime1()
    {
        if (!gameover)
        {
            int n = SaveLoadManager.instance.GetCoin();
            if (n >= FreezeCoin )
            {
               
                n -= FreezeCoin;
                SaveLoadManager.instance.StoreCoin(n);
                int m = Random.Range(12, 17);
                Timeleft += m;
              StartCoroutine(LevelCompleteCoinAnimation.instance.OnExtendTime(m));
                int totalcoin = SaveLoadManager.instance.GetCoin();
                TotalCoinUi.text = $"{totalcoin}";
            }
        }
    }
    public void NextLevel()
    {
        SoundManager.instance.BGmusicSource.Stop();
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        if (currentLevel < 10) // Move to the next level
        {
            SaveLoadManager.instance.SaveLevel(currentLevel + 1);
            SceneManager.LoadScene(currentLevel + 1);
        }
        else // Return to main menu after last level
        {
            SaveLoadManager.instance.SaveLevel(1); // Reset high score to start at Level 1
            SceneManager.LoadScene(1);
        }
    }

    public void LevelLoss()
    {
        gameover = true;
        SoundManager.instance.MainPlayingSource.PlayOneShot(SoundManager.instance.Lose);
        Time.timeScale = 0f;
        LosePanel.SetActive(true);
        SaveLoadManager.instance.LoseLife();
    }

    private IEnumerator WinLevel()
    {
        gameover = true;
        yield return new WaitForSeconds(0.5f);
        SoundManager.instance.MainPlayingSource.PlayOneShot(SoundManager.instance.Win);
        LevelCompleteCoinAnimation.instance.OnLevelComplete();
        int n = SaveLoadManager.instance.GetCoin();
        n += MaxCoinForLevel;
        SaveLoadManager.instance.StoreCoin(n);
        yield return new WaitForSeconds(3.4f);
        int totalcoin = SaveLoadManager.instance.GetCoin();
        TotalCoinUi.text = $"{totalcoin}";
        //Time.timeScale = 0f;
        WinPanel.SetActive(true);
        Debug.Log("You Win!"); // Replace with actual win logic
    }

    public void RestartLevel()
    {
        if(SaveLoadManager.instance.currentLives > 0)
        {
            SoundManager.instance.BGmusicSource.Stop();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
        

    }

    public void PauseGame()
    {
        if (!gameover)
        {
            Time.timeScale = 0f;
            PausePanel.SetActive(true);
        }

    }
    public void Resume()
    {
        Time.timeScale = 1f;
        PausePanel.SetActive(false);
    }

    public void MainMenu()
    {
        if (!gameover)
        {
            SaveLoadManager.instance.LoseLife();
        }
        SoundManager.instance.BGmusicSource.Stop();
        SceneManager.LoadScene(0);
    }
}
