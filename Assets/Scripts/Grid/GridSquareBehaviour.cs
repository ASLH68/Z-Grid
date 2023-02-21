using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSquareBehaviour : MonoBehaviour
{
    [SerializeField]
    private MapManager _manager;

    private bool _occupied;

    public void Init(MapManager manager)
    {
        _manager = manager;
        _occupied = false;
    }

    private void OnMouseDown()
    {
        Vector2Int gridSquarePos = new(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        if (Time.timeScale != 0 && gridSquarePos.x >= 3 && !EnemyManager.main.EnemyOn(gridSquarePos.x, gridSquarePos.y))
        {
            BuildingManager.main.CreateBuilding(gridSquarePos.x, gridSquarePos.y);
        }
    }

    private void OnMouseEnter()
    {
        if (BuildingManager.main._currentBuilding == BuildingManager.BuildingType.destroy)
        {
            BuildingManager.main.SetTargetPos(Vector3.forward * 100);
            return;
        }
        BuildingManager.main.SetTargetPos(transform.position);
    }
}
