using UnityEngine;

public class JSONReader : MonoBehaviour
{
    [System.Serializable]
    public class PlayerData
    {
        public float speed;
    }

    [System.Serializable]
    public class PulpitData
    {
        public float min_pulpit_destroy_time;
        public float max_pulpit_destroy_time;
        public float pulpit_spawn_time;
    }

    [System.Serializable]
    public class GameConfig
    {
        public PlayerData player_data;
        public PulpitData pulpit_data;
    }

    public static GameConfig LoadConfig()
    {
        // Load the JSON file from the Resources folder
        TextAsset jsonFile = Resources.Load<TextAsset>("doofus_dictionary");
        if (jsonFile != null)
        {
            // Deserialize the JSON to GameConfig object
            GameConfig config = JsonUtility.FromJson<GameConfig>(jsonFile.text);
            return config;
        }
        else
        {
            Debug.LogError("Failed to load config.json from Resources.");
            return null;
        }
    }
}
