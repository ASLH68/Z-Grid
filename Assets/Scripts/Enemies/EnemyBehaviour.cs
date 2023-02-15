using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyBehaviour : MonoBehaviour
{
    private enum MoveMode { Attack, Passive }

    private Rigidbody _rb;

    private Vector3[] _positions;
    private Vector3 _movePos;

    [SerializeField]
    private int _health;
    [SerializeField]
    private int _maxHealth;
    [SerializeField]
    private LayerMask _collisionMask;
    private MoveMode _mode;

    [SerializeField]
    private Material _baseMat;
    [SerializeField]
    private Material _dmgMat;

    // Start is called before the first frame update
    void Start()
    {
        _mode = MoveMode.Passive;
        _health = _maxHealth;
        _rb = GetComponent<Rigidbody>();
        UpdatePath();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        
        if (MapManager.main.MapData[Mathf.RoundToInt(_movePos.x), Mathf.RoundToInt(_movePos.z)] > 0)
        {
            Vector3 travelDir = transform.position + (_movePos - transform.position).normalized;

            BuildingManager.main.DamageBuilding(Mathf.RoundToInt(travelDir.x), Mathf.RoundToInt(travelDir.z));
            _rb.velocity = Vector3.zero;
        }
        else
        {
            Vector3 direction = _movePos - transform.position;
            _rb.velocity = direction.normalized;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)
            && transform.position.z > 10)
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

        Vector3Int endPos = new Vector3Int(MapManager.main.Width - 1, 0, startPos.z/*MapManager.main.Height / 2*/);
        /*
        int shortestDistance = int.MaxValue;
        for (int i = 0; i < MapManager.main.Height; i++)
        {
            Vector3Int tempEndPos = new Vector3Int(MapManager.main.Width - 1, 0, i);
            List<Node> testPathfind = Pathfinder.Pathfind(startPos, tempEndPos, MapManager.main.MapData);
            if (testPathfind.Count < shortestDistance)
            {
                shortestDistance = testPathfind.Count;
                endPos = tempEndPos;
            }
        }
        */
        if (startPos == endPos)
        {
            Destroy(gameObject);
        }

        List<Node> freeNodes = Pathfinder.Pathfind(startPos, endPos, MapManager.main.MapData, false);
        List<Node> destroyWallNodes = Pathfinder.Pathfind(startPos, endPos, MapManager.main.MapData, true);

        //print("Free: " + Node.NodeDistance(freeNodes) + " vs Wall: " + Node.NodeDistance(destroyWallNodes));

        List<Node> nodes;
        if (Node.NodeDistance(freeNodes) > Node.NodeDistance(destroyWallNodes) || !CanReach() || freeNodes[^1].Position.x < destroyWallNodes[^1].Position.x)
        {
            _mode = MoveMode.Attack;
            nodes = destroyWallNodes;
        }
        else
        {
            _mode = MoveMode.Passive;
            nodes = freeNodes;
        }

        _positions = new Vector3[nodes.Count];
        for (int i = 0; i < _positions.Length; i++)
        {
            _positions[i] = new Vector3(nodes[i].Position.x, 0.5f, nodes[i].Position.z);
        }

        UpdateTarget();
    }

    public void Move()
    {
        if (_movePos != null && _positions != null && _positions.Length >= 2)
        {
            if (Vector3.Distance(transform.position, _movePos) < 0.01f)
            {
                transform.position = _movePos;
                //UpdatePath();
                if (transform.position.x == MapManager.main.Width - 1)
                {
                    GameManager.main.LoseLife(1);
                    Destroy(gameObject);
                }
                UpdateTarget();
            }
            //_movePos = _positions[1];
        }
    }

    public void TakeDamage(int damageAmount)
    {
        StartCoroutine(DamageFlash());
        _health -= damageAmount;
        if (_health <= 0)
        {
            PlayerManager.main.ModifyCurrency(50);
            Destroy(gameObject);
        }
    }

    private IEnumerator DamageFlash()
    {
        GetComponent<Renderer>().material = _dmgMat;
        yield return new WaitForSeconds(0.1f);
        GetComponent<Renderer>().material = _baseMat;
    }

    /*
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
    */

    public bool CanReach()
    {
        Vector3Int startPos = new Vector3Int(Mathf.RoundToInt(transform.position.x), 0, Mathf.RoundToInt(transform.position.y));
        Vector3Int endPos = new Vector3Int(MapManager.main.Width - 1, 0, startPos.y);

        List<Node> nodes = Pathfinder.Pathfind(startPos, endPos, MapManager.main.MapData, false);
        if (nodes[^1].Position == endPos)
        {
            return true;
        }

        return false;
    }

    public void UpdateTarget()
    {
        for (int i = _positions.Length - 1; i >= 0; i--)
        {
            Vector3 targetPos = new(_positions[i].x, 0.5f, _positions[i].z);

            Vector3 direction = (targetPos - transform.position);

            if (direction.magnitude == 1 && MapManager.main.MapData[Mathf.RoundToInt(targetPos.x), Mathf.RoundToInt(targetPos.z)] >= 0 && _mode == MoveMode.Attack)
            {
                _movePos = new(Mathf.RoundToInt(targetPos.x), 0.5f, Mathf.RoundToInt(targetPos.z));
                break;
            }

            if (direction.magnitude > 0.01 && !Physics.Raycast(transform.position, direction, direction.magnitude, _collisionMask))
            {
                _movePos = new(Mathf.RoundToInt(targetPos.x), 0.5f, Mathf.RoundToInt(targetPos.z));
                break;
            }
        }
    }
}
