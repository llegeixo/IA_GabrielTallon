using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//Hay que poner esto para que funcione el NavMeshAgent.

public class RepasoExamen : MonoBehaviour
{
    //Para crear los estados
    enum State
    {
        Patrolling,
        Chasing,
        Attacking
    }
    
    //Para almacenar el estado actual entre los anteriores.
    private State _currentState;

    //Para que la IA pueda interactuar con el mapa.
    private NavMeshAgent _agent;
    
    //Variable que almacena la posición del jugador.
    private Transform _player;

    //Almacena los puntos los cuales pasará la IA para patrullar.
    [SerializeField] private Transform[] _patrolPoints;

    //Variable de tamaño de area de detección del jugador.
    [SerializeField] private float _detectionRange = 15;

    //Variable de tamaño de area de ataque.
    [SerializeField] private float _attackingRange = 5;

    //Para activar las siguientes variables.
    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        // Para almacenar la posición del jugador directamente desde el script. Esto se podria hacer desde el inspector de Unity, pero en este caso lo hemos hecho así. Se tiene que asignar un TAG al jugador para que funcione.
        _player = GameObject.FindWithTag("Player").transform;
    }
   
    void Start()
    {
        //Esto es para que se dirija hacia un punto de patrulla aleatorio.
        SetRandomPoint();
        //Esto indica el estado con el que se empieza.
        _currentState = State.Patrolling;
    }

    
    void Update()
    {
        //Esto controla que código ejecuta según el estado. Para cambiar el estado actual. Para esto hay que crear nuevas funciones para cada estado.
        switch (_currentState)
        {
            case State.Patrolling:
                Patrol();
            break;
            case State.Chasing:
                Chase();
            break;
            case State.Attacking:
                Attack();
            break;
        }
    }

    //Función creada para el estado. Aqui se escribe lo que conlleva estar en el estado a continuación.
    void Patrol()
    {
        //Esto hace que el enemigo pase al estado de chasing en base a la función IsInRange.
        if(IsInRange(_detectionRange) == true)
        {
            _currentState = State.Chasing;
        }

        //Esto comprueba que si ha llegado al punto de patrulla hará lo siguiente:
        if(_agent.remainingDistance < 0.5f)
        {
            //Código escrito en la función start, simplemente es copiar y pegar. Esto hace que al llegar al sitio de patrulla escoge otro punto aleatorio al que ir.
            SetRandomPoint();
        }
    }

    //Función creada para el estado. Aqui se escribe lo que conlleva estar en el estado a continuación.
    void Chase()
    {
        //Esto es lo mismo que hemos hecho antes para pasar a este estado, pero esta vez lo ponemos en false.
        if(IsInRange(_detectionRange) == false)
        {
            SetRandomPoint();
            _currentState = State.Patrolling;
        }

        //Si la distancia es menor que el attackingRange entre el personaje y el enemigo, se pasa al estado de ataque.
        if(IsInRange(_attackingRange) == true)
        {
            _currentState = State.Attacking;
        }

        //Esto hace que el enemigo vaya a la posición del jugador.
        _agent.destination = _player.position;
    }

    //Función creada para el estado. Aqui se escribe lo que conlleva estar en el estado a continuación.
    void Attack()
    {
        //Esto simula un ataque.
        Debug.Log("Atacando");

        //Esto hace que ataque y directamente pase al estado de chasing.
        _currentState = State.Chasing;
    }

    //Función para que patrulle a un punto aleatorio.  El [Random.Range(0, _patrolPoints.Length)] indica [Un numero random.En el rango (desde la variable almacenada en la Ray número 0, hasta la cantidad máxima de variables almacenadas en la misma - 1 para que no de errores por tema de como se cuenta en programación)]
    void SetRandomPoint()
    {
        _agent.destination = _patrolPoints[Random.Range(0, _patrolPoints.Length - 1)].position;
    }

    //Esto es una función para comprobar si esta dentro de un rango, ya sea el de ataque o el de detección. Si esta dentro, que sea true, si esta fuera, que sea false.
    bool IsInRange(float range)
    {
        if(Vector3.Distance(transform.position, _player.position) < range)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    //Esta función no hará falta escribirlo en el examen, a lo mejor nos llevamos un punto extra. Esto dibuja círculos azules en los puntos de patrulla del escenario.
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        foreach(Transform point in _patrolPoints)
        {
            Gizmos.DrawWireSphere(point.position, 1f);
        }

        //Para dibujar un gizmo en el detectionRange;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        //Para dibujar un gizmo en el attackingRange;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackingRange);        
    }
}
