using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Static")]
    public static GameManager main;

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
    }
}
