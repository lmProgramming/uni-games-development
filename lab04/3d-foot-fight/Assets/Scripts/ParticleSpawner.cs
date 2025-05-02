using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bloodPrefab;

    public void SpawnBlood(Vector3 position)
    {
        Instantiate(bloodPrefab, position, Quaternion.identity);
    }
}