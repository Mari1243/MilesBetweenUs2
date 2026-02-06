using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

[System.Serializable]
public class JournalSave : MonoBehaviour
{
    [SerializeField] FinalDragScript DraggableItemPrefab;
    [SerializeField] public static List<FinalDragScript> items = new List<FinalDragScript>();
    const string journalSub = "/journal_";
    const string journal_CountSub = "/journal.count";

    private Dictionary<string, Sprite[]> spriteSheetCache = new Dictionary<string, Sprite[]>();

    void Awake()
    {
        LoadItems();
    }

    void OnApplicationQuit()
    {
        SaveJournalItems();
    }

    void SaveJournalItems()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string basePath = Application.persistentDataPath + journalSub + SceneManager.GetActiveScene().buildIndex + "_";

        // Remove duplicates from the list, keeping only the last occurrence of each unique name
        RemoveDuplicateItems();

        // Delete old save files that no longer correspond to current items
        CleanupOldSaveFiles(basePath);

        print("Saving " + items.Count + " unique items");

        foreach (FinalDragScript item in items)
        {
            if (item == null) continue;

            // Use the item's name as part of the filename to ensure uniqueness
            string sanitizedName = SanitizeFileName(item.endname);
            string filePath = basePath + sanitizedName + ".dat";

            // Check if file already exists - if it does, we'll overwrite it with updated data
            if (File.Exists(filePath))
            {
                print("Overwriting existing save file for: " + item.endname);
            }

            // This will create new file or overwrite if it already exists
            FileStream stream = new FileStream(filePath, FileMode.Create);

            CollageItemData data = new CollageItemData(item);
            //print("Saving item: " + data.itemName + " at position " +data.position[0] + ", " + data.position[1] + ", " + data.position[2] + "with a sprite of " + data.spriteName);

            formatter.Serialize(stream, data);
            stream.Close();
        }
    }

    void RemoveDuplicateItems()
    {
        // Remove null items first
        items.RemoveAll(item => item == null);

        // Create a dictionary to track unique items by name
        Dictionary<string, FinalDragScript> uniqueItems = new Dictionary<string, FinalDragScript>();

        foreach (FinalDragScript item in items)
        {
            // This will keep the last occurrence if there are duplicates
            uniqueItems[item.endname] = item;
        }

        // Replace the list with only unique items
        items = uniqueItems.Values.ToList();
    }

    void CleanupOldSaveFiles(string basePath)
    {
        string saveDirectory = Application.persistentDataPath;
        string pattern = "journal_" + SceneManager.GetActiveScene().buildIndex + "_*.dat";

        // Get all existing save files for this scene
        string[] existingFiles = Directory.GetFiles(saveDirectory, pattern);

        // Create a set of current item filenames
        HashSet<string> currentFiles = new HashSet<string>();
        foreach (FinalDragScript item in items)
        {
            if (item == null) continue;
            string sanitizedName = SanitizeFileName(item.endname);
            string filePath = basePath + sanitizedName + ".dat";
            currentFiles.Add(filePath);
        }

        // Delete files that don't correspond to current items
        foreach (string file in existingFiles)
        {
            if (!currentFiles.Contains(file))
            {
                File.Delete(file);
                Debug.Log("Deleted old save file: " + file);
            }
        }
    }

    void LoadItems()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string saveDirectory = Application.persistentDataPath;
        string pattern = "journal_" + SceneManager.GetActiveScene().buildIndex + "_*.dat";

        string[] saveFiles = Directory.GetFiles(saveDirectory, pattern);

        if (saveFiles.Length == 0)
        {
            print("No save files found for scene " + SceneManager.GetActiveScene().buildIndex);
            return;
        }

        print("Loading " + saveFiles.Length + " saved items");

        foreach (string filePath in saveFiles)
        {
            if (File.Exists(filePath))
            {
                FileStream stream = new FileStream(filePath, FileMode.Open);
                CollageItemData data = formatter.Deserialize(stream) as CollageItemData;
                stream.Close();

                Vector3 position = new Vector3(data.position[0], data.position[1], data.position[2]);
                Vector3 rotation = new Vector3(data.rotation[0], data.rotation[1], data.rotation[2]);

                GameObject existingItem = GameObject.Find(data.itemName);
                if (existingItem != null)
                {
                    Destroy(existingItem);
                    print("Destroyed existing item: " + data.itemName);
                }

                if (data.parentName != null)
                {
                    var parent = GameObject.Find(data.parentName);
                    if (parent != null)
                    {
                        FinalDragScript item = Instantiate(DraggableItemPrefab, position, Quaternion.Euler(rotation));
                        item.transform.SetParent(parent.transform, worldPositionStays: true);

                        item.name = data.itemName;
                        item.endname = data.itemName;

                        // Use the cached loading method
                        Sprite mySprite = GetSpriteFromSheet(data.spriteSheetName, data.spriteName);

                        if (mySprite != null)
                        {
                            item.GetComponent<SpriteRenderer>().sprite = mySprite;
                            print("Instantiated saved item: " + data.itemName + " with sprite: " + data.spriteName);

                            // IMMEDIATELY UPDATE THE ITEM'S SPRITE VARIABLE
                            item.sprite = mySprite;
                        }
                        else
                        {
                            Debug.LogError("Sprite not found: " + data.spriteName + " in sheet: " + data.spriteSheetName);
                        }
                    }
                    else
                    {
                        print("Parent not found: " + data.parentName);
                    }
                }
                else
                {
                    print("Item was never saved with a parent and therefore cannot be instantiated.");
                }
            }
        }
    }

    // Cached sprite loading method
    Sprite GetSpriteFromSheet(string sheetName, string spriteName)
    {
        // Check if we've already loaded this sprite sheet
        if (!spriteSheetCache.ContainsKey(sheetName))
        {
            // Load it for the first time and cache it
            Sprite[] sprites = Resources.LoadAll<Sprite>(sheetName);
            spriteSheetCache[sheetName] = sprites;
            print("Loaded and cached sprite sheet: " + sheetName + " (" + sprites.Length + " sprites)");
        }

        // Find the sprite in the cached array
        Sprite[] cachedSprites = spriteSheetCache[sheetName];
        return System.Array.Find(cachedSprites, sprite => sprite.name == spriteName);
    }
    string SanitizeFileName(string name)
    {
        // Remove or replace characters that are invalid in filenames
        string invalid = new string(Path.GetInvalidFileNameChars());
        foreach (char c in invalid)
        {
            name = name.Replace(c.ToString(), "_");
        }
        return name;
    }

    public static void clearSave()
    {
        string saveDirectory = Application.persistentDataPath;

        // Get all files that match your naming pattern
        string[] journalFiles = Directory.GetFiles(saveDirectory, "journal*");

        foreach (string file in journalFiles)
        {
            File.Delete(file);
            Debug.Log("Deleted: " + file);
        }

        Debug.Log("All save files deleted!");
        items.Clear();
        print("Cleared the save!");
    }
}