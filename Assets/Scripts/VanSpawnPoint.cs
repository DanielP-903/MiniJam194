using UnityEngine;

public class VanSpawnPoint : MonoBehaviour
{
    [SerializeField] private float vanSpawnOffsetX;

    public float GetVanSpawnOffsetX()
    {
        return vanSpawnOffsetX;
    }
}
