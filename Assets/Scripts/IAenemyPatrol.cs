using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IAenemyPatrol : MonoBehaviour
{
    enum State
    {
        Patrolling,

        Chasing,

        Waiting,

        Attacking
    }

    State _currentState;

    NavMeshAgent _enemyAgent;
    Transform _playerTransform;
    //[SerializeField] Transform _patrolAreaCenter;
    [SerializeField] Vector2 _patrolAreaSize;

    [SerializeField] float _visionsRange = 15;
    [SerializeField] float _visionAngle = 90;

    Vector3 _lastTargetPosition;

    float _searchTimer;
    [SerializeField]float _searchWaitTime = 15;
    [SerializeField]float _searchRadius = 30;

    [SerializeField] Transform[] _patrolPoints;
    [SerializeField] float _speed = 2f;
    private int _currentPatrolIndex = 0;
    private float _waitTime = 5f;
    [SerializeField] float _attackRange = 2f;
    [SerializeField] private float _waitAttackTime = 2.0f;
    [SerializeField] private float _timer = 0.0f;

    void Awake()
    {
        _enemyAgent = GetComponent<NavMeshAgent>();
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start()
    {
        _currentState = State.Patrolling;
    }

    void Update()
    {
        switch (_currentState)
        {
            case State.Patrolling:
                Patrol();
            break;
            case State.Chasing:
                Chase();
            break;
            case State.Waiting:
                Wait();
            break;
            case State.Attacking:
                Attack();
            break;
        }
    }

    void Patrol()
    {
        if(OnRange() == true)
        {
            _currentState = State.Chasing;
        }
        
        if(_enemyAgent.remainingDistance < 0.5f)
        {
            ToPoint();
        }
    }

    void Chase()
    {
        _enemyAgent.destination = _playerTransform.position;

        if (OnRange())
        {
          if (Vector3.Distance(transform.position, _playerTransform.position) < _attackRange)
            {
                _currentState = State.Attacking;
            }  
        }
    }

    

    void Wait()
    {
        StartCoroutine(DoWait());
    }

    System.Collections.IEnumerator DoWait()
    {
        yield return new WaitForSeconds(_waitTime);
        _currentState = State.Patrolling;
    }

    void Attack()
    {
        _timer += Time.deltaTime;

        if (_timer > _waitAttackTime)
        {
            Debug.Log("bombazo");
            _timer = 0;
        }
        Debug.Log("bombazo");

        _currentState = State.Chasing;
    }

    void ToPoint()
    {
        Transform target = _patrolPoints[_currentPatrolIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, _speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 1f)
        {
            _currentState = State.Waiting;
            _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length;
            
            if (_currentPatrolIndex == 3)
            {
                _currentPatrolIndex = 0;
            }
        }

        if(OnRange() == true)
        {
            _currentState = State.Chasing;
        }
    }

    /*void SetRandomPoint()
    {
        float randomX = Random.Range(-_patrolAreaSize.x / 2, _patrolAreaSize.x / 2);
        float randomZ = Random.Range(-_patrolAreaSize.y / 2, _patrolAreaSize.y / 2);
        Vector3 _randomPoint = new Vector3(randomX, 0f, randomZ) + _patrolAreaCenter.position;

        _enemyAgent.destination = _randomPoint;
    }*/

    bool OnRange()
    {
        /* ia simple
        if(Vector3.Distance(transform.position, _playerTransform.position) <= _visionsRange)
        {
            return true;
        }

        return false;*/

        Vector3 _directionToPlayer = _playerTransform.position - transform.position;
        float _distanceToPlayer = _directionToPlayer.magnitude;
        float _angleToPlayer = Vector3.Angle(transform.forward, _directionToPlayer);

        if(_distanceToPlayer <= _visionsRange && _angleToPlayer < _visionAngle * 0.5f)
        {
            if(_playerTransform.position == _lastTargetPosition)
            {
                return true;
            }

            RaycastHit _hit;
            if(Physics.Raycast(transform.position, _directionToPlayer, out _hit, _distanceToPlayer))
            {
                if(_hit.collider.CompareTag("Player"))
                {
                    _lastTargetPosition = _playerTransform.position;
                    return true;
                }
            }

            return false;

        }

        return false;
    }

    /*void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_patrolAreaCenter.position, new Vector3(_patrolAreaSize.x, 0, _patrolAreaSize.y));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _visionsRange);

        Gizmos.color = Color.green;
        Vector3 _fovLine1 = Quaternion.AngleAxis(_visionAngle * 0.5f, transform.up) * transform.forward * _visionsRange;
        Vector3 _fovLine2 = Quaternion.AngleAxis(-_visionAngle * 0.5f, transform.up) * transform.forward * _visionsRange;
        Gizmos.DrawLine(transform.position, transform.position + _fovLine1);
        Gizmos.DrawLine(transform.position, transform.position + _fovLine2);
    }*/
}
