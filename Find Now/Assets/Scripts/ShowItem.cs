
using UnityEngine;

public class ShowItem : MonoBehaviour
{
    public enum Items
    {
        Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10,
    }
    public Items thisitem;

    public bool Hint = false;

    private void Start()
    {
        if(gameObject.GetComponent<Collider>() != null)
        {
            HoldAllItems.instance.Hideitems.Add(gameObject);
        }
        else
        {
            HoldAllItems.instance.ObjectList.Add(gameObject);
        }
    }
}
