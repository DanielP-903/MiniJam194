using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    private int noFilledTiles;
    //private float progressPercent;
    
    private readonly List<Tile> tiles = new List<Tile>();
    
    public void Init()
    {
        GameObject[] tileObjects = GameObject.FindGameObjectsWithTag("Tile");

        foreach (GameObject tileObject in tileObjects)
        {
            Tile newTile = tileObject.GetComponent<Tile>();
            newTile.enabled = true;
            tiles.Add(tileObject.GetComponent<Tile>());
        }
    }

    public void InformStateChange(Tile tile, int previousState)
    {
        if (!tile)
        {
            return;
        }
        
        // TODO: this is O(n) could be made a bit better, maps maybe?
        noFilledTiles = 0;
        foreach (var myTile in tiles)
        {
            if (myTile.IsAtMaxState())
            {
                noFilledTiles++;
            }
        }
        
        GameManager.Instance.playerHUD.UpdateFillPercentage(((float) noFilledTiles / tiles.Count) * 100);
    }

    public Tile GetRandomTile()
    {
        return tiles[Random.Range(0, tiles.Count)];
    }

    public static Tile GetRandomTileInRadius(Vector3 position, float radius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, radius);
        List<Tile> randomTiles = new List<Tile>();
        foreach (var c in colliders)
        {
            c.gameObject.TryGetComponent<Tile>(out var tile);
            if (tile)
            {
                randomTiles.Add(c.gameObject.GetComponent<Tile>());
            }
        }
        
        // // FOR TESTING TODO: REMOVE
        // foreach (var tile in randomTiles)
        // {
        //     tile.GetComponent<SpriteRenderer>().color = Color.red;
        // }
        //
        return randomTiles[Random.Range(0, randomTiles.Count)];
    }

    public int GetNumberOfTilesCaptured()
    {
        return noFilledTiles;
    }

    public int GetNumberOfTiles()
    {
        return tiles.Count;
    }
}
