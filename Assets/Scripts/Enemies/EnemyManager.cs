using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Static")]
    public static EnemyManager main;

    [Header("Enemies")]
    [SerializeField]
    private EnemyBehaviour _enemyObj;
    private List<EnemyBehaviour> _enemies;

    [Header("Waves")]
    [SerializeField]
    private Wave[] _waves;

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

        StartCoroutine(SpawnWaves());
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


    public void SpawnEnemy()
    {
        int width = MapManager.main.Width;
        int height = MapManager.main.Height;

        transform.position = new Vector3(0, 0.5f, Random.Range(0, height));
        _enemies.Add(Instantiate(_enemyObj, transform.position, Quaternion.identity));
    }

    public void UpdatePaths()
    {
        foreach (EnemyBehaviour curEnemy in _enemies)
        {
            curEnemy.UpdatePath();
        }
    }

    private IEnumerator SpawnWaves()
    {
        foreach (Wave curWave in _waves)
        {
            for (int i = 0; i < curWave.enemyCount; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(curWave.time);
        }
    }
}

[System.Serializable]
public struct Wave
{
    public int enemyCount;
    public int time;
}