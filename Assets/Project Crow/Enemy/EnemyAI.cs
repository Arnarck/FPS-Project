using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    Transform target;
    NavMeshAgent nav_mesh_agent;

    // Start is called before the first frame update
    void Start()
    {
        target = GI.player.transform;
        nav_mesh_agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        nav_mesh_agent.SetDestination(target.position);
    }
}
