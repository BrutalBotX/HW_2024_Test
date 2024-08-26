using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // Include this for UI
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pulpitPrefab; // Reference to the pulpit prefab
    public GameObject pulpitToDestroy; // Reference to the scene object named "Pulpit"
    public int maxPulpits = 2;
    public float minSpawnTime;
    public float maxSpawnTime;
    public float minLifespan;
    public float maxLifespan;

    public Text lifespanDisplay; // Reference to the UI Text element to display remaining lifespans

    private List<GameObject> activePulpits = new List<GameObject>();
    private Vector3 lastPulpitPosition;
    private int totalSpawned = 0;

    void Start()
    {
        LoadPulpitConfig();
        if (pulpitPrefab == null)
        {
            Debug.LogError("Pulpit Prefab is not assigned in GameManager.");
            return;
        }
        StartCoroutine(SpawnPulpits());
    }

    void LoadPulpitConfig()
    {
        JSONReader.GameConfig config = JSONReader.LoadConfig();
        if (config != null)
        {
            minSpawnTime = maxSpawnTime = config.pulpit_data.pulpit_spawn_time; // Set both min and max spawn time
            minLifespan = config.pulpit_data.min_pulpit_destroy_time; // Set min lifespan
            maxLifespan = config.pulpit_data.max_pulpit_destroy_time; // Set max lifespan
        }
        else
        {
            Debug.LogError("Failed to load pulpit configuration from JSON.");
        }
    }

    IEnumerator SpawnPulpits()
    {
        while (true)
        {
            CleanupDestroyedPulpits();
            Debug.Log($"Checking spawn conditions. Active Pulpits: {activePulpits.Count}, Max Pulpits: {maxPulpits}");
            if (activePulpits.Count < maxPulpits)
            {
                SpawnPulpit();
            }
            float spawnDelay = Random.Range(minSpawnTime, maxSpawnTime);
            Debug.Log($"Waiting for {spawnDelay} seconds before next spawn check.");
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void SpawnPulpit()
    {
        Vector3 spawnPosition = GetNextPulpitPosition();
        GameObject newPulpit = Instantiate(pulpitPrefab, spawnPosition, Quaternion.identity);
        float lifespan = Random.Range(minLifespan, maxLifespan);
        
        Pulpit pulpitComponent = newPulpit.GetComponent<Pulpit>();
        if (pulpitComponent != null)
        {
            pulpitComponent.lifespan = lifespan;
            pulpitComponent.gameManager = this; // Assign the GameManager to the pulpit
        }
        else
        {
            Debug.LogWarning("Pulpit component not found on the spawned object.");
        }

        activePulpits.Add(newPulpit);
        lastPulpitPosition = spawnPosition;
        totalSpawned++;

        Debug.Log($"Spawned Pulpit #{totalSpawned} at: {spawnPosition}. Active Pulpits: {activePulpits.Count}. Lifespan: {lifespan}");

        // Check if this is the first pulpit spawned
        if (totalSpawned == 1 && pulpitToDestroy != null)
        {
            Destroy(pulpitToDestroy); // Destroy the specified scene object
            Debug.Log("Destroyed the scene object named 'Pulpit'.");
        }
    }

    Vector3 GetNextPulpitPosition()
    {
        if (activePulpits.Count == 0)
        {
            return Vector3.zero; // Start at the origin if no pulpits exist
        }

        // Define the distance between pulpits
        float distanceBetweenPulpits = 9f; // Adjust this value as needed
        Vector3 newPosition;

        do
        {
            // Randomly select a direction (0: Up, 1: Down, 2: Left, 3: Right)
            int direction = Random.Range(0, 4);
            newPosition = lastPulpitPosition;

            switch (direction)
            {
                case 0: // Up
                    newPosition += new Vector3(0, 0, distanceBetweenPulpits);
                    break;
                case 1: // Down
                    newPosition += new Vector3(0, 0, -distanceBetweenPulpits);
                    break;
                case 2: // Left
                    newPosition += new Vector3(-distanceBetweenPulpits, 0, 0);
                    break;
                case 3: // Right
                    newPosition += new Vector3(distanceBetweenPulpits, 0, 0);
                    break;
            }

        } 
        while (Vector3.Distance(newPosition, lastPulpitPosition) < distanceBetweenPulpits || newPosition == lastPulpitPosition); // Ensure it's far enough and not the same

        return newPosition;
    }

    public void DestroyPulpit(GameObject pulpit)
    {
        if (pulpit != null && activePulpits.Contains(pulpit))
        {
            activePulpits.Remove(pulpit);
            Debug.Log($"Destroying Pulpit at: {pulpit.transform.position}. Active Pulpits: {activePulpits.Count}");
            Destroy(pulpit);
        }
    }

    void CleanupDestroyedPulpits()
    {
        activePulpits.RemoveAll(item => item == null);
    }

    public void UpdatePulpitLifespanDisplay(Pulpit pulpit)
    {
        if (lifespanDisplay != null)
        {
            lifespanDisplay.text = $"Remaining Lifespan: {(int)pulpit.GetRemainingLifespan()} seconds";
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over! Restarting the game...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload the current scene
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), $"Active Pulpits: {activePulpits.Count}, Score: {totalSpawned}");

        // Display remaining lifespan for each pulpit
        for (int i = 0; i < activePulpits.Count; i++)
        {
            Pulpit pulpitComponent = activePulpits[i].GetComponent<Pulpit>();
            if (pulpitComponent != null)
            {
                // Convert the remaining lifespan to an integer
                GUI.Label(new Rect(10, 30 + (i * 20), 300, 20), $"Pulpit {i + 1} Lifespan: {(int)pulpitComponent.GetRemainingLifespan()} seconds");
            }
        }
    }
}
