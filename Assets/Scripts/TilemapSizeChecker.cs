using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapSizeChecker : MonoBehaviour
{
    public Tilemap tilemap;

    void Start()
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap not assigned!");
            return;
        }

        BoundsInt bounds = tilemap.cellBounds;
        Debug.Log("Map Size: " + bounds.size.x + " x " + bounds.size.y);
        Debug.Log("Origin: " + bounds.position);
    }

    // Ye function check karega ki given world position tilemap ke andar hai ya nahi
    public bool IsPositionInsideMap(Vector3 worldPos)
    {
        Vector3Int cellPos = tilemap.WorldToCell(worldPos);
        BoundsInt bounds = tilemap.cellBounds;

        // Pehle bounds me check karo
        if (!bounds.Contains(cellPos))
            return false;

        // Fir check karo ki cell me tile exist karta hai ya nahi
        TileBase tile = tilemap.GetTile(cellPos);
        return tile != null;
    }
}