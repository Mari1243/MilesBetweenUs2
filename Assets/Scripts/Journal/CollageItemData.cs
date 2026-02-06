using System;
using UnityEngine;

[System.Serializable]
public class CollageItemData
{
    public CollageItemData(FinalDragScript item)
    {
        itemName = item.endname;
        spriteName = item.sprite.name;

        // NEW: Save the sprite sheet/texture name
        if (item.sprite != null && item.sprite.texture != null)
        {
            spriteSheetName = item.sprite.texture.name;
        }
        else
        {
            spriteSheetName = "SpriteSheet"; // Fallback to your default sheet name
        }

        parentName = item.parentname;

        position = new float[]
        {
            item.endlocation.x, item.endlocation.y, item.endlocation.z
        };
        rotation = new float[]
        {
            item.endrotation.x, item.endrotation.y, item.endrotation.z
        };
    }

    public float[] position = new float[3];
    public float[] rotation = new float[3];
    public string itemName;
    public string parentName;
    public string spriteName;
    public string spriteSheetName; // ADD THIS LINE
}