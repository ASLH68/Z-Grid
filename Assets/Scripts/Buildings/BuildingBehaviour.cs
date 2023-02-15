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

    [SerializeField]
    private Material _buildingMat;

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

        float damageRatio = (float)_health / (float)_maxHealth;
        Material brokenMat = new Material(_buildingMat);
        brokenMat.color *= 0.25f;

        GetComponent<Renderer>().material.color = _buildingMat.color * damageRatio + brokenMat.color * (1 - damageRatio);
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
            farthest.GetComponent<EnemyBehaviour>().TakeDamage(25);
            yield return new WaitForSeconds(_attackCooldown);
        }

        yield return null;
        StartCoroutine(Attack());
    }
}
