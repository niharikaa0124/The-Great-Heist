using TMPro;
using UnityEngine;

public class Mode1ThiefController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;

    public TileTypeChecker tileChecker;
    public GameObject escapePanel;
    public TextMeshProUGUI escapeText;
    public AudioSource WinningBeatAudio;
    public AudioSource Winaudio;
    public AudioSource LosingAudio;
    public GameObject moveTextGO;
    public GameObject highScoreTextGO;

    private bool victoryShown = false;

    void Start()
    {
        string mode = PlayerPrefs.GetString("GameMode");

        //if (mode != "ThiefVsAIDetective")
        //{
        //    this.enabled = false;
        //    return;
        //}
        if (mode == "ThiefVsAIDetective")
        {
            if (moveTextGO != null)
                moveTextGO.SetActive(false);

            if (highScoreTextGO != null)
                highScoreTextGO.SetActive(false);
        }

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (tileChecker == null)
            tileChecker = FindObjectOfType<TileTypeChecker>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize(); // Prevent diagonal boost

        if (animator != null)
        {
            animator.SetFloat("moveX", movement.x);
            animator.SetFloat("moveY", movement.y);
            animator.SetBool("isMoving", movement.sqrMagnitude > 0);
        }

        if (movement.x != 0)
        {
            GetComponent<SpriteRenderer>().flipX = movement.x < 0;
        }
    }

    void FixedUpdate()
    {
        if (movement == Vector2.zero || tileChecker == null || victoryShown) return;

        Vector2 targetPos = rb.position + movement * moveSpeed * Time.fixedDeltaTime;
        Vector3Int cell = tileChecker.walkableTilemap.WorldToCell(targetPos);

        if (tileChecker.IsWalkable(cell))
        {
            rb.MovePosition(targetPos);

            // ✅ After moving, check endline at new target cell
            if (tileChecker.endlineTilemap != null && tileChecker.endlineTilemap.HasTile(cell))
            {
                ShowVictory();
            }
        }
        else
        {
            Debug.Log("🚫 Thief blocked by obstacle.");
        }
    }

    void ShowVictory()
    {
        if (victoryShown) return;
        victoryShown = true;

        if (BackgroundMusicManager.Instance != null)
        {
            BackgroundMusicManager.Instance.StopMusic();
        }

        if (WinningBeatAudio != null) WinningBeatAudio.Play();
        if (Winaudio != null) Winaudio.Play();

        if (escapePanel != null)
        {
            escapePanel.SetActive(true);
            if (escapeText != null)
                escapeText.text = "You Escaped!! Well Played!";
        }

        Time.timeScale = 0f;
        Debug.Log("🎉 Player Thief Escaped!");
    }

    //void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (victoryShown) return;

    //    if (collision.gameObject.CompareTag("Detective"))
    //    {
    //        victoryShown = true;
    //        movement = Vector2.zero;
    //        rb.linearVelocity = Vector2.zero;
    //        rb.bodyType = RigidbodyType2D.Kinematic;

    //        if (BackgroundMusicManager.Instance != null)
    //        {
    //            BackgroundMusicManager.Instance.StopMusic();
    //        }

    //        if (LosingAudio != null) LosingAudio.Play(); // 🔊 Losing sound

    //        if (escapePanel != null)
    //        {
    //            escapePanel.SetActive(true);
    //            if (escapeText != null)
    //                escapeText.text = "You Got Caught! Try Again!";
    //        }

    //        Time.timeScale = 0f;
    //        Debug.Log("💀 Player Thief Caught!");
    //    }
    //}
}
