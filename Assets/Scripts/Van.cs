using System.Collections.Generic;
using UnityEngine;

public class Van : Tile
{
    [SerializeField] private List<GameObject> spawnLocations;

    public Vector3 GetRandomSpawnLocation()
    {
        return transform.position + spawnLocations[Random.Range(0, spawnLocations.Count)].transform.localPosition;
    }
}
