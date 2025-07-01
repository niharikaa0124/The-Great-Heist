using UnityEngine;
using UnityEngine.Tilemaps;

public class TileTypeChecker : MonoBehaviour
{
    public Tilemap walkableTilemap;
    public Tilemap obstacleTilemap;
    public Tilemap endlineTilemap;

    public bool IsWalkable(Vector3Int cellPos)
    {
        return walkableTilemap.HasTile(cellPos) && !obstacleTilemap.HasTile(cellPos);
    }
}
