using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBehaviour : MonoBehaviour
{
    private enum AttackType { Close, Farthest, Earliest, Healthiest, All, None }

    [SerializeField]
    private LayerMask _enemyMask;

    [Header("Building Stats")]
    [SerializeField]
    private int _maxHealth;
    private int _health;
    [SerializeField]
    private int _damage;
    [SerializeField]
    private float _radius;
    [SerializeField]
    private AttackType _type;
    [SerializeField]
    private float _attackCooldown;

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

    public bool TakeDamage(int damage)
    {
        _health -= damage;

        float damageRatio = (float)_health / _maxHealth;
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

        Transform target = null;

        foreach (Collider curObj in collidersInRadius)
        {
            if (target == null)
            {
                target = curObj.transform;
            }
            else
            {
                switch (_type)
                {
                    case AttackType.Close:
                        if (Vector3.Distance(curObj.transform.position, transform.position) < Vector3.Distance(target.position, transform.position))
                        {
                            target = curObj.transform;
                        }
                        break;
                    case AttackType.Farthest:
                        if (target == null || target.position.x < curObj.transform.position.x)
                        {
                            target = curObj.transform;
                        }
                        break;
                    case AttackType.Earliest:
                        if (target == null || target.position.x > curObj.transform.position.x)
                        {
                            target = curObj.transform;
                        }
                        break;
                    case AttackType.Healthiest:
                        if (target == null || target.GetComponent<EnemyBehaviour>().Health < curObj.transform.GetComponent<EnemyBehaviour>().Health)
                        {
                            target = curObj.transform;
                        }
                        break;
                }
            }
        }

        if (target != null)
        {
            target.GetComponent<EnemyBehaviour>().TakeDamage(_damage);
            yield return new WaitForSeconds(_attackCooldown);
        }        

        yield return null;
        StartCoroutine(Attack());
    }

    private void OnMouseDown()
    {
        if (Time.timeScale != 0 && BuildingManager.main._currentBuilding == BuildingManager.BuildingType.destroy)
        {
            PlayerManager.main.ModifyCurrency(20);
            BuildingManager.main.DestroyBuilding(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        }
    }
}
