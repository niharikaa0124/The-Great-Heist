using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using System.Linq;



public class ThiefController : MonoBehaviour
{
    public Tilemap walkableTilemap;
    public Tilemap obstacleTilemap;
    public Tilemap endlineTilemap;

    public GameObject escapePanel; // Assign in Inspector
    public TextMeshProUGUI escapeText; // Optional if you want to set text

    public Transform detectiveTransform;

    private Animator animator;
    private Vector3 previousPosition;


    public float moveSpeed = 2f;

    public TextMeshProUGUI caughtText;

    public GameObject caughtPanel; // assign in Inspector
    public AudioSource winningBeatAudio;
    public AudioSource LosingBeatAudio;
    public static BackgroundMusicManager Instance;
    public AudioSource backgroundMusic;
    public AudioSource winAudio;



    private Vector3Int currentCell;
    private bool isMoving = false;
    private Queue<Vector3> pathQueue = new Queue<Vector3>();
    private bool isCaught = false;

    private string currentMode = "";
    void Start()
    {
        currentMode = PlayerPrefs.GetString("GameMode", "DetectiveVsAIThief");
        Debug.Log("🎮 Current Game Mode: " + currentMode);

        animator = GetComponent<Animator>();
        previousPosition = transform.position;
        // Optional: force sort order higher if not set in inspector
        GetComponent<SpriteRenderer>().sortingLayerName = "Characters";
        GetComponent<SpriteRenderer>().sortingOrder = 10;
        GetComponent<BoxCollider2D>().size = new Vector2(transform.localScale.x, transform.localScale.y);

        currentCell = walkableTilemap.WorldToCell(transform.position);

        if (detectiveTransform != null)
        {
            Vector3Int thiefCell = walkableTilemap.WorldToCell(transform.position);
            float dist = Vector2.Distance(transform.position, detectiveTransform.position);

            if (dist <= 4f && GetAvailableMoves(thiefCell).Count >= 3)
            {
                RunMinimax();
            }
            else
            {
                RunDijkstra();
            }
        }
        else
        {
            Debug.LogWarning("Detective Transform not assigned. Running Dijkstra by default.");
            RunDijkstra();
        }
    }

    List<Vector3Int> GetAvailableMoves(Vector3Int cell)
    {
        List<Vector3Int> moves = new List<Vector3Int>();
        Vector3Int[] directions = {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0)
    };

        foreach (var dir in directions)
        {
            Vector3Int newPos = cell + dir;
            if (IsWalkable(newPos))
                moves.Add(newPos);
        }

