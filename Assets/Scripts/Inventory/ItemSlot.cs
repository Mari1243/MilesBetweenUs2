using UnityEngine;
using UnityEngine.UI;
public class ItemSlot : MonoBehaviour
{
    public Image icon;


    public void ClearSlot()
    {
        icon.enabled = false;
    }

    public void DrawSlot(InventoryItem item)
    {
        if (item == null)
        {
            ClearSlot();
            return;
        }
        icon.enabled = true;
        icon.sprite = item.itemData.img;
        icon.preserveAspect = true;
    }
}
