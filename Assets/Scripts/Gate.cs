using System;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public bool isPlayer1Gate;
    public Transform spawnPoint;
    public Vector3 startPosition;

    public ParticleSystem gateParticle;

    public static event Action<bool> OnPlayerScored;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Ball")) return;

        if (gateParticle != null)
        {
            Vector3 particlePosition = gateParticle.transform.position;
            gateParticle.transform.position = new Vector2(particlePosition.x, collision.transform.position.y);
            gateParticle.Play();
        }

        Destroy(collision.gameObject);

        // Если мяч попал в ворота первого игрока — очко получает второй игрок.
        OnPlayerScored?.Invoke(isPlayer1Gate);
    }
}
