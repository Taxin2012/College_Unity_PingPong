using UnityEngine;
using UnityEngine.InputSystem;

public class Paddle : MonoBehaviour
{
    public bool isPlayer1;
    public bool isAI;
    public Transform ballTransform;
    public float speed = 8f;
    public Vector3 StartPosition;

    private Rigidbody2D rb;
    private float movement;
    private PlayersInputs inputActions;

    private float topLimit = 4.5f;
    private float bottomLimit = -4.5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Режим игры выбирается в меню. ИИ управляет только второй ракеткой.
        if (!isPlayer1)
            isAI = GameSettings.Mode == GameMode.VsAI;
        else
            isAI = false;

        if (!isAI)
            inputActions = new PlayersInputs();

        if (speed <= 0f)
            speed = 8f;

        // ИИ не должен двигаться быстрее игрока.
        if (isAI && speed > 8f)
            speed = 8f;
    }

    private void OnEnable()
    {
        GameManager.OnGameOver += OnGameOver;
        GameManager.OnBallSpawned += OnBallSpawned;

        if (isAI) return;

        inputActions.Enable();

        if (isPlayer1)
        {
            inputActions.Player1.Move.performed += OnInputPerformed;
            inputActions.Player1.Move.canceled += OnInputCanceled;
        }
        else
        {
            inputActions.Player2.Move.performed += OnInputPerformed;
            inputActions.Player2.Move.canceled += OnInputCanceled;
        }
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= OnGameOver;
        GameManager.OnBallSpawned -= OnBallSpawned;

        if (isAI) return;

        if (isPlayer1)
        {
            inputActions.Player1.Move.performed -= OnInputPerformed;
            inputActions.Player1.Move.canceled -= OnInputCanceled;
        }
        else
        {
            inputActions.Player2.Move.performed -= OnInputPerformed;
            inputActions.Player2.Move.canceled -= OnInputCanceled;
        }

        inputActions.Disable();
        movement = 0f;
    }

    private void OnInputPerformed(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<float>();
    }

    private void OnInputCanceled(InputAction.CallbackContext context)
    {
        movement = 0f;
    }

    private void OnGameOver()
    {
        enabled = false;
    }

    private void OnBallSpawned(Transform newBallTransform)
    {
        ballTransform = newBallTransform;
    }

    private void Start()
    {
        StartPosition = transform.position;

        // Автоматический расчёт ограничений по стенам.
        GameObject topWall = GameObject.FindWithTag("TopWall");
        GameObject bottomWall = GameObject.FindWithTag("BottomWall");
        BoxCollider2D paddleCollider = GetComponent<BoxCollider2D>();

        if (topWall != null && bottomWall != null)
        {
            float halfPaddleHeight = paddleCollider != null ? paddleCollider.bounds.size.y / 2f : 0.5f;

            Collider2D topCollider = topWall.GetComponent<Collider2D>();
            Collider2D bottomCollider = bottomWall.GetComponent<Collider2D>();

            float topEdge = topCollider != null ? topCollider.bounds.min.y : topWall.transform.position.y;
            float bottomEdge = bottomCollider != null ? bottomCollider.bounds.max.y : bottomWall.transform.position.y;

            topLimit = topEdge - halfPaddleHeight;
            bottomLimit = bottomEdge + halfPaddleHeight;
        }
    }

    private void Update()
    {
        if (isAI || rb == null) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, movement * speed);
    }

    private void FixedUpdate()
    {
        if (!isAI || rb == null || ballTransform == null) return;

        float targetY = ballTransform.position.y;
        float newY = Mathf.MoveTowards(transform.position.y, targetY, speed * Time.fixedDeltaTime);
        rb.MovePosition(new Vector2(transform.position.x, newY));
    }

    private void LateUpdate()
    {
        float clampedY = Mathf.Clamp(transform.position.y, bottomLimit, topLimit);
        transform.position = new Vector3(transform.position.x, clampedY, transform.position.z);
    }

    public void ResetPaddle()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        movement = 0f;
        transform.position = StartPosition;
    }

    public void Reset()
    {
        ResetPaddle();
    }
}
