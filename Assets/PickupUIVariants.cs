using Unity.VisualScripting;
using UnityEngine;

public class PickupUIVariants : MonoBehaviour
{
    public Sprite interactSprite;
    public Sprite stealSprite;
    public Sprite endSprite;
    public Sprite Downstairs;
    private PickupUIVariants instance;
    private SpriteRenderer SpriteRenderer;


    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeUI(string str)
    {
        if (str == "canSteal")
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = stealSprite;
        }
        else if(str =="pickupable")
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = interactSprite;
        }
        else if (str =="END")
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = endSprite;
        }
        else if (str =="Downstairs")
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = Downstairs;
        }
        
    }
}
