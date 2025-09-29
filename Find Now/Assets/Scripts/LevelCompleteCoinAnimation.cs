using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;

public class LevelCompleteCoinAnimation : MonoBehaviour
{
    public static LevelCompleteCoinAnimation instance { get; private set; }

    public Transform coinParent;  // Parent object containing all coins (centered)
  
    public Canvas canvas;         // Reference to UI Canvas
    public int coinCount = 10;    // Number of coins to animate
    public float spawnDelay = 0.1f;  // Delay between animations
    public float moveDuration = 1f;  // Duration of movement
    public Ease ease = Ease.InOutQuad; // Easing type for smooth animation
    
    private Vector2 screenCenter;
    private Vector2 topRightCorner;
    private Vector2 BottomMiddleCorner;
    public Vector2 TopMiddleCorner = Vector2.zero;
    [Header("Sprites Animation")]
    public List<GameObject> AnimalSprite = new List<GameObject>();
    [Header("Time Animations")]
    public GameObject TimeAnimationobj;
    public RectTransform timeplace;

    public RectTransform safearea;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        AnimalSprite.Clear();
        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();

        // Calculate the center of the screen
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        screenCenter = new Vector2(0, 0); // Coins start from the center (anchored)

        // Calculate the top-right corner based on canvas size
        topRightCorner = new Vector2(safearea.rect.width / 2, safearea.rect.height / 2);
        topRightCorner += new Vector2(-112.4f, -52.79004f);
        TopMiddleCorner = new Vector2(0, safearea.rect.height / 2);
        TopMiddleCorner += new Vector2(0,timeplace.anchoredPosition.y);
    }
    public void MoveAnimalSprites()
    {
        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        foreach (GameObject sprite in AnimalSprite)
        {
            if (sprite != null)
            {
                RectTransform obj = sprite.GetComponent<RectTransform>();
                if (obj != null)
                {
                    // Calculate Bottom Middle Position
                    float bottomY = -safearea.rect.height / 2 + 200; // Offset by 165 units above bottom
                    Vector2 bottomMiddlePosition = new Vector2(obj.anchoredPosition.x, bottomY);

                    // Move sprite smoothly
                    MoveSprite(obj, bottomMiddlePosition);
                }
            }
        }
    }
    public IEnumerator OnExtendTime(int m)
    {
        if(TimeAnimationobj != null)
        {
            TimeAnimationobj.SetActive(true);

            GameObject Timetext = TimeAnimationobj.transform.GetChild(0).gameObject;
            Timetext.GetComponent<TextMeshProUGUI>().text = $"Increase Time {m}";
            RectTransform coinRect = TimeAnimationobj.GetComponent<RectTransform>();
            coinRect.anchoredPosition= new Vector2(0, 0);
            yield return new WaitForSeconds(0.5f);
            MoveCoin(coinRect,TopMiddleCorner);        
        }
    }

    
    public void OnLevelComplete()
    {
        if (coinParent != null)
        {
            StartCoroutine(MoveCoins());
         //  StartCoroutine(Reset());
        }
    }
    private IEnumerator Reset()
    {
        foreach (Transform coin in coinParent)
        {
            coin.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }

    }
    IEnumerator MoveCoins()
    {
        int movedCoins = 0;
        yield return Reset();
        foreach (Transform coin in coinParent)
        {
            if (movedCoins >= coinCount) break; // Move only the specified number of coins

            RectTransform coinRect = coin.GetComponent<RectTransform>();

            if (coinRect != null)
            {
                MoveCoin(coinRect,topRightCorner);
                movedCoins++;
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }
    void MoveSprite(RectTransform coin, Vector2 target)
    {

        // Move coin from center to the calculated top-right corner
        coin.DOAnchorPos(target, moveDuration)
            .SetEase(ease)
            .OnComplete(() =>
            {
                // Hide coin after reaching the target
            });
    }
    void MoveCoin(RectTransform coin, Vector2 target)
    {
      
        // Move coin from center to the calculated top-right corner
        coin.DOAnchorPos(target, moveDuration)
            .SetEase(ease)
            .OnComplete(() =>
            {
                coin.gameObject.SetActive(false); // Hide coin after reaching the target
            });
    }
}
