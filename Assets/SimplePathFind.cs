using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class SimplePathfinder : MonoBehaviour
{
    public Tilemap walkableTilemap;
    public Tilemap obstacleTilemap;
    public Tilemap endlineTilemap;

    public List<Vector3> FindPath(Vector3 startWorld, Vector3 targetWorld)
    {
        Vector3Int start = walkableTilemap.WorldToCell(startWorld);
        Vector3Int target = walkableTilemap.WorldToCell(targetWorld);

        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        queue.Enqueue(start);
        visited.Add(start);

        Vector3Int[] directions = {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0)
        };

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();

            if (current == target || (endlineTilemap != null && endlineTilemap.HasTile(current)))
            {
                target = current; // Set actual target if finish line found
                break;
            }

            foreach (var dir in directions)
            {
                Vector3Int next = current + dir;

                if (!visited.Contains(next) && IsWalkable(next))
                {
                    queue.Enqueue(next);
                    visited.Add(next);
                    cameFrom[next] = current;
                }
            }
        }

        // Reconstruct path
        List<Vector3> path = new List<Vector3>();
        if (!cameFrom.ContainsKey(target)) return path;

        Vector3Int step = target;
        while (step != start)
        {
            path.Add(walkableTilemap.GetCellCenterWorld(step));
            step = cameFrom[step];
        }

        path.Reverse();
        return path;
    }

    bool IsWalkable(Vector3Int cell)
    {
        return walkableTilemap.HasTile(cell) && !obstacleTilemap.HasTile(cell);
    }
}
