using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float timeLimit = 120f;
    public int totalCubes;

    [Header("References")]
    public Transform player;
    public TMP_Text timerText;
    public TMP_Text cubesText;
    public GameObject gameOverPanel;
    public TMP_Text gameOverReasonText;

    private float currentTime;
    private int collectedCubes = 0;
    private bool isGameOver = false;

    // State for falling check
    private float timeFalling = 0f;
    private Rigidbody playerRb;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentTime = timeLimit;
        totalCubes = FindObjectsByType<Collectible>(FindObjectsSortMode.None).Length;

        if (player != null) playerRb = player.GetComponent<Rigidbody>();

        UpdateUI();
        gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (isGameOver) return;

        // 1. Timer Logic
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            EndGame("Time's Up!");
        }

        // 2. Falling into Void Logic
        CheckFreeFall();

        UpdateUI();
    }

    void CheckFreeFall()
    {
        if (player == null) return;

        // FIX 1: Raise the start point by 0.5 units (Knees/Waist) so it doesn't clip through the floor
        Vector3 rayOrigin = player.position + (player.up * 0.5f);

        // Raycast down relative to player's current gravity orientation
        Ray ray = new Ray(rayOrigin, -player.up);

        // Debug line to visualize the ray in Scene view (optional)
        Debug.DrawRay(rayOrigin, -player.up * 50f, Color.red);

        // FIX 2: Velocity Check
        // Only consider it "Falling" if the ray hits nothing AND the player is actually moving.
        // If speed is near 0, we are just standing still, even if the ray misses.
        float currentSpeed = playerRb != null ? playerRb.linearVelocity.magnitude : 10f; // Use 'velocity' for older Unity

        // If ray hits NOTHING for 50 units...
        if (!Physics.Raycast(ray, 50f))
        {
            // ... AND we are moving faster than 1.0f (falling)
            if (currentSpeed > 1.0f)
            {
                timeFalling += Time.deltaTime;

                // Wait 1.5s to confirm fall
                if (timeFalling > 1.5f)
                {
                    EndGame("Lost in Space!");
                }
            }
        }
        else
        {
            // Reset if we found ground OR we stopped moving
            timeFalling = 0f;
        }
    }

    public void CollectCube()
    {
        collectedCubes++;
        if (collectedCubes >= totalCubes)
        {
            EndGame("Mission Complete!");
        }
    }

    public void EndGame(string reason)
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);
        gameOverReasonText.text = reason;

        if (player.GetComponent<PlayerController>())
            player.GetComponent<PlayerController>().enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    void UpdateUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60F);
        int seconds = Mathf.FloorToInt(currentTime % 60F);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        cubesText.text = $"Cubes: {collectedCubes} / {totalCubes}";
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}