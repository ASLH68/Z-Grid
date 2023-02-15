using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Static")]
    public static GameManager main;
    private int _currentRound;

    [Header("Given Currencies")]
    [Tooltip("Amount of currency the player receives at the end round 1")]
    [SerializeField] private int _currecyAmount;

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
        MapManager.main.CreateMap();
        EnemyManager.main.SpawnEnemy();
        _currentRound = 1;
    }

    /// <summary>
    /// Starts the sandbox mode after round 1 ends
    /// </summary>
    public void EndRound1()
    {
        BuildingManager.main.DestroyAllBuildings();
        PlayerManager.main.ModifyCurrency(_currecyAmount);
    }

    /// <summary>
    /// Incrases the current round by 1
    /// </summary>
    private void IncreaseRound()
    {
        _currentRound++;
    }
}
