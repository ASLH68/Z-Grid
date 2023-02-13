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
        _buildings = new();
    }

    public void CreateBuilding(int x, int y)
    {
        transform.position = new Vector3(x, 0, y) + _offset;
        _buildings.Add(Instantiate(_buildingObj, transform.position, Quaternion.identity));
        MapManager.main.EditGrid(x, y, -1);
        EnemyManager.main.UpdatePaths();
    }

    public void DestroyBuilding(int x, int y)
    {
        foreach (BuildingBehaviour curBehaviour in _buildings.ToArray())
        {
            if (Mathf.RoundToInt(curBehaviour.transform.position.x) == x && Mathf.RoundToInt(curBehaviour.transform.position.z) == y)
            {
                MapManager.main.EditGrid(x, y, 0);
                Destroy(curBehaviour.gameObject);
                _buildings.Remove(curBehaviour);
            }
        }
    }
}
