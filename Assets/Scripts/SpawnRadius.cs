using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnRadius : MonoBehaviour
{
    [System.Serializable]
    public struct Spawnable
    {
        public GameObject gameObject;
        public float weight;
    }

    public List<Spawnable> optionalObjects1;
    public List<Transform> spawnPoints1;

    public int minObjectsToSpawnSection1;
    public int maxObjectsToSpawnSection1;

    public bool useRadiusForSpawning = false;
    public float spawnRadius = 0.25f;
    public bool useRandomRotation = false; // Новый параметр

    private List<Transform> spawnPointsToDestroy = new List<Transform>();

    void Start()
    {
        SpawnOptionalObjectsSection(1);
    }

    void SpawnOptionalObjectsSection(int section)
    {
        int numObjects = 0;
        switch (section)
        {
            case 1:
                numObjects = Random.Range(minObjectsToSpawnSection1, maxObjectsToSpawnSection1 + 1);
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
        }
    }

    void SpawnObjectOnUniqueSpawnPoint(List<Spawnable> objectsToSpawn, List<Transform> spawnPointsList)
    {
        GameObject objectToSpawn = ChooseObjectWithChance(objectsToSpawn);
        Transform spawnPoint = GetUniqueSpawnPoint(spawnPointsList);
        Quaternion rotation = Quaternion.identity;
        if (useRandomRotation)
        {
            rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        }
        if (spawnPoint != null)
        {
            GameObject spawnedObject = Instantiate(objectToSpawn, spawnPoint.position, rotation);
            spawnedObject.transform.rotation = rotation;
            spawnPointsToDestroy.Add(spawnPoint);
        }
    }

    Transform GetUniqueSpawnPoint(List<Transform> spawnPointsList)
    {
        var unusedSpawnPoints = spawnPointsList.Where(point => !spawnPointsToDestroy.Contains(point)).ToList();
        if (unusedSpawnPoints.Count > 0)
        {
            int randomIndex;
            if (useRadiusForSpawning)
            {
                randomIndex = Random.Range(0, unusedSpawnPoints.Count);
                Transform selectedSpawnPoint = unusedSpawnPoints[randomIndex];
                Vector3 randomPositionInRadius = selectedSpawnPoint.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), 0, Random.Range(-spawnRadius, spawnRadius));
                unusedSpawnPoints[randomIndex].position = randomPositionInRadius;
            }
            else
            {
                randomIndex = Random.Range(0, unusedSpawnPoints.Count);
            }
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
