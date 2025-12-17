using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public float timeLimit = 120f;

    [Header("References")]
    public Transform player;
    public TMP_Text timerText;
    public TMP_Text cubesText;
    public GameObject gameOverPanel;
    public TMP_Text gameOverReasonText;

    private float currentTime;
    private int totalCubes;
    private int collectedCubes = 0;
    private bool isGameOver = false;

    //Cache String Builder or use simple formatting to avoid GC
    private int lastDisplayedSecond = -1;

    // State for falling check
    private float timeFalling = 0f;
    private Rigidbody playerRb;

    void Awake()
    {
        // Singleton Pattern with duplicate protection
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // Cache total cubes
        totalCubes = FindObjectsByType<Collectible>(FindObjectsSortMode.None).Length;
        if (player != null) playerRb = player.GetComponent<Rigidbody>();
    }

    void Start()
    {
        currentTime = timeLimit;
        gameOverPanel.SetActive(false);

        // Initial UI Update
        UpdateCubesUI(0, totalCubes);

        // Start Optimization Routines
        StartCoroutine(FallCheckRoutine());
    }

    void OnEnable()
    {
        GameEvents.OnCubeCollected += HandleCubeCollected;
    }

    void OnDisable()
    {
        GameEvents.OnCubeCollected -= HandleCubeCollected;
    }

    void Update()
    {
        if (isGameOver) return;
        // Timer Logic
        currentTime -= Time.deltaTime;

        int currentSecond = Mathf.CeilToInt(currentTime);
        if (currentSecond != lastDisplayedSecond)
        {
            UpdateTimerUI(currentTime);
            lastDisplayedSecond = currentSecond;
        }

        if (currentTime <= 0)
        {
            EndGame("Time's Up!");
        }
    }

    // --- Event Handlers ---

    private void HandleCubeCollected(int current, int total)
    {
        collectedCubes = current;
        UpdateCubesUI(collectedCubes, totalCubes);

        if (collectedCubes >= totalCubes)
        {
            EndGame("Mission Complete!");
        }
    }

    public void RegisterCollectible(Collectible collectible)
    {
        // Optional: dynamic registration if needed
    }

    

    //Throttled Fall Check (Runs every 0.2s instead of every frame)
    IEnumerator FallCheckRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while (!isGameOver)
        {
            CheckFreeFall();
            yield return wait;
        }
    }

    void CheckFreeFall()
    {
        if (player == null) return;

        Vector3 rayOrigin = player.position + (player.up * 0.5f);
        Ray ray = new Ray(rayOrigin, -player.up);
        float currentSpeed = playerRb != null ? playerRb.linearVelocity.magnitude : 10f;
        if (!Physics.Raycast(ray, 50f))
        {
            if (currentSpeed > 1.0f)
            {
                timeFalling += 0.2f;
                if (timeFalling > 1.5f) EndGame("Lost in Space!");
            }
        }
        else
        {
            timeFalling = 0f;
        }
    }

    // --- UI Updates ---

    void UpdateTimerUI(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    void UpdateCubesUI(int current, int total)
    {
        cubesText.text = $"Cubes: {current} / {total}";
    }

    public void EndGame(string reason)
    {
        if (isGameOver) return;
        isGameOver = true;

        GameEvents.TriggerGameOver(reason); 

        gameOverPanel.SetActive(true);
        gameOverReasonText.text = reason;

        if (player.GetComponent<PlayerController>())
            player.GetComponent<PlayerController>().enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}