using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private Transform playerTarget;
    [SerializeField]
    private bool zombie, bat, ghost;
    [SerializeField]
    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        if (zombie == true)
        {
            ZombieMoveTo();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        if (bat == true || ghost == true)
            transform.position = Vector3.MoveTowards(transform.position, playerTarget.position, MainController.step);
    }

    public void ZombieMoveTo()
    {
        agent.destination = playerTarget.position;
    }
}
