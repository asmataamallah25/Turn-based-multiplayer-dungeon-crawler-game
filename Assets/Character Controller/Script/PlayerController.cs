using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class PlayerController : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float Speed = 450;
    public bool RotateToDirection = true;
    public bool RotateWithMouseClick = false;

    [Header("Jump Settings")]
    public float JumpPower = 22;
    public float Gravity = 6;
    public int AirJumps = 1;
    public LayerMask groundLayer;

    [Header("Dash Settings")]
    public float DashPower = 3;
    public float DashDuration = 0.20f;
    public float DashCooldown = 0.5f;
    public bool AirDash = true;

    private bool canMove = true;
    private bool canDash = true;
    private float moveDirection;
    private int currentJumps = 0;
    private Rigidbody2D rb;
    private BoxCollider2D col;

    void Start()
    {
        // Initialize components and setup gravity
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        rb.gravityScale = Gravity;
    }

    void Update()
    {
        // Ensure only the owner can control this player object over the network
        if (!IsOwner) return;

        moveDirection = Input.GetAxisRaw("Horizontal");
        
        RotateToMoveDirection();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RotateToMouse();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (moveDirection != 0 && canDash)
            {
                if (!AirDash && !InTheGround()) return;
                StartCoroutine(Dash());
            }
        }
    }

    void FixedUpdate()
    {
        // Keep physics movement synced for the owner
        if (!IsOwner) return;
        Move();
    }

    void Move()
    {
        if (canMove)
        {
            rb.linearVelocity = new Vector2(moveDirection * Speed * Time.fixedDeltaTime, rb.linearVelocity.y);
        }
    }

    private bool InTheGround()
    {
        // Check for ground collision to enable jumping
        RaycastHit2D ray;
        Vector2 position = new Vector2(col.bounds.center.x, col.bounds.min.y);
        ray = Physics2D.Raycast(position, Vector2.down, col.bounds.extents.y + 0.2f, groundLayer);
        return ray.collider != null;
    }

    void Jump()
    {
        if (InTheGround())
        {
            rb.linearVelocity = Vector2.up * JumpPower;
        }
        else if (currentJumps < AirJumps)
        {
            currentJumps++;
            rb.linearVelocity = Vector2.up * JumpPower;
        }
    }

    void RotateToMoveDirection()
    {
        if (!RotateToDirection) return;
        if (moveDirection != 0 && canMove)
        {
            transform.rotation = (moveDirection == 1) ? Quaternion.identity : Quaternion.Euler(0, 180, 0);
        }
    }

    IEnumerator Dash()
    {
        // Temporarily boost speed and remove gravity for the dash effect
        canDash = false;
        float originalSpeed = Speed;
        Speed *= DashPower;
        rb.gravityScale = 0f;
        
        yield return new WaitForSeconds(DashDuration);

        rb.gravityScale = Gravity;
        Speed = originalSpeed;
        
        yield return new WaitForSeconds(DashCooldown - DashDuration);
        canDash = true;
    }

    void RotateToMouse()
    {
        if (!RotateWithMouseClick || Camera.main == null) return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.rotation = (mousePos.x < transform.position.x) ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Reset jump counter when touching ground
        if (collision.collider.CompareTag("Ground"))
        {
            currentJumps = 0;
        }
    }
}