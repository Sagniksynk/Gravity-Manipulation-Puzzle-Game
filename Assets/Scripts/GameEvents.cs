using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<int, int> OnCubeCollected; // (current, total)
    public static event Action<float> OnTimerUpdated;
    public static event Action<string> OnGameOver;

    // Helper methods to safely invoke events
    public static void TriggerCubeCollected(int current, int total) => OnCubeCollected?.Invoke(current, total);
    public static void TriggerTimerUpdate(float time) => OnTimerUpdated?.Invoke(time);
    public static void TriggerGameOver(string reason) => OnGameOver?.Invoke(reason);
}