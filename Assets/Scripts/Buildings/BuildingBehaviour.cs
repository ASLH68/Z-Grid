using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBehaviour : MonoBehaviour
{
    [SerializeField]
    private LayerMask _enemyMask;

    [SerializeField]
    private int _health;
    [SerializeField]
    private int _maxHealth;
    [SerializeField]
    private float _attackCooldown;
    [SerializeField]
    private float _radius;

    // Start is called before the first frame update
    void Start()
    {
        _health = _maxHealth;
        StartCoroutine(Attack());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool TakeDamage()
    {
        _health--;
        if (_health <= 0)
        {
            return true;
        }

        return false;
    }

    private IEnumerator Attack()
    {
        Collider[] collidersInRadius = Physics.OverlapSphere(transform.position, _radius, _enemyMask);

        Transform farthest = null;

        foreach (Collider curObj in collidersInRadius)
        {
            if (farthest == null || farthest.position.x < curObj.transform.position.x)
            {
                farthest = curObj.transform;
            }
        }

        if (farthest != null)
        {
            farthest.GetComponent<EnemyBehaviour>().TakeDamage(10);
            yield return new WaitForSeconds(_attackCooldown);
        }

        yield return null;
        StartCoroutine(Attack());
    }
}
