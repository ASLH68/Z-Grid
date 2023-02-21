using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

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
    [SerializeField]
    private Material _dangerMaterial;

    [Header("Walls")]
    [SerializeField] private GameObject _wallPrefab;
    [SerializeField] private Transform _lvl1Walls;
    [HideInInspector] public Transform[] _wallSpawnPointsLvl1;
    

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
                if (x < 3)
                {
                    newGridSquare.GetComponent<Renderer>().material = _dangerMaterial;
                }

                _mapData[x, y] = 0;
            }
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

    public bool InBounds(Vector3Int position)
    {
        return !(position.x < 0 || position.x >= _mapData.GetLength(0) || position.z < 0 || position.z >= _mapData.GetLength(1));
    }
}
