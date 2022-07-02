using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    float current_health;
    bool is_provoked;

    Transform target;
    NavMeshAgent nav_mesh_agent;

    [SerializeField] float detection_range = 4f;
    [SerializeField] float look_speed = 10f;
    [SerializeField] float starting_health = 100f;
    [SerializeField] float max_health = 100f;

    void Awake()
    {
        nav_mesh_agent = GetComponent<NavMeshAgent>();
        current_health = starting_health;
    }

    void Start()
    {
        target = GI.player.transform;
    }

    void Update()
    {
        float distance_from_target = Util.distance(transform.position, target.position);

        if (is_provoked && distance_from_target <= nav_mesh_agent.stoppingDistance) // Process enemy attack
        {
            Vector3 relative_position = target.position - transform.position;
            Vector3 look_position = new Vector3(relative_position.x, 0f, relative_position.z); // Allows the enemy to look only on X and Z axis.
            Quaternion look_rotation = Quaternion.LookRotation(look_position);

            transform.rotation = Quaternion.Slerp(transform.rotation, look_rotation, look_speed * Time.deltaTime);

            // TODO Make the enemy attack

            Debug.Log("Attack!");
        }
        else if (is_provoked) // Process enemy chasing
        {
            Vector3 relative_position = target.position - transform.position;
            Vector3 look_position = new Vector3(relative_position.x, 0f, relative_position.z); // Allows the enemy to look only on X and Z axis.
            Quaternion look_rotation = Quaternion.LookRotation(look_position);

            transform.rotation = Quaternion.Slerp(transform.rotation, look_rotation, look_speed * Time.deltaTime);
            nav_mesh_agent.SetDestination(target.position);
        }
        else if (distance_from_target <= detection_range) // Checks if the target is close enough
        {
            // TODO Process Raycast
            is_provoked = true;
        }
    }

    // TODO Make the enemy chase the player when it takes damage
    public void TakeDamage(float value)
    {
        if (value < Mathf.Epsilon) value *= -1;

        current_health -= value;
        if (current_health < Mathf.Epsilon)
        {
            // Die
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detection_range);
    }
}
