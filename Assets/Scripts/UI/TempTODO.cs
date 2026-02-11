using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
using DG.Tweening;
using System.Collections;
public class TempTODO : MonoBehaviour
{

    public Image objectiveList;
    public List<Sprite> checkedListImages;
    public List<InventoryItem> currentInventory = new List<InventoryItem>();
    public GameObject notif;

    private void OnEnable()
    {
        InventoryManager.OnInventoryChange += checkObjective;

    }
    private void OnDisable()
    {
        InventoryManager.OnInventoryChange -= checkObjective;

    }
    private void Start()
    {
        notif.transform.localScale=new Vector3(0, 0, 0);
    }
    public void checkObjective(List<InventoryItem> inventory)

    {

        if (inventory.Count >= 1)
        {
            foreach (InventoryItem item in inventory)
            {
                if (item.itemData.itemName == "Snacks")
                {
                    objectiveList.sprite = checkedListImages[0];
                    StartCoroutine(animateNotif());
                }
                else if (item.itemData.loreItem)
                {

                    objectiveList.sprite = checkedListImages[1];

                }
                else if (item.itemData.loreItem&&item.itemData.itemName=="Snacks")
                {

                    objectiveList.sprite = checkedListImages[2];
                    StartCoroutine(animateNotif());

                }
            }
        }
        else
        {
            return;
        }

    }
    private IEnumerator animateNotif()
    {
        notif.SetActive(true);

        notif.transform.DOScale(new Vector3(.4f, .4f, .4f), .5f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(1f);


    }

}
