using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Static")]
    public static MapManager main;

    [Header("Map")]
    [SerializeField]
    private int _width;
    [SerializeField]
    private int _height;
    private int[,] _mapData;
    [SerializeField]
    private GridSquareBehaviour _gridSquareObj;
    [SerializeField] private Transform _mapParent;

    [Header("Walls")]
    [SerializeField] private GameObject _wallPrefab;
    [SerializeField] private Transform _lvl1Walls;
    private Transform[] _wallSpawnPointsLvl1;
    

    public int Width => _width;
    public int Height => _height;

    public int[,] MapData => _mapData;

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
        _wallSpawnPointsLvl1 = GameObject.Find("WallSpawnPointsLvl1").GetComponentsInChildren<Transform>();
            }

    public void CreateMap()
    {
        _mapData = new int[_width, _height];

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                transform.position = new Vector3(x, 0, y);
                GridSquareBehaviour newGridSquare = Instantiate(_gridSquareObj, transform.position, Quaternion.identity);
                newGridSquare.transform.SetParent(_mapParent);
                newGridSquare.gameObject.transform.name = x + ", " + y;
                newGridSquare.Init(this);

                _mapData[x, y] = 0;
            }
        }
        SpawnWalls();
    }

    /// <summary>
    /// Spawnws walls at each spawn point
    /// </summary>
    public void SpawnWalls()
    {
        int numWalls = 0; // prevents wall from spawning at location of parent game obj
        foreach(Transform t in _wallSpawnPointsLvl1)
        {
            if (numWalls != 0)
            {
                GameObject newWall = Instantiate(_wallPrefab, t);
                newWall.transform.SetParent(_lvl1Walls);
            }
            numWalls++;
        }
    }

    public void EditGrid(int x, int y, int value)
    {
        _mapData[x, y] = value;
    }

    public int GetGridPos(int x, int y)
    {
        return _mapData[x, y];
    }
}
