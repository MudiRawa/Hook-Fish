using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [Header("Fish")]
    public List<FishData> fishList;

    [Header("Spawn Area")]
    public BoxCollider spawnArea;

    [Header("Spawn Settings")]
    public float spawnInterval = 3f;
    public int maxFish = 20;

    private float spawnTimer;

    private List<GameObject> spawnedFish =
        new List<GameObject>();

    private void Start()
    {
        spawnTimer = 0f;
    }

    private void Update()
    {
        RemoveDestroyedFish();

        spawnTimer -=
            Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            if (spawnedFish.Count < maxFish)
            {
                SpawnFish();
            }

            spawnTimer =
                spawnInterval;
        }
    }

    private void SpawnFish()
    {
        if (
            fishList == null ||
            fishList.Count == 0
        )
        {
            Debug.LogWarning(
                "FishSpawner: Fish List masih kosong."
            );

            return;
        }

        if (spawnArea == null)
        {
            Debug.LogWarning(
                "FishSpawner: Spawn Area belum diisi."
            );

            return;
        }

        // Pilih FishData secara random
        FishData fishData =
            fishList[
                Random.Range(
                    0,
                    fishList.Count
                )
            ];

        if (fishData == null)
        {
            Debug.LogWarning(
                "FishSpawner: FishData tidak valid."
            );

            return;
        }

        if (fishData.fishPrefab == null)
        {
            Debug.LogWarning(
                "FishSpawner: FishPrefab untuk " +
                fishData.fishName +
                " belum diisi."
            );

            return;
        }

        // Random posisi dalam area
        Bounds bounds =
            spawnArea.bounds;

        Vector3 spawnPosition =
            new Vector3(
                Random.Range(
                    bounds.min.x,
                    bounds.max.x
                ),
                Random.Range(
                    bounds.min.y,
                    bounds.max.y
                ),
                Random.Range(
                    bounds.min.z,
                    bounds.max.z
                )
            );

        // Random rotasi
        Quaternion spawnRotation =
            Quaternion.Euler(
                0f,
                Random.Range(
                    0f,
                    360f
                ),
                0f
            );

        // Spawn ikan
        GameObject fish =
            Instantiate(
                fishData.fishPrefab,
                spawnPosition,
                spawnRotation
            );

        spawnedFish.Add(fish);

        // Setup FishMovement
        FishMovement fishMovement =
            fish.GetComponent<FishMovement>();

        if (fishMovement != null)
        {
            fishMovement.SetMovementArea(
                spawnArea
            );

            fishMovement.SetFishData(
                fishData
            );
        }
        else
        {
            Debug.LogWarning(
                fishData.fishName +
                " tidak memiliki FishMovement."
            );
        }

        Debug.Log(
            "Spawn ikan: " +
            fishData.fishName
        );
    }

    private void RemoveDestroyedFish()
    {
        spawnedFish.RemoveAll(
            fish => fish == null
        );
    }

    public int GetCurrentFishCount()
    {
        RemoveDestroyedFish();

        return spawnedFish.Count;
    }
}