using UnityEditor.ShaderGraph;
using UnityEngine;

public class RicochettingBullet : MonoBehaviour
{
    public float bulletSpeed = 8f;
    public int damage = 10;

    public int maxRicochet = 10;
    public float minBounceAngle = 10f; // Minimum angle in degrees for a valid bounce
    public float maxBounceAngle = 45f; // Maximum angle in degrees for a valid bounce

    private Rigidbody2D rb2d;
    private int ricochetCount = 0;
    public bool isActive = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isActive = true;
        rb2d = GetComponent<Rigidbody2D>();
        float angle = Random.Range(0f, 360f);
        Vector2 direction = convertAngleToVector(angle);
        rb2d.linearVelocity = direction * bulletSpeed;
    }


    Vector2 convertAngleToVector(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Arena"))
        {
            Bounce(collision.contacts[0].normal);
        }
    }
    void Bounce(Vector2 normal)
    {
        ricochetCount++;
        Vector2 tangent = new Vector2(-normal.y, normal.x);
        float side = Random.value < 0.5f ? -1f : 1f;
        float angle = Random.Range(minBounceAngle, maxBounceAngle);
        float radian = angle * Mathf.Deg2Rad;
        Vector2 newDirection = (Mathf.Cos(radian) * normal + Mathf.Sin(radian) * tangent * side).normalized; // not really sure what happening with the maths but it works?
        rb2d.linearVelocity = newDirection * bulletSpeed;
        if (ricochetCount >= maxRicochet)
        {
            isActive = false;
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerHealth.Instance.TakeDamage(damage);
        }
    }
}
