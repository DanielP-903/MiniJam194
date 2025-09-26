using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int GRID_SIZE_X = 5;
    [SerializeField] private int GRID_SIZE_Y = 5;
    
    private List<GameObject> tiles;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int x = 0; x < GRID_SIZE_X; x++)
        {
            for (int y = 0; y < GRID_SIZE_Y; y++)
            {
                GameObject newTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity);
                if (newTile != null)
                {
                    newTile.transform.parent = transform; // Attach to this
                    tiles.Add(newTile);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
