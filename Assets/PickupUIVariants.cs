using Unity.VisualScripting;
using UnityEngine;

public class PickupUIVariants : MonoBehaviour
{
    public Sprite interactSprite;
    public Sprite stealSprite;
    private PickupUIVariants instance;
    private SpriteRenderer SpriteRenderer;

    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeUI(bool stealable)
    {
        if (stealable)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = stealSprite;
        }
        else
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = interactSprite;
        }
    }
}
