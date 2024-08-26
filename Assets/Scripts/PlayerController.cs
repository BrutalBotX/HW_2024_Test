using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb; // Reference to the Rigidbody component
    public Vector3 InitialPosition = new Vector3(0, 1, 0); // Initial position for resetting
    private GameManager gameManager; // Reference to the GameManager
    public float speed; // Speed of the player

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Ensure Rigidbody is assigned
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing from the player.");
            return; // Exit if Rigidbody is not assigned
        }

        LoadPlayerSpeed(); // Load player speed from JSON
        this.name = "Doofus"; // Set the name of the player
        gameManager = FindObjectOfType<GameManager>(); // Find the GameManager in the scene

        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
        }
    }

    void Update()
    {
        // Check if the player has fallen off the platform
        if (transform.position.y < 0) 
        {
            CheckGameOver();
        }

        // Check for player movement input
        if (Input.GetKey(KeyCode.D))
        {
            Debug.Log("D key pressed");
            rb.AddForce(-speed, 0, 0); // Move right
        }
        if (Input.GetKey(KeyCode.A))
        {
            Debug.Log("A key pressed");
            rb.AddForce(speed, 0, 0); // Move left
        }
        if (Input.GetKey(KeyCode.W))
        {
            Debug.Log("W key pressed");
            rb.AddForce(0, 0, -speed); // Move forward
        }
        if (Input.GetKey(KeyCode.S))
        {
            Debug.Log("S key pressed");
            rb.AddForce(0, 0, speed); // Move backward
        }

        if (Input.GetKey(KeyCode.Return))
        {
            SceneManager.LoadScene(0); // Load the main menu or another scene
        }
    }

    void LoadPlayerSpeed()
    {
        JSONReader.GameConfig config = JSONReader.LoadConfig(); // Load configuration
        if (config != null)
        {
            speed = config.player_data.speed; // Assign the speed value from player_data
            Debug.Log("Player speed loaded: " + speed); // Log the loaded speed
        }
        else
        {
            Debug.LogError("Failed to load player speed from config.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player collides with the ground
        if (collision.gameObject.CompareTag("Ground")) // Ensure your ground has the tag "Ground"
        {
            Debug.Log("Doofus landed on the ground."); // Log when the player lands on the ground
        }
    }

    void CheckGameOver()
    {
        // Check if the player is touching any pulpit
        if (!IsTouchingPulpit())
        {
            Debug.Log("Game Over! Doofus fell off the platform.");
            gameManager?.GameOver(); // Call the GameOver method in GameManager, using null-conditional operator
        }
    }

    bool IsTouchingPulpit()
    {
        // Check if the player is touching any pulpit object
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.5f); // Adjust the radius as needed
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("pulpit"))
            {
                return true; // The player is touching a pulpit
            }
        }
        return false; // The player is not touching any pulpit
    }

    public void Reset()
    {
        this.transform.position = InitialPosition; // Reset the player's position
    }
}
