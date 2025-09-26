using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int GRID_SIZE_X = 5;
    [SerializeField] private int GRID_SIZE_Y = 5;

    private int noFilledTiles = 0;
    private float progressPercent = 0.0f;
    
    private List<GameObject> tiles = new List<GameObject>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector2(-GRID_SIZE_X/2, -GRID_SIZE_Y/2);
        
        for (int x = 0; x < GRID_SIZE_X; x++)
        {
            for (int y = 0; y < GRID_SIZE_Y; y++)
            {
                GameObject newTile = Instantiate(tilePrefab, new Vector3(transform.position.x + x, transform.position.y + y), Quaternion.identity);
                if (newTile != null)
                {
                    newTile.transform.parent = transform; // Attach to this
                    tiles.Add(newTile.gameObject);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 50;
        GUI.Label(new Rect(10,10,200,40), "Filled " + progressPercent + "%", style);
        
    }

    public void InformStateChange(Tile tile, int previousState)
    {
        if (!tile)
        {
            return;
        }
        
        // TODO: this is O(n) could be made a bit better, maps maybe?
        noFilledTiles = 0;
        foreach (var myTile in tiles.Where(myTile => myTile.GetComponent<Tile>().GetState() == 4))
        {
            noFilledTiles++;
        }

        progressPercent = ((float) noFilledTiles / (GRID_SIZE_X * GRID_SIZE_Y))* 100;
    }
}
