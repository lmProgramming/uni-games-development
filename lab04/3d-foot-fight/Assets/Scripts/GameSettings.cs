using UnityEngine;

[CreateAssetMenu(fileName = "DefaultGameSettings", menuName = "Game/Default Game Settings", order = 1)]
public class GameSettings : ScriptableObject
{
    public int initialLives = 3;

    public float initialHealth = 100f;

    public int scoreGoal = 5;

    public bool hasTimeLimit;
    public float timeLimitSeconds = 180f;
}