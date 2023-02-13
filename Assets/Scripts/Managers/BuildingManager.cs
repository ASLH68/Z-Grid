using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [Header("Static")]
    public static BuildingManager main;

    [Header("Buildings")]
    private List<BuildingBehaviour> _buildings;
    [SerializeField]
    private BuildingBehaviour _buildingObj;
    [SerializeField]
    private Vector3 _offset;

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
    }

    /// <summary>
    /// Instantiates a new building with BuildingBehaviour at index (x,y), and changes MapData to cpmensate for it
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void CreateBuilding(int x, int y)
    {
        //Creates the new building object
        transform.position = new Vector3(x, 0, y) + _offset;
        _buildings.Add(Instantiate(_buildingObj, transform.position, Quaternion.identity));

        //Edits the map
        MapManager.main.EditGrid(x, y, -1);

        //Edits the pathing of all visible enemies
        EnemyManager.main.UpdatePaths();
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
    }
}
