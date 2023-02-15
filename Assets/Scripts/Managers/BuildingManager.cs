using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingManager : MonoBehaviour
{
    [Header("Static")]
    public static BuildingManager main;

    [Header("Buildings")]
    private List<BuildingBehaviour> _buildings;
    [SerializeField]
    private Vector3 _offset;
    [SerializeField] BuildingBehaviour _wallObj;
    [SerializeField] BuildingBehaviour _turretObj;

    [Header("Building Costs")]
    [SerializeField] private int _wallCost;
    [SerializeField] private int _turretCost;

    public int WallCost => _wallCost;
    public int TurretCost => _turretCost;
    public enum BuildingType
    {
        wall,
        turret
    }

    public BuildingType _currentBuilding;

    //Ignore, for script management
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
        //Creates an empty list of the buildings to be creates
        _buildings = new();
        SetCurrentBuilding("wall");
        SpawnWalls();
    }

    public void SpawnWalls()
    {
        int numWalls = 0; // prevents wall from spawning at location of parent game obj
        foreach (Transform t in MapManager.main._wallSpawnPointsLvl1)
        {
            if (numWalls != 0)
            {
                _buildings.Add(Instantiate(_wallObj, t));
            }
            numWalls++;

            //Edits the map
            MapManager.main.EditGrid(Mathf.RoundToInt(t.transform.position.x), Mathf.RoundToInt(t.transform.position.z), -1);

            //Edits the pathing of all visible enemies
            EnemyManager.main.UpdatePaths();
        }
    }

    /// <summary>
    /// Instantiates a new building with BuildingBehaviour at index (x,y), and changes MapData to cpmensate for it
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void CreateBuilding(int x, int y)
    {
        switch (_currentBuilding)
        {
            //Creates the new wall object
            case BuildingType.wall:
                transform.position = new Vector3(x, 0, y) + _offset;
                if (PlayerManager.main.HasEnoughCurrency(_wallCost))
                {
                    _buildings.Add(Instantiate(_wallObj, transform.position, Quaternion.identity));
                    PlayerManager.main.ModifyCurrency(-_wallCost);

                    //Edits the map
                    MapManager.main.EditGrid(x, y, 25);

                    //Edits the pathing of all visible enemies
                    EnemyManager.main.UpdatePaths();
                }
                break;
            //Creates a new turret obj
            case BuildingType.turret:
                transform.position = new Vector3(x, 0, y) + _offset;
                if (PlayerManager.main.HasEnoughCurrency(_turretCost))
                {
                    _buildings.Add(Instantiate(_turretObj, transform.position, Quaternion.identity));
                    PlayerManager.main.ModifyCurrency(-_turretCost);

                    //Edits the map
                    MapManager.main.EditGrid(x, y, 75);

                    //Edits the pathing of all visible enemies
                    EnemyManager.main.UpdatePaths();
                }
                break;
        }
    }

    public void DamageBuilding(int x, int y)
    {
        //Find the building at the location of (x,y)
        foreach (BuildingBehaviour curBehaviour in _buildings.ToArray())
        {
            if (Mathf.RoundToInt(curBehaviour.transform.position.x) == x && Mathf.RoundToInt(curBehaviour.transform.position.z) == y)
            {
                if (curBehaviour.TakeDamage())
                {
                    DestroyBuilding(x, y);
                }
            }
        }
    }

    /// <summary>
    /// Removes a building and the corresponding BuildingBehaviour at index (x,y), and changes MapData to cpmensate for it
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void DestroyBuilding(int x, int y)
    {
        //Find the building at the location of (x,y)
        foreach (BuildingBehaviour curBehaviour in _buildings.ToArray())
        {
            if (Mathf.RoundToInt(curBehaviour.transform.position.x) == x && Mathf.RoundToInt(curBehaviour.transform.position.z) == y)
            {
                //Edits the grid and removes the building
                MapManager.main.EditGrid(x, y, 0);
                Destroy(curBehaviour.gameObject);
                _buildings.Remove(curBehaviour);
            }
        }

        EnemyManager.main.UpdatePaths();
    }

    /// <summary>
    /// Destroys all building on the map
    /// </summary>
    public void DestroyAllBuildings()
    {
        foreach (BuildingBehaviour curBehaviour in _buildings.ToArray())
        {
            //Edits the grid and removes the building
            DestroyBuilding(Mathf.RoundToInt(curBehaviour.transform.position.x), Mathf.RoundToInt(curBehaviour.transform.position.z));   
        }
    }

    /// <summary>
    /// Sets the current building to newBuilding
    /// </summary>
    /// <param name="newBuilding"></param>
    public void SetCurrentBuilding(string newBuilding)
    {
        switch (newBuilding)
        {
            case "wall":
                _currentBuilding = BuildingType.wall;
                break;
            case "turret":
                _currentBuilding = BuildingType.turret;
                break;
        }
        CanvasManager.main.UpdateBuildingText();
    }
}
