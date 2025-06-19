public static class GameEvents
{
    public delegate void OnGameOver(int winner);
    public static event OnGameOver gameOverEvent;

    public static void TriggerGameOver(int winner)
    {
        gameOverEvent?.Invoke(winner);
    }
}
