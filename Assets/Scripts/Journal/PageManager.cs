using System.IO;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

//[CreateAssetMenu(fileName = "item", menuName = "Scriptable Objects/Item")]
public class PageManager : MonoBehaviour //ScriptableObject
{
    [SerializeField] private List<GameObject> collidingObjects = new List<GameObject>();
    [SerializeField] private List<GameObject> StoredObjects = new List<GameObject>();


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "CollageItem")
        {
            if (!collidingObjects.Contains(other.gameObject))
            collidingObjects.Add(other.gameObject);
            other.gameObject.GetComponent<FinalDragScript>().parentname = this.gameObject.name.ToString();
            //bug
            foreach (GameObject fart in collidingObjects)
            {
                Debug.Log(fart.name.ToString());
            }
            //this might need to assign a parent to the object, im trying this here
            //bug

        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "CollageItem")
        {
            if (collidingObjects.Contains(other.gameObject))
                collidingObjects.Remove(other.gameObject);

            
        }
    }

    public void ParentItems()
    {
        foreach (GameObject item in collidingObjects)
        {
            item.transform.SetParent(gameObject.transform);

            //Vector3 size = item.transform.localScale;
            //Vector3 location = item.transform.position;
            //Quaternion rotation = item.transform.localRotation;
            //needs to save the objects size, rotation, and location as a tuple??
            //then that goes into a new list

        }
    }
    public void UnParentItems()
    {
        
        foreach (GameObject item in collidingObjects)
        {
            Vector3 worldPosition = item.transform.position;
            //so funny but this is finding the parent of the page (the rotation point) and then finding its parent (the journal) and assigning the object as a child of that
            item.transform.SetParent(gameObject.transform.parent.transform.parent.transform);
            item.transform.position = worldPosition;
        }
    }

}
