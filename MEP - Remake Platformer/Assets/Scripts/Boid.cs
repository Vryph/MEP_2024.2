using UnityEngine;

public class Boid : MonoBehaviour
{
    [SerializeField] public Vector3 velocity;
    [SerializeField] public float maxVelocity = 2.5f;
    public Vector3 acceleration = Vector3.right;
    public float id, group;
    public SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        velocity.x = Random.Range(-1f, 1f);
        velocity.y = Random.Range(-1f, 1f);
    }
    void FixedUpdate()
    {
        velocity += acceleration * Time.fixedDeltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxVelocity);
        acceleration = Vector3.zero;
        transform.position += velocity * Time.fixedDeltaTime;


        if (velocity.sqrMagnitude > 0.01f)  //Rotates the Sprite 
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;  
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);  
        }
    }
}
