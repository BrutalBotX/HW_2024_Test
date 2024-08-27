using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // Include this for UI
using UnityEngine.SceneManagement;

public class Mov : MonoBehaviour
{
    public Rigidbody rb;
    public float speed;
    public Vector3 InitialPosition = new Vector3(0, 1, 0);
    private GameManager gameManager;

    void Start()
    {        
        LoadPlayerSpeed();
        this.name = "Doofus";
        rb.useGravity = true;
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {

        if (transform.position.y < 0) 
        {
            CheckGameOver();
        }

        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(-speed, 0, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(speed, 0, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(0, 0, speed);
        }
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(0, 0, -speed);
        }
        if (Input.GetKey(KeyCode.Return))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetKey(KeyCode.L))
        {
            SceneManager.LoadScene(0);
        }

    }

    void LoadPlayerSpeed()
    {
        JSONReader.GameConfig config = JSONReader.LoadConfig();
        if (config != null)
        {
            speed = config.player_data.speed; // Assign the speed value from player_data
            Debug.Log("Player speed loaded: " + speed);
        }
        else
        {
            Debug.LogError("Failed to load player speed from config.");
        }
    }

        void CheckGameOver()
    {
        // Check if the player is touching any pulpit
        if (transform.position.y < 0)
        {
            Debug.Log("Game Over! Doofus fell off the platform.");
            gameManager?.GameOver(); // Call the GameOver method in GameManager, using null-conditional operator
        }
    }

}
