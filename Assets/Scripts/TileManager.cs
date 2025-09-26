using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int GRID_SIZE_X = 5;
    [SerializeField] private int GRID_SIZE_Y = 5;

    private int noFilledTiles;
    private float progressPercent;
    
    private List<Tile> tiles = new List<Tile>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector2(-GRID_SIZE_X/2, -GRID_SIZE_Y/2);

        GameObject[] tileObjects = GameObject.FindGameObjectsWithTag("Tile");

        foreach (GameObject tileObject in tileObjects)
        {
            tiles.Add(tileObject.GetComponent<Tile>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        //GUIStyle style = new GUIStyle();
        //style.fontSize = 50;
        //GUI.Label(new Rect(10,10,200,40), "Filled " + progressPercent + "%", style);
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

        progressPercent = ((float) noFilledTiles / tiles.Count) * 100;
        //print(progressPercent);
    }

    public Tile GetRandomTile()
    {
        return tiles[UnityEngine.Random.Range(0, tiles.Count)];
    }

    public Tile GetRandomTileInRadius(Vector3 position, float radius)
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
        return randomTiles[UnityEngine.Random.Range(0, randomTiles.Count)];
    }
}