        return moves;
    }

    private int stuckCounter = 0;
    private Queue<Vector3Int> recentMoves = new Queue<Vector3Int>();
    private int memorySize = 5;


    void Update()
    {
        if (isCaught || isMoving) return;

        // ✅ Plan new path if empty
        if (pathQueue.Count == 0)
        {
            float dist = Vector2.Distance(transform.position, detectiveTransform.position);
            if (dist <= 4f && GetAvailableMoves(walkableTilemap.WorldToCell(transform.position)).Count >= 3)
            {
                RunMinimax();
            }
            else
            {
                RunDijkstra();
            }
        }

        // ✅ Move to next position in path
        if (pathQueue.Count > 0 && !isMoving)
        {
            Vector3 nextPos = pathQueue.Dequeue();
            StartCoroutine(MoveTo(nextPos));
        }

        // ✅ Animation setup (based on movement direction)
        if (!isCaught)
        {
            Vector3 velocity = (transform.position - previousPosition) / Time.deltaTime;

            float moveX = velocity.x;
            float moveY = velocity.y;

            if (animator != null)
            {
                animator.SetFloat("moveX", Mathf.Clamp(moveX, -1, 1));
                animator.SetFloat("moveY", Mathf.Clamp(moveY, -1, 1));
                animator.SetBool("isMoving", velocity.magnitude > 0.01f);

                // ✅ Flip sprite only if moving more horizontally
                if (Mathf.Abs(moveX) > Mathf.Abs(moveY))
                {
                    GetComponent<SpriteRenderer>().flipX = moveX < 0;
                }
            }

            previousPosition = transform.position;

            // ✅ Maintain Z = 0
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }

        // ✅ Loop stuck detection
        if (recentMoves.Count >= 3)
        {
            var last3 = recentMoves.Skip(recentMoves.Count - 3).ToList();
            if (last3[0] == last3[2])
            {
                stuckCounter++;
                if (stuckCounter >= 3)
                {
                    Debug.Log("🚨 Thief stuck in loop! Forcing Dijkstra...");
                    RunDijkstra();
                    stuckCounter = 0;
                }
            }
            else
            {
                stuckCounter = 0;
            }
        }
    }


    void LateUpdate()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        // 🛡 Force visibility
        if (sr != null && sr.color.a < 1f)
        {
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
        }

        // 🔁 Z-force (safety)
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    private Vector3Int lastMinimaxMove;
    private Vector3Int previousCell;
    //private int stuckCounter = 0;
    public int maxStuckCount = 3;
    private Vector3Int lastThiefCell;
    private Vector3Int secondLastThiefCell;

    void RunMinimax()
    {
        Vector3Int thiefCell = walkableTilemap.WorldToCell(transform.position);
        Vector3Int detectiveCell = walkableTilemap.WorldToCell(detectiveTransform.position);

        Vector3Int[] directions = {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0)
    };

        Vector3Int bestMove = thiefCell;
        float bestScore = float.MinValue;

        foreach (var thiefMove in directions)
        {
            Vector3Int newThiefPos = thiefCell + thiefMove;
            if (!IsWalkable(newThiefPos)) continue;

            float worstScore = float.MaxValue;

            foreach (var detectiveMove in directions)
            {
                Vector3Int newDetectivePos = detectiveCell + detectiveMove;
                if (!IsWalkable(newDetectivePos)) continue;

                float distance = Vector3Int.Distance(newThiefPos, newDetectivePos);
                if (distance < worstScore)
                    worstScore = distance;
            }

            if (newThiefPos == lastThiefCell)
            {
                worstScore -= 1.5f;  // Penalize going back to same cell
            }

            if (worstScore > bestScore)
            {
                bestScore = worstScore;
                bestMove = newThiefPos;
            }
        }

        lastThiefCell = thiefCell;

        pathQueue.Clear();
        Vector3 bestMoveWorldPos = walkableTilemap.GetCellCenterWorld(bestMove);
        bestMoveWorldPos.z = 0f; // ✅ Force Z = 0 to prevent invisibility
        pathQueue.Enqueue(bestMoveWorldPos);
    }

    bool IsWalkable(Vector3Int cell)
    {
        return walkableTilemap.HasTile(cell) && !obstacleTilemap.HasTile(cell);
    }



    IEnumerator MoveTo(Vector3 destination)
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, destination) > 0.05f)
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
            transform.position = new Vector3(newPos.x, newPos.y, 0); // 👈 Force Z = 0 during movement
            yield return null;
        }

        transform.position = new Vector3(destination.x, destination.y, 0); // 👈 Final Z = 0

        currentCell = walkableTilemap.WorldToCell(transform.position);
        isMoving = false;

        // ✅ Check if reached Endline
        if (endlineTilemap.HasTile(currentCell))
        {
            isCaught = true;

            // 🔇 Stop background music if active
            if (BackgroundMusicManager.Instance != null)
            {
                BackgroundMusicManager.Instance.StopMusic();
                Debug.Log("Background music stopped due to thief escape");
            }

            string mode = PlayerPrefs.GetString("GameMode");

            if (mode == "ThiefVsAIDetective" && winAudio != null)
            {
                winAudio.Play();
                winningBeatAudio.Play();
            }
            else if (mode == "DetectiveVsAIThief" && LosingBeatAudio != null)
            {
                LosingBeatAudio.Play();
            }
            else if (mode == "2Players" && winAudio != null)
            {
                winAudio.Play();
                winningBeatAudio.Play();
            }

            if (escapePanel != null)
            {
                escapePanel.SetActive(true);

                if (escapeText != null)
                {
                    if (mode == "ThiefVsAIDetective")
                        escapeText.text = "You Won! \n Thief escaped.";
                    else if (mode == "DetectiveVsAIThief")
                        escapeText.text = "Thief Escaped! \n Better Luck Next Time!";
                    else
                        escapeText.text = "Player Escaped!";
                }

                // 🕒 Freeze game
                Time.timeScale = 0f;
                Transform resetTransform = escapePanel.transform.Find("ResetHighScoreButton");
                if (resetTransform != null)
                {
                    resetTransform.gameObject.SetActive(true);
                }
            }

            Debug.Log("Thief Escaped!");
            yield break;
        }

        recentMoves.Enqueue(currentCell);
        if (recentMoves.Count > memorySize)
        {
            recentMoves.Dequeue();
        }

        if (recentMoves.Count == memorySize && recentMoves.Distinct().Count() <= 2)
        {
            Debug.Log("⚠️ Thief stuck in loop! Forcing Dijkstra...");
            RunDijkstra();
            recentMoves.Clear();
        }

        Transform resetBtn = escapePanel.transform.Find("ResetHighScoreButton");
        if (resetBtn != null)
        {
            resetBtn.gameObject.SetActive(true);
        }

        if (!isCaught)
        {
            pathQueue.Clear();
        }
    }

    class CellCost
    {
        public int cost;
        public Vector3Int cell;

        public CellCost(int c, Vector3Int v)
        {
            cost = c;
            cell = v;
        }
    }

    void RunDijkstra()
    {
        Dictionary<Vector3Int, int> distance = new Dictionary<Vector3Int, int>();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        List<CellCost> pq = new List<CellCost>();

        distance[currentCell] = 0;
        pq.Add(new CellCost(0, currentCell));

        Vector3Int[] directions = {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0)
    };

        Vector3Int targetCell = new Vector3Int();
        bool found = false;

        while (pq.Count > 0)
        {
            pq.Sort((a, b) => a.cost.CompareTo(b.cost));
            CellCost currentNode = pq[0];
            pq.RemoveAt(0);

            Vector3Int current = currentNode.cell;
            int cost = currentNode.cost;

            if (visited.Contains(current)) continue;
            visited.Add(current);

            if (endlineTilemap.HasTile(current))
            {
                targetCell = current;
                found = true;
                break;
            }

            foreach (var dir in directions)
            {
                Vector3Int neighbor = current + dir;

                if (visited.Contains(neighbor)) continue;
                if (!walkableTilemap.HasTile(neighbor)) continue;
                if (obstacleTilemap.HasTile(neighbor)) continue;

                int newCost = cost + 1;

                if (!distance.ContainsKey(neighbor) || newCost < distance[neighbor])
                {
                    distance[neighbor] = newCost;
                    cameFrom[neighbor] = current;
                    pq.Add(new CellCost(newCost, neighbor));
                }
            }
        }

        if (found)
        {
            List<Vector3> path = new List<Vector3>();
            Vector3Int step = targetCell;

            while (step != currentCell)
            {
                Vector3 worldPos = walkableTilemap.GetCellCenterWorld(step);
                worldPos.z = 0f; // ✅ Force Z = 0
                path.Add(worldPos);
                step = cameFrom[step];
            }

            path.Reverse();
            foreach (Vector3 point in path)
            {
                pathQueue.Enqueue(point);
            }
        }
        else
        {
            Debug.Log("No path found to endline using Dijkstra.");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("🟡 Collision entered with: " + collision.gameObject.tag);

        if (collision.gameObject.CompareTag("Detective"))
        {
            isCaught = true;
            StopAllCoroutines();
            pathQueue.Clear();

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }

            if (BackgroundMusicManager.Instance != null)
                BackgroundMusicManager.Instance.StopMusic();

            string mode = PlayerPrefs.GetString("GameMode");
            Debug.Log("🎮 Mode in collision: " + mode);

            if (mode == "ThiefVsAIDetective" && LosingBeatAudio != null)
            {
                LosingBeatAudio.Play();
            }
            else if (mode == "DetectiveVsAIThief" && winningBeatAudio != null)
            {
                winningBeatAudio.Play();
                winAudio.Play();
            }
            else if (mode == "TwoPlayers" && winningBeatAudio != null)
            {
                winningBeatAudio.Play();
                winAudio.Play();
            }

            if (caughtText != null)
            {
                caughtPanel.SetActive(true);

                if (mode == "DetectiveVsAIThief")
                {
                    if (ScoreManager.instance != null)
                        caughtText.text = "You Won!!!\n Thief caught in " + ScoreManager.instance.moveCount + " moves!";
                    else
                        caughtText.text = "You Won!!!\n Thief was caught!";
                }
                else if (mode == "ThiefVsAIDetective")
                {
                    caughtText.text = "Detective caught you.\n Better luck next time.";
                }
                else if (mode == "TwoPlayers")
                {
                    caughtText.text = "Game Over.\nDetective caught the thief!";
                }

                Transform restartBtn = caughtPanel.transform.Find("RestartButton");
                if (restartBtn != null) restartBtn.gameObject.SetActive(true);

                Transform resetBtn = caughtPanel.transform.Find("ResetHighScoreButton");
                if (resetBtn != null) resetBtn.gameObject.SetActive(true);

                ScoreManager.instance?.TryUpdateHighScore();
                Time.timeScale = 0f;
            }

            Debug.Log("🟥 Thief caught!");
        }
    }

}

