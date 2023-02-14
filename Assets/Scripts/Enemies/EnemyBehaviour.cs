using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyBehaviour : MonoBehaviour
{
    private Rigidbody _rb;

    private Vector3[] _positions;
    private Vector3 _movePos;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        UpdatePath();

        _movePos = _positions[1];

        //StartCoroutine(MoveCoroutine());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        Vector3 direction = _movePos - transform.position;

        _rb.velocity = direction.normalized;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string path = "";

            foreach (Vector3 curPos in _positions)
            {
                path += curPos + ", ";
            }
            print(path);
        }
    }

    public void UpdatePath()
    {
        Vector3Int startPos = new Vector3Int(Mathf.RoundToInt(transform.position.x), 0, Mathf.RoundToInt(transform.position.z));
        Vector3Int endPos = new Vector3Int(MapManager.main.Width - 1, 0, MapManager.main.Height / 2);

        if (startPos == endPos)
        {
            Destroy(gameObject);
        }

        List<Node> _nodes = Pathfinder.Pathfind(startPos, endPos, MapManager.main.MapData);

        _positions = new Vector3[_nodes.Count];
        for (int i = 0; i < _positions.Length; i++)
        {
            _positions[i] = new Vector3(_nodes[i].Position.x, 0.5f, _nodes[i].Position.z);
        }
    }

    public void Move()
    {
        /*
        for (int i = _positions.Length - 1; i >= 0; i--)
        {
            Vector3 targetPos = new(_positions[i].x, 0.5f, _positions[i].z);

            Vector2 direction = targetPos - transform.position;

            if (!Physics.Raycast(transform.position, direction.normalized, direction.magnitude))
            {
                _movePos = targetPos;
                break;

                /*
                float angle = Mathf.Atan2(direction.y, direction.x);

                transform.eulerAngles = Vector3.up * angle * Mathf.Rad2Deg;
                
            }
        }
        */

        if (_movePos != null && _positions != null)
        {
            if (Vector3.Distance(transform.position, _movePos) < 1
            || _positions[1] == transform.position)
            {
                UpdatePath();
            }
            if (_positions.Length >= 2)
            {
                _movePos = _positions[1];
            }
        }
    }

    public IEnumerator MoveCoroutine()
    {
        Vector3 startPos = new Vector3(Mathf.RoundToInt(transform.position.x), 0.5f, Mathf.RoundToInt(transform.position.y));
        Vector3 endPos = new Vector3(MapManager.main.Width - 1, 0.5f, startPos.y);

        while (startPos != endPos)
        {
            startPos = new Vector3(Mathf.RoundToInt(transform.position.x), 0.5f, Mathf.RoundToInt(transform.position.y));

            transform.position = _movePos;

            yield return new WaitForSeconds(1);
        }

        yield return null;
    }
}
