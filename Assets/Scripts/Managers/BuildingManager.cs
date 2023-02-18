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
    [SerializeField] BuildingBehaviour _basicTurretObj;    
    [SerializeField] BuildingBehaviour _machineTurretObj;
    [SerializeField] BuildingBehaviour _sniperTurretObj;

    [Header("Building Costs")]
    [SerializeField] private int _wallCost;
    [SerializeField] private int _basicTurretCost;    
    [SerializeField] private int _machineTurretCost;
    [SerializeField] private int _sniperTurretCost;

    [Header("Building Buttons")]
    [SerializeField] private GameObject _wallButton;
    [SerializeField] private GameObject _basicTurretButton;
    [SerializeField] private GameObject _machineTurretButton;
    [SerializeField] private GameObject _sniperTurretButton;

    public int WallCost => _wallCost;
    public int BasicTurretCost => _basicTurretCost;
    public int MachineTurretCost => _machineTurretCost;
    public int SniperTurretCost => _sniperTurretCost;

    [Header("Weights")]
    [SerializeField] private int _wallWeight;
    [SerializeField] private int _turretWeight;

    public enum BuildingType
    {
        wall,
        baseTurret,
        machineTurret,
        sniperTurret
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
            MapManager.main.EditGrid(Mathf.RoundToInt(t.transform.position.x), Mathf.RoundToInt(t.transform.position.z), 100);

            //Edits the pathing of all visible enemies
            EnemyManager.main.UpdatePaths(true);
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
                PlaceBuilding(x, y, _wallObj, _wallCost, _wallWeight);
                break;

            //Creates a new turret obj
            case BuildingType.baseTurret:
                PlaceBuilding(x, y, _basicTurretObj, _basicTurretCost, _turretWeight);
                _basicTurretCost = Mathf.FloorToInt(_basicTurretCost * 1.025f);
                CanvasManager.main.SetCosts();
                break;

            //Creates a new machine turret
            case BuildingType.machineTurret:
                PlaceBuilding(x, y, _machineTurretObj, _machineTurretCost, _turretWeight);
                _machineTurretCost = Mathf.FloorToInt(_machineTurretCost * 1.025f);
                CanvasManager.main.SetCosts();
                break;

            //Creates a new sniper turret
            case BuildingType.sniperTurret:
                PlaceBuilding(x, y, _sniperTurretObj, _sniperTurretCost, _turretWeight);
                _sniperTurretCost = Mathf.FloorToInt(_sniperTurretCost * 1.025f);
                CanvasManager.main.SetCosts();
                break;
        }
    }

    /// <summary>
    /// Places a building
    /// </summary>
    /// <param name="x"> x pos of building </param>
    /// <param name="y"> y pos of building </param>
    /// <param name="buildingToPlace"> building being placed </param>
    /// <param name="cost"> cost of building </param>
    /// <param name="weight"> weight of building </param>
    private void PlaceBuilding(int x, int y, BuildingBehaviour buildingToPlace, int cost, int weight)
    {
        transform.position = new Vector3(x, 0, y) + _offset;

        if (PlayerManager.main.HasEnoughCurrency(cost))
        {
            _buildings.Add(Instantiate(buildingToPlace, transform.position, Quaternion.identity));
            PlayerManager.main.ModifyCurrency(-cost);

            //Edits the map
            MapManager.main.EditGrid(x, y, weight);

            //Edits the pathing of all visible enemies
            EnemyManager.main.UpdatePaths(new Vector3(x, 0.5f, y));
        }
    }

    /// <summary>
    /// Damages a building
    /// </summary>
    /// <param name="x"> x pos of building </param>
    /// <param name="y"> y pos of building </param>
    /// <param name="damage"> amt of dmg </param>
    public void DamageBuilding(int x, int y, int damage)
    {
        //Find the building at the location of (x,y)
        foreach (BuildingBehaviour curBehaviour in _buildings.ToArray())
        {
            if (Mathf.RoundToInt(curBehaviour.transform.position.x) == x && Mathf.RoundToInt(curBehaviour.transform.position.z) == y)
            {
                if (curBehaviour.TakeDamage(damage))
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

        EnemyManager.main.UpdatePaths(new Vector3(x, 0.5f, y));
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
                CanvasManager.main.CurrentButton = _wallButton;
                break;
            case "base turret":
                _currentBuilding = BuildingType.baseTurret;
                CanvasManager.main.CurrentButton = _basicTurretButton;
                break;
            case "machine turret":
                _currentBuilding = BuildingType.machineTurret;
                CanvasManager.main.CurrentButton = _machineTurretButton;
                break;
            case "sniper turret":
                _currentBuilding = BuildingType.sniperTurret;
                CanvasManager.main.CurrentButton = _sniperTurretButton;
                break;
        }
        //CanvasManager.main.UpdateBuildingText();
    }
}
