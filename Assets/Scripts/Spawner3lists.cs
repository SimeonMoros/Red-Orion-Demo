using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Spawner3lists : MonoBehaviour
{
    [System.Serializable]
    public struct Spawnable
    {
        public GameObject gameObject;
        public float weight;
    }

    public List<Spawnable> optionalObjects1;
    public List<Transform> spawnPoints1;
    public List<Spawnable> optionalObjects2;
    public List<Transform> spawnPoints2;
    public List<Spawnable> optionalObjects3;
    public List<Transform> spawnPoints3;

    public int minObjectsToSpawnSection1;
    public int maxObjectsToSpawnSection1;
    public int minObjectsToSpawnSection2;
    public int maxObjectsToSpawnSection2;
    public int minObjectsToSpawnSection3;
    public int maxObjectsToSpawnSection3;

    private List<Transform> spawnPointsToDestroy = new List<Transform>();

    void Start()
    {
        SpawnOptionalObjectsSection(1);
        SpawnOptionalObjectsSection(2);
        SpawnOptionalObjectsSection(3);
    }

    void SpawnOptionalObjectsSection(int section)
    {
        int numObjects;
        switch (section)
        {
            case 1:
                numObjects = Random.Range(minObjectsToSpawnSection1, maxObjectsToSpawnSection1 + 1);
                break;
            case 2:
                numObjects = Random.Range(minObjectsToSpawnSection2, maxObjectsToSpawnSection2 + 1);
                break;
            default:
                numObjects = Random.Range(minObjectsToSpawnSection3, maxObjectsToSpawnSection3 + 1);
                break;
        }
        for (int i = 0; i < numObjects; i++)
        {
            SpawnOptionalObjects(section);
        }
        AddAllSpawnPointsToDestroy(section);
    }

    void SpawnOptionalObjects(int section)
    {
        switch (section)
        {
            case 1:
                if (optionalObjects1 != null && spawnPoints1.Count > 0)
                {
                    SpawnObjectOnUniqueSpawnPoint(optionalObjects1, spawnPoints1);
                }
                break;
            case 2:
                if (optionalObjects2 != null && spawnPoints2.Count > 0)
                {
                    SpawnObjectOnUniqueSpawnPoint(optionalObjects2, spawnPoints2);
                }
                break;
            default:
                if (optionalObjects3 != null && spawnPoints3.Count > 0)
                {
                    SpawnObjectOnUniqueSpawnPoint(optionalObjects3, spawnPoints3);
                }
                break;
        }
    }

    void SpawnObjectOnUniqueSpawnPoint(List<Spawnable> objectsToSpawn, List<Transform> spawnPointsList)
    {
        GameObject objectToSpawn = ChooseObjectWithChance(objectsToSpawn);
        Transform spawnPoint = GetUniqueSpawnPoint(spawnPointsList);
        if (spawnPoint != null)
        {
            Instantiate(objectToSpawn, spawnPoint.position, spawnPoint.rotation);
            spawnPointsToDestroy.Add(spawnPoint);
        }
    }

    Transform GetUniqueSpawnPoint(List<Transform> spawnPointsList)
    {
        var unusedSpawnPoints = spawnPointsList.Where(point => !spawnPointsToDestroy.Contains(point)).ToList();
        if (unusedSpawnPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, unusedSpawnPoints.Count);
            return unusedSpawnPoints[randomIndex];
        }
        else
        {
            return null;
        }
    }

    GameObject ChooseObjectWithChance(List<Spawnable> objects)
    {
        float totalWeight = 0;
        foreach (var obj in objects)
        {
            totalWeight += obj.weight;
        }

        float randomValue = Random.Range(0, totalWeight);
        float currentSum = 0;

        foreach (var obj in objects)
        {
            currentSum += obj.weight;
            if (currentSum >= randomValue)
            {
                return obj.gameObject;
            }
        }

        return null;
    }

    void AddAllSpawnPointsToDestroy(int section)
    {
        switch (section)
        {
            case 1:
                spawnPointsToDestroy.AddRange(spawnPoints1);
                break;
            case 2:
                spawnPointsToDestroy.AddRange(spawnPoints2);
                break;
            default:
                spawnPointsToDestroy.AddRange(spawnPoints3);
                break;
        }
    }

    void Update()
    {
        if (spawnPointsToDestroy.Count > 0)
        {
            for (int i = 0; i < spawnPointsToDestroy.Count; i++)
            {
                Destroy(spawnPointsToDestroy[i].gameObject);
            }
            spawnPointsToDestroy.Clear();
        }
    }
}
