using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Static")]
    public static EnemyManager main;

    [Header("Enemies")]
    private List<EnemyBehaviour> _enemies;

    [SerializeField] private Transform _enemiesParent;

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

    // Start is called before the first frame update
    void Start()
    {
        _enemies = new();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach(EnemyBehaviour curEnemy in _enemies.ToArray())
        {
            if (curEnemy == null)
            {
                _enemies.Remove(curEnemy);
            }
            else
            {
                curEnemy.Move();
            }
        }
    }

    public void StartRound(Round round)
    {
        StartCoroutine(SpawnWaves(round));
    }

    public void SpawnEnemy(EnemyBehaviour enemyObj)
    {
        int width = MapManager.main.Width;
        int height = MapManager.main.Height;

        transform.position = new Vector3(0, 0.5f, /*13*/Random.Range(0, height));
        if (_enemies != null)
        {
            _enemies.Add(Instantiate(enemyObj, transform.position, Quaternion.identity));
        }
    }

    public void UpdatePaths(Vector3 pos)
    {
        foreach (EnemyBehaviour curEnemy in _enemies)
        {
            if (curEnemy != null && curEnemy.WithinRange(pos))
            {
                curEnemy.UpdatePath();
            }
        }
    }

    public void UpdatePaths(bool doAll)
    {
        foreach (EnemyBehaviour curEnemy in _enemies)
        {
            if (curEnemy != null)
            {
                curEnemy.UpdatePath();
            }
        }
    }

    private IEnumerator SpawnWaves(Round round)
    {
        foreach (Wave curWave in round.waves)
        {
            while (curWave.EnemyCount() > 0)
            {
                SpawnEnemy(curWave.GetRandomEnemy());
                yield return new WaitForSeconds(0.5f + Random.Range(0f, 0.5f));
            }
            yield return new WaitForSeconds(curWave.time);
        }
    }

    public bool AnyEnemiesLeft()
    {
        if (_enemies == null)
        {
            return true;
        }
        foreach(EnemyBehaviour curEnemy in _enemies.ToArray())
        {
            if (curEnemy == null)
            {
                _enemies.Remove(curEnemy);
            }
        }

        return _enemies.Count != 0;
    }
}