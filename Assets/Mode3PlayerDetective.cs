using UnityEngine;

public class Mode3PlayerDetective : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Animator animator;
    private Vector2 movement;
    private Rigidbody2D rb;
    private bool isCaught = false;

    public TileTypeChecker tileChecker;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (tileChecker == null)
            tileChecker = FindObjectOfType<TileTypeChecker>();
    }

    void Update()
    {
        float horizontal = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
        float vertical = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);

        movement = new Vector2(horizontal, vertical).normalized;

        if (animator != null)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetBool("speed", movement.sqrMagnitude > 0);

            if (movement.sqrMagnitude > 0.01f)
            {
                animator.SetFloat("LastHorizontal", movement.x);
                animator.SetFloat("LastVertical", movement.y);
            }
        }

        if (movement.x != 0)
            GetComponent<SpriteRenderer>().flipX = movement.x < 0;
    }

    void FixedUpdate()
    {
        if (movement == Vector2.zero || tileChecker == null) return;

        Vector2 targetPos = rb.position + movement * moveSpeed * Time.fixedDeltaTime;
        Vector3Int cell = tileChecker.walkableTilemap.WorldToCell(targetPos);

        if (tileChecker.IsWalkable(cell))
        {
            rb.MovePosition(targetPos);

            // Manual collision check with Thief
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(targetPos, 0.2f);
            foreach (Collider2D col in hitColliders)
            {
                if (col.CompareTag("Thief"))
                {
                    Mode3PlayerThief thiefScript = col.GetComponent<Mode3PlayerThief>();

                    if (thiefScript != null)
                    {
                        Debug.Log("🎯 Thief caught manually without trigger!");

                        if (BackgroundMusicManager.Instance != null)
                            BackgroundMusicManager.Instance.StopMusic();

                        if (thiefScript.winningBeatAudio != null)
                            thiefScript.winningBeatAudio.Play();

                        if (thiefScript.winAudio != null)
                            thiefScript.winAudio.Play();

                        if (thiefScript.caughtPanel != null)
                        {
                            thiefScript.caughtPanel.SetActive(true);
                            if (thiefScript.caughtText != null)
                            {
                                thiefScript.caughtText.text = "Game Over!\nDetective Caught the Thief!";
                            }
                        }

                        Time.timeScale = 0f;
                    }

                    break;
                }
            }
        }
        else
        {
            Debug.Log("🚫 Detective blocked by obstacle.");
        }
    }

    public void StopMovement()
    {
        isCaught = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }
}
