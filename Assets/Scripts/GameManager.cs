using UnityEngine;
using TMPro; // REQUIRED for TextMesh Pro
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float timeLimit = 120f; // 2 Minutes
    public int totalCubes;
    [SerializeField] private float maxFreeFallingTime = 4f;

    [Header("References")]
    public Transform player;
    public TMP_Text timerText;   // CHANGED: Supports TextMeshPro
    public TMP_Text cubesText;   // CHANGED: Supports TextMeshPro
    public GameObject gameOverPanel;
    public TMP_Text gameOverReasonText; // CHANGED: Supports TextMeshPro

    private float currentTime;
    private int collectedCubes = 0;
    private bool isGameOver = false;

    // State for falling check
    private float timeFalling = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentTime = timeLimit;

        // Auto-count cubes in the scene
        totalCubes = FindObjectsByType<Collectible>(FindObjectsSortMode.None).Length;

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
        // Raycast down relative to player's current gravity orientation
        Ray ray = new Ray(player.position, -player.up);

        // If ray hits NOTHING for 50 units...
        if (!Physics.Raycast(ray, 50f))
        {
            timeFalling += Time.deltaTime;

            // ...and we have been falling for > 1.5s (buffer for jumps)
            if (timeFalling > maxFreeFallingTime)
            {
                EndGame("Lost in Space!");
            }
        }
        else
        {
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

        // Disable player controls
        if (player.GetComponent<PlayerController>())
            player.GetComponent<PlayerController>().enabled = false;

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f; // Pause
    }

    void UpdateUI()
    {
        // Format time
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