using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    float current_health;
    bool is_provoked;

    Transform target;
    NavMeshAgent nav_mesh_agent;
    Animator animator;

    [SerializeField] float detection_range = 4f;
    [SerializeField] float look_speed = 10f;
    [SerializeField] float starting_health = 100f;
    [SerializeField] float max_health = 100f;
    [SerializeField] float damage = 25f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        nav_mesh_agent = GetComponent<NavMeshAgent>();
        current_health = starting_health;
    }

    void Start()
    {
        target = GI.player.transform;
    }

    void Update()
    {
        Vector3 relative_position = target.position - transform.position;
        float distance_from_target = Util.distance(transform.position, target.position);

        if (is_provoked && distance_from_target <= nav_mesh_agent.stoppingDistance) // Makes the enemy attack
        {
            Vector3 look_position = new Vector3(relative_position.x, 0f, relative_position.z); // Allows the enemy to look only on X and Z axis.
            Quaternion look_rotation = Quaternion.LookRotation(look_position);

            transform.rotation = Quaternion.Slerp(transform.rotation, look_rotation, look_speed * Time.deltaTime);

            animator.SetBool("isWalking", false);
            animator.SetTrigger("Attack");

            //Physics.Over
        }
        else if (is_provoked) // Makes the enemy chase the player
        {
            Vector3 look_position = new Vector3(relative_position.x, 0f, relative_position.z); // Allows the enemy to look only on X and Z axis.
            Quaternion look_rotation = Quaternion.LookRotation(look_position);

            transform.rotation = Quaternion.Slerp(transform.rotation, look_rotation, look_speed * Time.deltaTime);
            nav_mesh_agent.SetDestination(target.position);
            animator.SetBool("isWalking", true);
        }
        else if (distance_from_target <= detection_range) // Checks if the target is close enough
        {
            // Checks if there is an wall between player and enemy
            RaycastHit hit;
            bool has_hit_colliders = Physics.Raycast(transform.position, relative_position.normalized, out hit, distance_from_target);
            Debug.DrawRay(transform.position, relative_position, Color.green);

            if (has_hit_colliders && hit.collider.CompareTag("Player")) is_provoked = true;
        }
    }

    public void attack_player()
    {
        GI.player.change_health_amount(-damage);
    }

    public void take_damage(float value)
    {
        if (value < Mathf.Epsilon) value *= -1;

        current_health -= value;
        if (current_health < Mathf.Epsilon)
        {
            Destroy(gameObject);
        }
        else
        {
            RaycastHit hit;
            Vector3 relative_position = target.position - transform.position;
            bool has_hit_colliders = Physics.Raycast(transform.position, relative_position.normalized.normalized, out hit, Mathf.Infinity);
            Debug.DrawRay(transform.position, relative_position, Color.green);

            if (has_hit_colliders && hit.collider.CompareTag("Player")) is_provoked = true;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detection_range);
    }
}
