using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Drone : MonoBehaviour
{
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float detectionRadius = 20f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float attackCooldown = 2.0f;
    
    public float travelForce = 10f;
    
    private float _rayDistance = 5f;
    private float _stoppingDistance = 1.5f;
    private float _lastAttackTime = 0f;
    
    private Vector3 _lastTestPosition;
    private Vector3 _destination;
    private Quaternion _desiredRotation;
    private Vector3 _direction;
    private Vector3 _lastPlayerPosition;
    private Vector3 _playerVelocity;
    private Transform _target;
    private DroneState _currentState;
    public LayerMask layerMask;
    private BaseEnemy _baseEnemy;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public Projectile projectile;
    
    public enum DroneState
    {
        Patrol,
        Chase,
        Attack
    }

    private void Start()
    {
        _baseEnemy = GetComponent<BaseEnemy>();
    }

    private void FixedUpdate()
    {
        switch (_currentState)
        {
            case DroneState.Patrol:
                CheckForPlayer();
                HandlePatrol();
                break;
            case DroneState.Chase:
                CheckForPlayer();
                HandleChase(_target);
                break;
            case DroneState.Attack:
                if (_baseEnemy.isAlive)
                {
                    HandleAttack();
                }
                break;
        }
    }
    
    private void HandlePatrol()
    {
        if (NeedsDestination())
        {
            GetDestination();
        }
        else if (IsPathBlocked() && _clearDirs.Count != 0)
        {
            ChooseAndMoveToDirection(_destination);
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
            _currentState = DroneState.Patrol;
            return;
        }
        
        if (IsPathBlocked() && _clearDirs.Count != 0)
        {
            ChooseAndMoveToDirection(target.position);
        }
        else
        {
            MoveToDestination(target.position);
        }
        
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        
        if (distanceToTarget <= attackRange)
        {
            _currentState = DroneState.Attack;
        }
    }
    
    
    private void HandleAttack()
    {
        if (_target == null)
        {
            _currentState = DroneState.Patrol;
            return;
        }
        
        //UpdatePlayerVelocity();

        // predict the future position of the player (not in use its OP)
        /*float projectileTravelTime = (attackRange / projectile.speed); 
        Vector3 predictedPosition = _target.position + _playerVelocity * projectileTravelTime;

        Vector3 targetDirection = (predictedPosition - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, predictedPosition);*/
        
        Vector3 targetDirection = (_target.position - transform.position).normalized; 
        float distanceToTarget = Vector3.Distance(transform.position, _target.position);
        

        if (distanceToTarget > attackRange)
        {
            _currentState = DroneState.Chase;
        }
    
        LookAtTarget(_target.position); 
        
        if (Time.time > _lastAttackTime + attackCooldown)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.LookRotation(targetDirection));
            Projectile projectileScript = bullet.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.SetDamage(_baseEnemy.damage);  // Pass the scaled damage to the projectile
            }
            _lastAttackTime = Time.time;
        }
    }
    
    private void MoveDroneBackwards(Vector3 backwardDirection)
    {
        Debug.Log("Hit a Wall! Will turn around.");
        _desiredRotation = Quaternion.LookRotation(backwardDirection);
        rb.rotation = Quaternion.Slerp(rb.rotation, _desiredRotation, 0.1f);
    }
    
    private void MoveToDestination(Vector3 destination)
    {
        Vector3 targetDir = (destination - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, 0.3f);
        rb.AddForce(targetDir * travelForce, ForceMode.Force);
    }
    
    private void LookAtTarget(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        rb.rotation = Quaternion.Slerp(rb.rotation, lookRotation, Time.deltaTime * speed);
    }
    
    List<Vector3> _clearDirs = new List<Vector3>();
    private bool IsPathBlocked()
    {
        _clearDirs.Clear();
        bool isBlocked = false;
        int numberOfRays = 10;
        float spreadAngle = 70f;
        float angleStep = spreadAngle / (numberOfRays - 1);
    
        float startAngle = -spreadAngle / 2;

        for (int i = 0; i < numberOfRays; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            Ray ray = new Ray(transform.position, direction);
        
            if (!Physics.Raycast(ray, out RaycastHit hit, _rayDistance, layerMask))
            {
                Debug.DrawRay(transform.position, direction * _rayDistance, Color.green);
                _clearDirs.Add(direction);
            }
            else
            {
                Debug.DrawRay(transform.position, direction * _rayDistance, Color.red);
                isBlocked = true;
            }
        }

        return isBlocked;
    }
    
    private void ChooseAndMoveToDirection(Vector3 destination)
    {
        //d are the directions from the list. Its being turned into a position by adding transform.postion.
        //destination - (transform.position + d) gets the vector from the current position to the destination 
        //sqrMagnitude gets the length of each direction. The list will be ordered by which vector is the shortest.
        //After sorting, the first direction in the list will be the shortest direction from the drone to the destination
        Vector3 bestDirection = _clearDirs.OrderBy(d => (destination - (transform.position + d)).sqrMagnitude).First();
        //creates an actual position from the chosen direction
        Vector3 newPosition = transform.position + bestDirection;
        Debug.DrawRay(transform.position, bestDirection, Color.blue, 2f);
        MoveToDestination(newPosition);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_destination, 1f);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private void GetDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 20f;
        randomDirection.y = 0f;

        Vector3 testPosition = transform.position + randomDirection;

        if (!Physics.CheckSphere(testPosition, 1f, layerMask) &&
            !Physics.Linecast(transform.position, testPosition, layerMask))
        {
            _destination = testPosition;
        }
    }
    
    private bool NeedsDestination()
    {
        return _destination == Vector3.zero || Vector3.Distance(transform.position, _destination) <= _stoppingDistance;
    }
    
    private Transform CheckForPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                Vector3 directionToTarget = (hitCollider.transform.position - transform.position).normalized;
                float distanceToTarget = Vector3.Distance(transform.position, hitCollider.transform.position);
                
                if (Physics.Raycast(transform.position, directionToTarget, out RaycastHit hitInfo, distanceToTarget))
                {
                    if (hitInfo.collider.CompareTag("Player"))
                    {
                        _target = hitCollider.transform;
                        _currentState = DroneState.Chase;
                        return _target;
                    }
                }
                
            }
        }
        return _target = null;
    }
    private void UpdatePlayerVelocity()
    {
        if (_target != null)
        {
            _playerVelocity = (_target.position - _lastPlayerPosition) / Time.fixedDeltaTime;
            _lastPlayerPosition = _target.position;
        }
    }
}


