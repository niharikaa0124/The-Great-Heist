using UnityEngine;
using UnityEngine.Tilemaps;

public class DetectivePlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Tile Checker")]
    public TileTypeChecker tileChecker;

    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 moveInput;
    private Vector2 lastTilePosition;
    private float tileSize = 1f;
    private bool isCaught = false;

    // Animator Parameter Names
    private const string Horizontal = "Horizontal";
    private const string Vertical = "Vertical";
    private const string LastHorizontal = "LastHorizontal";
    private const string LastVertical = "LastVertical";

    void Awake()
    {
        string mode = PlayerPrefs.GetString("GameMode");
        //if (mode != "ThiefVsAIDetective")
        //{
        //    this.enabled = false;
        //    return;
        //}
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (tileChecker == null)
        {
            Debug.LogError("TileTypeChecker not assigned!");
        }

        lastTilePosition = rb.position;
    }

    void Update()
    {
        if (isCaught) return;

        // Basic input handling
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize(); // Prevent diagonal speed boost

        // Animator parameters
        animator.SetFloat(Horizontal, moveInput.x);
        animator.SetFloat(Vertical, moveInput.y);

        if (moveInput != Vector2.zero)
        {
            animator.SetFloat(LastHorizontal, moveInput.x);
            animator.SetFloat(LastVertical, moveInput.y);
        }
    }

    void FixedUpdate()
    {
        if (isCaught || moveInput == Vector2.zero) return;

        Vector2 targetPosition = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
        Vector3Int cellPos = tileChecker.walkableTilemap.WorldToCell(targetPosition);

        if (tileChecker.IsWalkable(cellPos))
        {
            rb.MovePosition(targetPosition);

            if (Vector2.Distance(targetPosition, lastTilePosition) >= tileSize)
            {
                ScoreManager.instance?.IncreaseMove();
                lastTilePosition = targetPosition;
            }
        }
        else
        {
            Debug.Log("Blocked! Cannot move to obstacle tile.");
        }
    }

    public void StopMovement()
    {
        isCaught = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }
    //void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.CompareTag("Thief"))
    //    {
    //        Mode3PlayerThief thiefScript = other.GetComponent<Mode3PlayerThief>();
    //        if (thiefScript != null)
    //        {
    //            thiefScript.TriggerCaught();
    //        }
    //    }
    //}
}
