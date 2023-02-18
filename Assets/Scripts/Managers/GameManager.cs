using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Static")]
    public static GameManager main;
    private int _currentRound;

    [Header("Given Currencies")]
    [Tooltip("Amount of currency the player receives at the end round 1")]
    [SerializeField] private int _currencyAmount;

    [SerializeField]
    private int _health;
    [SerializeField]
    private int _maxHealth;

    [Multiline]
    [SerializeField]
    private string _enemyCode;

    [SerializeField]
    private Round _round;

    public int Health => _health;
    public int CurrentRound => _currentRound;

    private void Awake()
    {
        if (main == null)
        {
            main = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        _health = _maxHealth;
        _currentRound = 1;
        MapManager.main.CreateMap();
        
        CanvasManager.main.UpdateLivesText();

        _round = new Round(_enemyCode);
        EnemyManager.main.StartRound(_round);
    }

    public void LoseLife(int value)
    {
        _health -= value;
        CanvasManager.main.UpdateLivesText();
        if (_health <= 0)
        {
            SceneManager.LoadScene("End Game");
        }
    }

    /// <summary>
    /// Starts the sandbox mode after round 1 ends
    /// </summary>
    public void EndRound1()
    {
        //PlayerManager.main.ModifyCurrency(_currencyAmount);
    }

    /// <summary>
    /// Incrases the current round by 1
    /// </summary>
    private void IncreaseRound()
    {
        _currentRound++;
        CanvasManager.main.UpdateRoundText();
    }
}

[System.Serializable]
public struct Round
{
    public Wave[] waves;
    
    public Round(string enemyCode)
    {
        string[] waves = enemyCode.Split("\n");

        this.waves = new Wave[waves.Length];

        for (int i = 0; i < waves.Length; i++)
        {
            this.waves[i] = new Wave(waves[i].Split(","));
        }
    }
}

[System.Serializable]
public struct Wave
{
    public EnemyBehaviour[] enemies;
    public int[] enemyAmounts;

    public float time;

    public Wave(string[] enemyCount)
    {
        enemies = new EnemyBehaviour[EnemyManager.main.EnemyTypes.Length];
        enemyAmounts = new int[EnemyManager.main.EnemyTypes.Length];

        int time = 0;

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i] = EnemyManager.main.EnemyTypes[i];
            enemyAmounts[i] = int.Parse(enemyCount[i]);
            time += int.Parse(enemyCount[i]) / 2;
        }

        this.time = time;
    }

    public EnemyBehaviour GetRandomEnemy()
    {
        if (!EnemiesLeft())
        {
            return null;
        }

        int enemyIndex;
        do
        {
            enemyIndex = Random.Range(0, enemies.Length);
        } while (enemyAmounts[enemyIndex] <= 0);

        enemyAmounts[enemyIndex]--;
        return enemies[enemyIndex];
    }

    private bool EnemiesLeft()
    {
        foreach (int curAmount in enemyAmounts)
        {
            if (curAmount > 0)
            {
                return true;
            }
        }
        return false;
    }

    public int EnemyCount()
    {
        int total = 0;
        foreach (int curAmount in enemyAmounts)
        {
            total += curAmount;
        }
        return total;
    }
}
