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

    [SerializeField]
    private Round[] _rounds;

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

        EnemyManager.main.StartRound(_rounds[0]);
        StartCoroutine(RoundProgression());

        CanvasManager.main.UpdateLivesText();
    }

    public void LoseLife(int value)
    {
        CanvasManager.main.UpdateLivesText();
        _health -= value;
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
        //BuildingManager.main.DestroyAllBuildings();
        PlayerManager.main.ModifyCurrency(_currencyAmount);
    }

    /// <summary>
    /// Incrases the current round by 1
    /// </summary>
    private void IncreaseRound()
    {
        _currentRound++;
        CanvasManager.main.UpdateRoundText();
    }

    public IEnumerator RoundProgression()
    {
        while (true)
        {
            if (!EnemyManager.main.AnyEnemiesLeft())
            {
                if (_currentRound == 1)
                {
                    EndRound1();
                }

                PlayerManager.main.ModifyCurrency(250);

                IncreaseRound();

                if (_currentRound >= _rounds.Length)
                {
                    break;
                    //EnemyManager.main.StartRound(new Round(_currentRound));
                }
                else
                {
                    EnemyManager.main.StartRound(_rounds[_currentRound - 1]);
                }
            }

            yield return new WaitForSeconds(1);
        }
    }
}

[System.Serializable]
public struct Round
{
    public Wave[] waves;

    /*
    public Round(int roundNum)
    {
        waves = new Wave[Mathf.FloorToInt(Mathf.Sqrt(roundNum)) + 1];

        for (int i = 0; i < waves.Length; i++)
        {
            waves[i].enemyCount = (10 + Random.Range(-5, 6)) / waves.Length;
            waves[i].time = (9.9f + Random.Range(-2.5f, 2.5f)) / waves.Length;
        }
    }
    */
}

[System.Serializable]
public struct Wave
{
    public EnemyBehaviour[] enemies;
    public int[] enemyAmounts;

    public float time;

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
