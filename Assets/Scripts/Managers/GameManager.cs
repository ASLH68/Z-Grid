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

    private GameObject _lightObj;

    public int Health => _health;
    public int CurrentRound { get => _currentRound; set => _currentRound = value; }

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

        _lightObj = FindObjectOfType<Light>().gameObject;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
            _lightObj.SetActive(Time.timeScale != 0);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Start");
        }
        if (Input.GetKey(KeyCode.Space) && Time.timeScale == 1)
        {
            Time.timeScale = 3;
        }
        else if (Time.timeScale != 0)
        {
            Time.timeScale = 1;
        }
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
    /// Increments the round by 1 and loads the end game scene if game is over
    /// </summary>
    public void UpdateCurrentRound()
    {
        if(CurrentRound == 50)
        {
            SceneManager.LoadScene("Game Won");
        }
        else
        {
            CurrentRound++;
        }
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
