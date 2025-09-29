using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    
    public LayerMask ObjectMissLayer;
    public LayerMask ObjectUnderSomeThing;
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse click
        {
            Camera selectedCamera = Camera.main;

            if (selectedCamera != null)
            {
                Ray ray = selectedCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if(!HoldAllItems.instance.miniMapOpen)
                {
                    if(Physics.Raycast(ray, out hit, Mathf.Infinity, ObjectUnderSomeThing))
                    {
                        print(hit.collider.gameObject.name);
                        var obj = hit.collider.gameObject;
                        if (obj != null)
                        {
                            obj.gameObject.SetActive(false);
                        }
                    }
                    else if (Physics.Raycast(ray, out hit, Mathf.Infinity, ObjectMissLayer))
                    {
                        print(hit.collider.gameObject.name);
                        var obj = hit.collider.gameObject;
                        if (obj != null)
                        {

                            HoldAllItems.instance.ShowItem(obj);
                        }
                    }


                }
                
               
            }
        }
        
    }

}
