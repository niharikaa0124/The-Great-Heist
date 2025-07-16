using UnityEngine;
using TMPro;

public class Mode3PlayerThief : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Animator animator;
    private Vector2 movement;
    private Rigidbody2D rb;

    public TileTypeChecker tileChecker;
    public GameObject escapePanel;
    public TextMeshProUGUI escapeText;
    public GameObject caughtPanel;
    public TextMeshProUGUI caughtText;
    public AudioSource winningBeatAudio;
    public AudioSource winAudio;

    private bool hasEscaped = false;

    void Start()
    {
        string mode = PlayerPrefs.GetString("GameMode");

        //// Disable this script if not 2-Player mode
        //if (mode != "TwoPlayers")
        //{
        //    this.enabled = false;
        //    return;
        //}
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (tileChecker == null)
            tileChecker = FindObjectOfType<TileTypeChecker>();
    }

    void Update()
    {
        movement.x = 0;
        movement.y = 0;

        if (Input.GetKey(KeyCode.UpArrow)) movement.y = 1;
        if (Input.GetKey(KeyCode.DownArrow)) movement.y = -1;
        if (Input.GetKey(KeyCode.LeftArrow)) movement.x = -1;
        if (Input.GetKey(KeyCode.RightArrow)) movement.x = 1;

        movement.Normalize();

        movement.Normalize();

        if (animator != null)
        {
            animator.SetFloat("moveX", movement.x);
            animator.SetFloat("moveY", movement.y);
            animator.SetBool("isMoving", movement.sqrMagnitude > 0);
        }

        if (movement.x != 0)
            GetComponent<SpriteRenderer>().flipX = movement.x < 0;
    }


    void FixedUpdate()
    {
        if (hasEscaped || movement == Vector2.zero || tileChecker == null) return;

        Vector2 targetPos = rb.position + movement * moveSpeed * Time.fixedDeltaTime;
        Vector3Int cell = tileChecker.walkableTilemap.WorldToCell(targetPos);

        if (tileChecker.IsWalkable(cell))
        {
            rb.MovePosition(targetPos);

            if (tileChecker.endlineTilemap != null && tileChecker.endlineTilemap.HasTile(cell))
            {
                ShowEscape();
            }
        }
        else
        {
            Debug.Log("🚫 Thief blocked by obstacle.");
        }
    }

    void ShowEscape()
    {
        if (hasEscaped) return;
        hasEscaped = true;

        if (BackgroundMusicManager.Instance != null)
        {
            BackgroundMusicManager.Instance.StopMusic();
        }

        if (winningBeatAudio != null) winningBeatAudio.Play();
        if (winAudio != null) winAudio.Play();
        
        if (escapePanel != null)
        {
            escapePanel.SetActive(true);
            if (escapeText != null)
            {
                escapeText.text = "Thief Escaped!";
            }
        }

        Time.timeScale = 0f;
        Debug.Log("2P Mode Thief Escaped!");
    }
    public void TriggerCaught()
    {
        if (hasEscaped) return; // Ignore if thief already escaped
        hasEscaped = true;

        if (BackgroundMusicManager.Instance != null)
        {
            BackgroundMusicManager.Instance.StopMusic();
        }

        if (winningBeatAudio != null) winningBeatAudio.Play();
        if (winAudio != null) winAudio.Play();

        if (caughtPanel != null)
        {
            caughtPanel.SetActive(true);
            if (caughtText != null)
            {
                caughtText.text = "Game Over.\nDetective Caught the Thieff!";
            }
        }

        Time.timeScale = 0f;
        Debug.Log("2P Mode Thief Caught!");
    }

}
