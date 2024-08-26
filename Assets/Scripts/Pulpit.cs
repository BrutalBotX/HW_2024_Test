using UnityEngine;

public class Pulpit : MonoBehaviour
{
    public float lifespan;
    public GameManager gameManager;
    private float remainingLifespan;

    void Start()
    {
        remainingLifespan = lifespan;
        InvokeRepeating("UpdateRemainingLifespan", 1f, 1f); // Update every second
        Invoke("DestroySelf", lifespan);
    }

    void UpdateRemainingLifespan()
    {
        remainingLifespan -= 1f; // Decrease remaining lifespan by 1 second
        if (gameManager != null)
        {
            gameManager.UpdatePulpitLifespanDisplay(this); // Update the GameManager with the current lifespan
        }
    }

    void DestroySelf()
    {
        if (gameManager != null)
        {
            gameManager.DestroyPulpit(gameObject);
        }
        else
        {
            Debug.LogWarning("GameManager reference is null in Pulpit script.");
            Destroy(gameObject);
        }
    }

    public float GetRemainingLifespan()
    {
        return remainingLifespan;
    }
}
