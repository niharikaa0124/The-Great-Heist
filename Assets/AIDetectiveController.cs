using UnityEngine;
using System.Collections.Generic;

public class AIDetectiveController : MonoBehaviour
{
    public Transform thief;                   // 🎯 Thief target
    public float moveSpeed = 7f;

    private List<Vector3> path = new List<Vector3>();
    private int pathIndex = 0;
    private Animator animator;

    public SimplePathfinder simplePathfinder; // ✅ Use simpler pathfinder

    void Start()
    {
        animator = GetComponent<Animator>();
        if (thief == null)
        {
            GameObject thiefObj = GameObject.FindWithTag("Thief");
            if (thiefObj != null)
                thief = thiefObj.transform;
        }

        if (simplePathfinder == null)
            simplePathfinder = FindObjectOfType<SimplePathfinder>();

        if (thief != null && simplePathfinder != null)
        {
            InvokeRepeating(nameof(UpdatePath), 0.1f, 0.4f);
        }
        else
        {
            Debug.LogWarning("❗ Thief or SimplePathfinder not assigned.");
        }
    }

    void Update()
    {
        if (path == null || pathIndex >= path.Count)  return;

        Vector3 target = path[pathIndex];
        Vector3 direction = (target - transform.position).normalized;
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);
            animator.SetFloat("speed", direction.sqrMagnitude);

            if (direction != Vector3.zero)
            {
                animator.SetFloat("last Horizontal", direction.x);
                animator.SetFloat("lastVertical", direction.y);
            }
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            pathIndex++;
        }
    }

    void UpdatePath()
    {
        if (thief == null || simplePathfinder == null) return;

        path = simplePathfinder.FindPath(transform.position, thief.position);
        pathIndex = 0;
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

