using UnityEngine;
using System.Collections;
public class AttackManipulation : MonoBehaviour
{
    public GameObject attack;

    public bool rotating = false;
    public float numberOfRotations = 5f;
    public float rotationDeceleration = 10f;

    public bool scaling = false;
    public float scaleDuration = 1f;
    public float scaleCentre = 1f;
    public float scaleAmplitude = 0.5f;
    public float scaleFrequency = 4f;
    private float phase = 0f;
    public Vector3 startScale;
    public bool positioning = false;
    public bool positionDirection = false; // true for horizontal, false for vertical
    public Vector3 positionCentre;
    public Vector3 positionAxisHori = Vector3.right;
    public Vector3 positionAxisVert = Vector3.up;
    public float positionAmplitude = 2f;
    public float positionFrequency = 3f;

    public int damageAmount = 10;
    public float positionDuration = 3f;
    
    public float activeDuration = 1f; // Duration for which the attack is active
    public bool attackComplete = false;
    private SpriteRenderer spriteRenderer;
    private Collider2D Collider2D;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        startScale = transform.localScale;
        positionCentre = transform.localPosition;
    }
    public void ActivateAttack()
    {
        gameObject.SetActive(true);
        spriteRenderer = GetComponent<SpriteRenderer>();
        Collider2D = GetComponent<Collider2D>();
        StartCoroutine(initialiseModification());
        attackComplete = false;
    }

    IEnumerator initialiseModification()
    {
        Collider2D.enabled = false;
        float elapsedTime = 0f;
        float maxDuration = Mathf.Max(
            rotating? rotationDeceleration : 0f,
            scaling? scaleDuration : 0f,
            positioning? positionDuration : 0f
            );
        
        

        while (elapsedTime < maxDuration)
        {
            elapsedTime += Time.deltaTime;
            if (rotating)
            {
                float rotation = 360f * numberOfRotations;
                float decimalModifier = Mathf.Clamp01(elapsedTime / rotationDeceleration);
                float currentSpeed = Mathf.Lerp(rotation, 0f, GradualRotationSlow(decimalModifier));
                transform.Rotate(0, 0, currentSpeed * Time.deltaTime);

            }
            if (scaling)
            {
                float decimalModifier = Mathf.Clamp01(elapsedTime / scaleDuration);
                float currentFrequency = Mathf.Lerp(scaleFrequency, 0f, GradualRotationSlow(decimalModifier)); // Exponential decay for smoother scaling
                phase += currentFrequency * Time.deltaTime;
                float pulse = Mathf.Abs(Mathf.Sin(phase * Mathf.PI * 2));
                float scaleValue = 1f + pulse * scaleAmplitude;
                transform.localScale = startScale * scaleValue;
            }
            if (positioning)
            {
                float decimalModifier = Mathf.Clamp01(elapsedTime / positionDuration);
                float currentFrequency = Mathf.Lerp(positionFrequency, 0f, GradualRotationSlow(decimalModifier)); // Exponential decay for smoother positioning
                phase += currentFrequency * Time.deltaTime;
                float pulse = Mathf.Sin(phase * Mathf.PI * 2);
                float positionValue = pulse * positionAmplitude;
                Vector3 offset = (positionDirection ? positionAxisHori : positionAxisVert) * positionValue;
                transform.position = positionCentre + offset;
            }
            yield return null;
        }
        Collider2D.enabled = true;
        yield return new WaitForSeconds(activeDuration);
        Collider2D.enabled = false;
        attackComplete = true;
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    float GradualRotationSlow(float value)
    {
        return 1f - (1f - value) * (1f - value);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth.Instance.TakeDamage(damageAmount);
            Debug.Log($"Player hit for {damageAmount} damage!");
        }
    }
}
