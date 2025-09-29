using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class HoldAllItems : MonoBehaviour
{
    public static HoldAllItems instance { get; private set; }

    public List<GameObject> Hideitems = new List<GameObject>();
    public List<GameObject> ObjectList = new List<GameObject>();
    public GameObject Parent;
    public bool miniMapOpen= false;
    public GameObject Vfx; // Particle effect prefab

    [Header("Animation Settings")]
    public float shrinkDuration = 0.5f; // Duration of shrink animation
    public Ease shrinkEase = Ease.InBack; // Easing type (adjustable in Inspector)

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    private  void Start()
    {
        //Hideitems.Clear(); 
       // ObjectList.Clear();
        
       
    }
    private void Update()
    {
        foreach (var item in ObjectList)
        {
            if (!LevelCompleteCoinAnimation.instance.AnimalSprite.Contains(item))
            {
                LevelCompleteCoinAnimation.instance.AnimalSprite.Add(item);
            }
          
        }
        Hideitems.RemoveAll(obj => obj == null || obj.activeInHierarchy == false);
    }
    public void ShowItem(GameObject obj)
    {
        SoundManager.instance.MainPlayingSource.PlayOneShot(SoundManager.instance.Spot);
        GameObject obj1 = Instantiate(Vfx, obj.transform.localPosition , Quaternion.identity);
        obj1.transform.SetParent(Parent.transform, false);
        // Play scale-down animation before destroying
        var showitem = obj.GetComponent<ShowItem>();
        if (showitem == null) return; // Ensure obj has ShowItem component

        // Create a temporary list to store items to remove
        List<GameObject> itemsToRemove = new List<GameObject>();

        foreach (var item in ObjectList)
        {
            var itemComponent = item.GetComponent<ShowItem>();
            if (itemComponent != null && itemComponent.thisitem == showitem.thisitem)
            {
                item.transform.GetChild(1).gameObject.SetActive(true);
                itemsToRemove.Add(item); // Store items for later removal
            }
        }

        // Remove items safely after iteration
        foreach (var item in itemsToRemove)
        {
            ObjectList.Remove(item);
        }

        // Play shrink animation
        obj.transform.DOScale(Vector3.zero, shrinkDuration)
            .SetEase(shrinkEase)
            .OnComplete(() =>
            {
               
                Destroy(obj);
            });
    }

}
