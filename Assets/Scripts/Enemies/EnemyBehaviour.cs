using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyBehaviour : MonoBehaviour
{
    private enum MoveMode { Attack, Passive }

    private Rigidbody _rb;

    [SerializeField]
    private Vector3[] _positions;
    [SerializeField]
    private Vector3 _movePos;

    [Header("Enemy Data")]
    [SerializeField]
    private int _maxHealth;
    private int _health;
    [SerializeField]
    private int _moneyAmount;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private int _damage;
    [SerializeField]
    private float _sightRange;
    [SerializeField]
    private float _pathSightRange;
    [SerializeField]
    private int _intelligence;
    [SerializeField]
    private bool _willExplode;

    [SerializeField]
    private LayerMask _collisionMask;
    [SerializeField]
    private MoveMode _mode;

    [SerializeField]
    private Material _baseMat;
    [SerializeField]
    private Material _dmgMat;
    private bool _hurted = false;
    [SerializeField]
    private GameObject _warning;

    public int Health => _health;

    // Start is called before the first frame update
    void Start()
    {
        _mode = MoveMode.Passive;
        _health = _maxHealth;
        _rb = GetComponent<Rigidbody>();
        if (_warning != null)
        {
            StartCoroutine(WarningFlash());
        }
        UpdatePath();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        
        if (MapManager.main.MapData[Mathf.RoundToInt(_movePos.x), Mathf.RoundToInt(_movePos.z)] > 0)
        {
            if (_willExplode)
            {
                Explode();
            }
            Vector3 travelDir = transform.position + (_movePos - new Vector3(Mathf.RoundToInt(transform.position.x), 0.5f, Mathf.RoundToInt(transform.position.z))).normalized;

            BuildingManager.main.DamageBuilding(Mathf.RoundToInt(travelDir.x), Mathf.RoundToInt(travelDir.z), _damage);

            if (Vector3.Distance(transform.position, _movePos) > 1.01f)
            {
                Vector3 direction = _movePos - transform.position;
                _rb.velocity = direction.normalized * _speed * 0.5f;
            }
            else
            {
                _rb.velocity = Vector3.zero;
            }
        }
        else if (MapManager.main.MapData[Mathf.RoundToInt(_movePos.x), Mathf.RoundToInt(_movePos.z)] == 0)
        {
            Vector3 direction = _movePos - transform.position;
            _rb.velocity = direction.normalized * _speed * 0.5f;
        }
        else
        {
            _rb.velocity = Vector3.zero;
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
        List<Node> freeNodes = Pathfinder.Pathfind(startPos, endPos, MapManager.main.MapData, false, _intelligence);
        List<Node> destroyWallNodes = Pathfinder.Pathfind(startPos, endPos, MapManager.main.MapData, true, _intelligence);

        List<Node> nodes;
        if (freeNodes[^1].Position != endPos || destroyWallNodes[^1].Position != endPos)
        {
            _mode = MoveMode.Attack;
            List<Vector3Int> nodePos = new();

            Vector3Int curPos = startPos;

            while (curPos != endPos)
            {
                if (curPos.x != endPos.x)
                {
                    curPos += (int)Mathf.Sign(endPos.x - curPos.x) * Vector3Int.right;
                }
                else if (curPos.z != endPos.z)
                {
                    curPos += (int)Mathf.Sign(endPos.z - curPos.z) * Vector3Int.forward;
                }

                nodePos.Add(curPos);
            }

            _positions = new Vector3[nodePos.Count];

            for (int i = 0; i < _positions.Length; i++)
            {
                _positions[i] = new Vector3(nodePos[i].x, 0.5f, nodePos[i].z);
            }
        }
        else
        {
            if (Node.NodeDistance(freeNodes) > Node.NodeDistance(destroyWallNodes) || freeNodes[^1].Position != endPos)
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
        }

        if (MapManager.main.MapData[Mathf.RoundToInt(_positions[1].x), Mathf.RoundToInt(_positions[1].z)] == 0
            && MapManager.main.MapData[Mathf.RoundToInt(_positions[0].x), Mathf.RoundToInt(_positions[0].z)] == 0)
        {
            _movePos = _positions[1];
        }
        else
        {
            _movePos = _positions[0];
        }
        UpdateTarget();
    }

    public void Move()
    {
        if (_movePos == null || _movePos == Vector3.zero)
        {
            _movePos = _positions[0];
        }

        if (Vector3.Distance(transform.position, _movePos) < 0.05f)
        {
            transform.position = _movePos;
            //UpdatePath();
            if (transform.position.x == MapManager.main.Width - 1 && !_hurted)
            {
                GameManager.main.LoseLife(_damage);
                _hurted = true;
                Destroy(gameObject);
            }
        }
        UpdateTarget();
        //_movePos = _positions[1];
    }

    public void TakeDamage(int damageAmount)
    {
        StartCoroutine(DamageFlash());
        _health -= damageAmount;
        if (_health <= 0)
        {
            PlayerManager.main.ModifyCurrency(_moneyAmount);
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

    public void UpdateTarget()
    {
        /*
        for (int i = _positions.Length - 1; i >= IndexOf(_movePos); i--)
        {
            Vector3 targetPos = new(_positions[i].x, 0.5f, _positions[i].z);
            Vector3 direction = (targetPos - transform.position);

            _whatAmIDoing += "\n\n " + Physics.Raycast(transform.position, direction, direction.magnitude, _collisionMask);
            if (!Physics.Raycast(transform.position, direction, direction.magnitude, _collisionMask))
            {
                _movePos = new(Mathf.RoundToInt(targetPos.x), 0.5f, Mathf.RoundToInt(targetPos.z));
                return;
            }

            /*
            if (direction.magnitude > 0.01 && !Physics.Raycast(transform.position, direction, direction.magnitude, _collisionMask))
            {
                _movePos = new(Mathf.RoundToInt(targetPos.x), 0.5f, Mathf.RoundToInt(targetPos.z));
                return;
            }

            if (direction.magnitude - 1 <= 0.01f && MapManager.main.MapData[Mathf.RoundToInt(targetPos.x), Mathf.RoundToInt(targetPos.z)] >= 0)
            {
                _movePos = new(Mathf.RoundToInt(targetPos.x), 0.5f, Mathf.RoundToInt(targetPos.z));
                return;
            }
            
        }
        */
        if (transform.position == _movePos || _movePos == new Vector3(0, 0.5f, 0))
        {
            Vector3 oldPos = _movePos;
            _movePos = _positions[Mathf.Min(IndexOf(_movePos) + 1, _positions.Length - 1)];
        }
    }

    public bool WithinRange(Vector3 pos)
    {
        if (Vector3.Distance(pos, transform.position) <= _sightRange)
        {
            return true;
        }

        foreach (Vector3 curPos in _positions)
        {
            if (Vector3.Distance(pos, curPos) <= _pathSightRange)
            {
                return true;
            }
        }

        return false;
    }

    public int IndexOf(Vector3 testPos)
    {
        for (int i = 0; i < _positions.Length; i++)
        {
            if (_positions[i] == testPos)
            {
                return i;
            }
        }

        return 0;
    }

    public void Explode()
    {
        for (int x = -Mathf.RoundToInt(_sightRange); x < Mathf.RoundToInt(_sightRange); x++)
        {
            for (int y = -Mathf.RoundToInt(_sightRange); y < Mathf.RoundToInt(_sightRange); y++)
            {
                Vector3Int avgPos = new Vector3Int(Mathf.RoundToInt(transform.position.x) + x, 0, Mathf.RoundToInt(transform.position.z) + y);

                if (MapManager.main.InBounds(new Vector3Int(avgPos.x, avgPos.z))
                    && Vector3.Distance(transform.position, new Vector3(avgPos.x, 0.5f, avgPos.z)) <= _sightRange)
                {
                    BuildingManager.main.DamageBuilding(avgPos.x, avgPos.z, 5000);
                }
            }
        }

        TakeDamage(_health);
    }

    public void ApplyChanges(int roundNum)
    {
        switch (_health)
        {
            case 100:
                _health *= Mathf.RoundToInt(Mathf.Pow(1.25f, Mathf.Floor(roundNum / 10f)));
                _damage *= Mathf.RoundToInt(Mathf.Pow(1.25f, Mathf.Floor(roundNum / 10f)));
                break;
            case 75:
                _health *= Mathf.RoundToInt(Mathf.Pow(1.5f, Mathf.Floor(roundNum / 10f)));
                _damage *= Mathf.RoundToInt(Mathf.Pow(1.5f, Mathf.Floor(roundNum / 10f)));
                break;
            case 250:
                _health *= Mathf.RoundToInt(Mathf.Pow(1.15f, Mathf.Floor(roundNum / 10f)));
                _damage *= Mathf.RoundToInt(Mathf.Pow(1.15f, Mathf.Floor(roundNum / 10f)));
                break;
            case 200:
                _health *= Mathf.RoundToInt(Mathf.Pow(1.1f, Mathf.Floor(roundNum / 10f)));
                break;
        }
    }

    private IEnumerator WarningFlash()
    {
        while (true)
        {
            _warning.SetActive(!_warning.activeSelf);
            yield return new WaitForSeconds(0.75f);
        }
    }
}
