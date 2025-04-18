using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class SpiderTurret : MonoBehaviour
{

    [SerializeField] private float attackRange = 12f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float detectionRadius = 50f;
    [SerializeField] private int damage = 10;
    public float patrolRadius = 50f;
    
    private float _lastAttackTime;
    private float _stoppingDistance = 1.5f;
    
    private Transform _target;
    private TurretState _currentState;
    private Vector3 _destination;
    private NavMeshAgent _agent;
    private BaseEnemy _baseStats;
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    
    public enum TurretState
    {
        Patrol,
        Chase,
        Attack
    }

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _baseStats = GetComponent<BaseEnemy>();
    }
    
    private void FixedUpdate()
    {
        switch (_currentState)
        {
            case TurretState.Patrol:
                CheckForPlayer();
                HandlePatrol();
                break;
            case TurretState.Chase:
                CheckForPlayer();
                HandleChase(_target);
                break;
            case TurretState.Attack:
                HandleAttack();   
                break;
        }
    }
    
    private void HandlePatrol()
    {
        if (!_agent.pathPending && _agent.remainingDistance < 1f)
        {
            GetDestination();
        }
        else
        {
            MoveToDestination(_destination);
        }
    }

    private void HandleChase(Transform target)
    {
        if (target == null)
        {
            _currentState = TurretState.Patrol;
            if (_agent != null)
                _agent.isStopped = true; 
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Move towards the target if outside of attack range
        if (distanceToTarget > attackRange)
        {
            MoveToDestination(target.position);
            if (_agent != null && _agent.isStopped)
                _agent.isStopped = false; 
        }
        else
        {
            if (_agent != null)
                _agent.isStopped = true;
        }

        LookAtTarget(target.position);
        
        if (distanceToTarget <= attackRange)
        {
            _currentState = TurretState.Attack;
        }
    }
    
    private void HandleAttack()
    {
        if (_target == null)
        {
            _currentState = TurretState.Patrol;
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, _target.position);
        if (distanceToTarget > attackRange)
        {
            _currentState = TurretState.Chase;
            return;
        }

        // Calculate the direction to the target with a vertical offset
        Vector3 adjustedTargetPosition = new Vector3(_target.position.x, 
            _target.position.y - 0.5f,  // Lower the aim by adjusting y-component
            _target.position.z);
        LookAtTarget(adjustedTargetPosition);  // Ensure LookAtTarget uses this adjusted position as well

        if (Time.time > _lastAttackTime + attackCooldown)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.LookRotation(adjustedTargetPosition - transform.position));
            Projectile projectileScript = bullet.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.SetDamage(_baseStats.damage);  // Pass the scaled damage to the projectile
            }
            _lastAttackTime = Time.time;
        }
    }


    
    private Transform CheckForPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                /*Vector3 directionToTarget = (hitCollider.transform.position - transform.position).normalized;
                float distanceToTarget = Vector3.Distance(transform.position, hitCollider.transform.position);
                
                if (Physics.Raycast(transform.position, directionToTarget, out RaycastHit hitInfo, distanceToTarget))
                {
                    if (hitInfo.collider.CompareTag("Player"))
                    {
                    }
                }*/
                
                _target = hitCollider.transform;
                _currentState = TurretState.Chase;
                return _target;
            }
        }
        return _target = null;
    }
    
    private void GetDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            _destination = hit.position;
            MoveToDestination(_destination);
        }
        else
        {
            Debug.Log("Failed to find a valid destination.");
        }
    }
    
    private void LookAtTarget(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 6f);
    }
    
    private void MoveToDestination(Vector3 targetDestination)
    {
        if (_agent != null)
        {
            _agent.SetDestination(targetDestination);
        }
    }
}
