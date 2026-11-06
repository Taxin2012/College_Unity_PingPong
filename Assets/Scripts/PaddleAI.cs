using UnityEngine;

public class PaddleAI : MonoBehaviour
{
    [SerializeField] private Transform ballTransform;

    [SerializeField] private float speed;        

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (ballTransform == null)
        {
            GameObject ball = GameObject.FindWithTag("Ball");
            if (ball != null) ballTransform = ball.transform;
        }
    }

    void FixedUpdate()
    {
        if (ballTransform == null) return;

        Vector2 newVelocity = new Vector2(0f, (ballTransform.position.y > transform.position.y + 0.1f ? 1f : -1f) * speed);
        rb.linearVelocity = newVelocity;
    }
}