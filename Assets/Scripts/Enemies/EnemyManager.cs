using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Static")]
    public static EnemyManager main;

    [Header("Enemies")]
    private List<EnemyBehaviour> _enemies;
    [SerializeField]
    private EnemyBehaviour[] _enemyTypes;

    private int _pulseValue = 0;
    private int _enemyPulseCount = 0;

    [SerializeField] private Transform _enemiesParent;

    public EnemyBehaviour[] EnemyTypes => _enemyTypes;

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

    public void SpawnEnemy(EnemyBehaviour enemyObj, int pos)
    {
        transform.position = new Vector3(0, 0.5f, pos);

        if (_enemies != null)
        {
            EnemyBehaviour newEnemy = Instantiate(enemyObj, transform.position, Quaternion.identity);

            _enemies.Add(newEnemy);

            newEnemy.ApplyChanges(GameManager.main.CurrentRound);
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
        int width = MapManager.main.Width;
        int height = MapManager.main.Height;

        foreach (Wave curWave in round.waves)
        {
            yield return new WaitForSeconds(5);

            int totalEnemies = curWave.EnemyCount();
            int pulseEnemies = 0;
            int puleRow = 2 + 5 * Random.Range(0, 4);

            while (curWave.EnemyCount() > 0)
            {
                if (pulseEnemies < totalEnemies / 8)
                {
                    SpawnEnemy(curWave.GetRandomEnemy(), puleRow + Random.Range(-2, 3));
                    pulseEnemies++;
                }
                else
                {
                    SpawnEnemy(curWave.GetRandomEnemy(), Random.Range(0, height));
                }
                yield return new WaitForSeconds(0.25f);
            }

            while (AnyEnemiesLeft())
            {
                yield return new WaitForSeconds(1);
            }
            PlayerManager.main.ModifyCurrency(150);

            GameManager.main.UpdateCurrentRound();

            CanvasManager.main.UpdateRoundText();
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

    public bool EnemyOn(int x, int y)
    {
        foreach (EnemyBehaviour curEnemy in _enemies)
        {
            if (curEnemy != null)
            {
                Vector3 avgEnemyPos = new(Mathf.RoundToInt(curEnemy.transform.position.x), 0.5f, Mathf.RoundToInt(curEnemy.transform.position.z));

                if (avgEnemyPos == new Vector3(x, 0.5f, y))
                {
                    return true;
                }
            }
        }

        return false;
    }
}