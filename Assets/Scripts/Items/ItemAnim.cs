using UnityEngine;

public class ItemAnim : MonoBehaviour
{
    public float bounceHeight = 0.5f; 
    public float bounceSpeed = 2f; 
    public float rotateSpeed = 90f;

    private float originalY;

    void Start()
    {
        originalY = transform.position.y;
    }

    void Update()
    {
        // Bounce the item up and down
        float newY = originalY + Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Rotate the item
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }
}