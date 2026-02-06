using UnityEngine;
using System.Collections.Generic;

public class HintUI : MonoBehaviour
{
    [Header("Add your sprites here")]
    [SerializeField] private List<Sprite> sprites = new List<Sprite>();

    public List<Sprite> Sprites => sprites;

    private void Start()
    {
        // Option 1: Replace the entire list reference
        UIManager.hintsprites = sprites.ToArray();

    }
}
