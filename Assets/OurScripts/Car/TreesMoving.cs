using UnityEngine;

public class TreesMoving : MonoBehaviour
{
    private float speed = 80f;
    private void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime);
    }
    private void OnTriggerExit(Collider collision)
    {
        if(collision.CompareTag("areaCar"))
        {
            Destroy(gameObject);
        }
    }
}
