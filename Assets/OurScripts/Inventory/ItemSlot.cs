using UnityEngine;
using UnityEngine.UI;
public class ItemSlot : MonoBehaviour
{
    public Image icon;


    public void ClearSlot()
    {
        icon.enabled = false;
    }

    public void DrawSlot(Item item)
    {
        print("drawing "+ item.name);
        if (item == null)
        {
            ClearSlot();
            return;
        }
        icon.enabled = true;
        icon.sprite = item.img;
        icon.preserveAspect = true;
    }
}
