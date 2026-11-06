using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Эффект сжатия (Squash & Stretch)")]
    [Tooltip("Сила сжатия. Чем меньше значение, тем сильнее сплющится мяч (например, 0.6f)")]
    public float squashFactor = 0.6f;
    [Tooltip("Скорость восстановления формы в полете")]
    public float restoreSpeed = 10f;
    private Vector3 originalScale;

    [Header("Настройки движения")]
    public float speed = 10f;
    public float speedMultiplier = 1.05f;

    [Header("Настройки детекции")]
    public LayerMask paddleLayer;

    [Header("Динамический хвост (Trail)")]
    [Tooltip("Коэффициент длины хвоста. Время жизни хвоста = Скорость мяча * этот коэффициент")]
    public float trailDurationFactor = 0.02f;
    [Tooltip("Минимальное время жизни хвоста (чтобы он не исчезал совсем при старте)")]
    public float minTrailTime = 0.1f;
    [Tooltip("Максимальное время жизни хвоста (чтобы он не растягивался на весь экран)")]
    public float maxTrailTime = 0.5f;

    private float currentSpeed;
    private Vector2 direction;
    private float ballRadius;
    private TrailRenderer trail; // Ссылка на компонент хвоста

    // Переменные для автоматических стен
    private float topWallY = 5f;
    private float bottomWallY = -5f;

    void Start()
    {
        originalScale = transform.localScale;

        // Автоматически находим Trail Renderer на мяче
        trail = GetComponent<TrailRenderer>();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.useFullKinematicContacts = true;

        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider != null)
        {
            ballRadius = circleCollider.radius * Mathf.Max(transform.localScale.x, transform.localScale.y);
        }
        else
        {
            ballRadius = 0.5f;
        }

        GameObject topWall = GameObject.FindWithTag("TopWall");
        GameObject bottomWall = GameObject.FindWithTag("BottomWall");

        // Берём внутренние границы стен, чтобы мяч не заходил внутрь коллайдера.
        if (topWall != null)
        {
            Collider2D topCollider = topWall.GetComponent<Collider2D>();
            topWallY = topCollider != null ? topCollider.bounds.min.y : topWall.transform.position.y;
        }

        if (bottomWall != null)
        {
            Collider2D bottomCollider = bottomWall.GetComponent<Collider2D>();
            bottomWallY = bottomCollider != null ? bottomCollider.bounds.max.y : bottomWall.transform.position.y;
        }

        Launch();
    }

    public void Reset()
    {
        currentSpeed = speed;
        transform.position = Vector3.zero;
        transform.localScale = originalScale;

        // Очищаем старый хвост при перезапуске, чтобы он не тянулся из угла в центр
        if (trail != null) trail.Clear();

        Launch();
    }

    void Update()
    {
        float moveDistance = currentSpeed * Time.deltaTime;

        // ИСПРАВЛЕННЫЙ РАСЧЕТ ХВОСТА:
        if (trail != null)
        {
            // Считаем, во сколько раз мяч ускорился относительно стартовой скорости
            float speedRatio = currentSpeed / speed;

            // Хвост растет от минимального до максимального значения пропорционально ускорению
            // Если мяч на стартовой скорости, время жизни равно minTrailTime
            // Чем быстрее летит мяч, тем ближе хвост к maxTrailTime
            float targetTrailTime = minTrailTime + (speedRatio - 1f) * trailDurationFactor;

            trail.time = Mathf.Clamp(targetTrailTime, minTrailTime, maxTrailTime);
        }

        transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * restoreSpeed);

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, ballRadius, direction, moveDistance, paddleLayer);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            currentSpeed *= speedMultiplier;

            Collider2D paddleCollider = hit.collider;
            float hitY = (transform.position.y - paddleCollider.transform.position.y) / paddleCollider.bounds.size.y;
            float dirX = transform.position.x > paddleCollider.transform.position.x ? 1f : -1f;

            direction = new Vector2(dirX, (hitY * 2f) + Random.Range(-0.3f, 0.3f)).normalized;

            transform.localScale = new Vector3(originalScale.x * squashFactor, originalScale.y * (2f - squashFactor), originalScale.z);

            float remainingDistance = moveDistance - hit.distance;
            if (remainingDistance < 0.05f) remainingDistance = 0.05f;

            transform.position = (Vector3)hit.centroid + (Vector3)(direction * remainingDistance);
        }
        else
        {
            transform.position += (Vector3)(direction * moveDistance);
        }

        if (transform.position.y + ballRadius >= topWallY && direction.y > 0)
        {
            direction.y = -direction.y;
            transform.localScale = new Vector3(originalScale.x * (2f - squashFactor), originalScale.y * squashFactor, originalScale.z);
        }
        else if (transform.position.y - ballRadius <= bottomWallY && direction.y < 0)
        {
            direction.y = -direction.y;
            transform.localScale = new Vector3(originalScale.x * (2f - squashFactor), originalScale.y * squashFactor, originalScale.z);
        }

        if (Mathf.Abs(direction.y) < 0.2f)
        {
            float targetY = transform.position.y > 0 ? -0.5f : 0.5f;
            direction = new Vector2(direction.x, targetY).normalized;
        }
    }

    private void Launch()
    {
        currentSpeed = speed;
        float dirX = Random.Range(0, 2) == 0 ? -1f : 1f;
        float dirY = Random.Range(-0.5f, 0.5f);
        direction = new Vector2(dirX, dirY).normalized;
    }
}