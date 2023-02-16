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
        if (!_occupied)
        {
            Vector2Int gridSquarePos = new(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
            if (gridSquarePos.x >= 3 && !EnemyManager.main.EnemyOn(gridSquarePos.x, gridSquarePos.y))
            {
                _manager.EditGrid(gridSquarePos.x, gridSquarePos.y, -1);
                BuildingManager.main.CreateBuilding(gridSquarePos.x, gridSquarePos.y);
                _occupied = true;
            }
        }
    }
}
