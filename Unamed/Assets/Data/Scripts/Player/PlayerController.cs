using UnityEngine.InputSystem;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    private Animator anim;
    private Rigidbody2D rb;
    private InputManager inputManager;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Movement Settings")]
    [SerializeField, Range(0f, 100f)] private float maxSpeed = 4f;
    [SerializeField, Range(0f, 100f)] private float moveSpeed = 5f;
    [SerializeField, Range(0f, 100f)] private float maxAcceleration = 35f;

    private float maxSpeedChange;
    private Vector2 currentVelocity;
    private Vector2 desiredVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        inputManager = GetComponent<InputManager>();
    }

    private void Update()
    {
        if (inputManager.MovementInput.x > 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (inputManager.MovementInput.x < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        float cappedSpeed = Mathf.Min(moveSpeed, maxSpeed);
        desiredVelocity = inputManager.MovementInput.normalized * cappedSpeed;

        desiredVelocity = new Vector2(inputManager.MovementInput.x, inputManager.MovementInput.y) * Mathf.Max(cappedSpeed, 0f);
        anim.SetFloat("DirectionX", Mathf.Abs(desiredVelocity.x), 0.1f, Time.deltaTime);
        anim.SetFloat("DirectionY", Mathf.Abs(desiredVelocity.y), 0.1f, Time.deltaTime);
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        currentVelocity = rb.linearVelocity;
        maxSpeedChange = maxAcceleration * Time.fixedDeltaTime;

        currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, desiredVelocity.x, maxSpeedChange);
        currentVelocity.y = Mathf.MoveTowards(currentVelocity.y, desiredVelocity.y, maxSpeedChange);

        rb.linearVelocity = currentVelocity;
    }
}
