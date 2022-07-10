using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    float current_health, time_elapsed_since_last_attack;
    bool is_alive = true, is_provoked, can_attack = true;

    Transform target;
    NavMeshAgent nav_mesh_agent;
    Animator animator;

    [SerializeField] GameObject loot_area;
    [SerializeField] float detection_range = 4f;
    [SerializeField] float look_speed = 10f;
    [SerializeField] float starting_health = 100f;
    [SerializeField] float max_health = 100f;
    [SerializeField] float damage = 25f;
    [Header("Attack Settings")]
    [SerializeField] Transform attack_point;
    [SerializeField] LayerMask player_layer_mask;
    [SerializeField] float attack_range = .5f;
    [SerializeField] float time_to_attack = 1f;

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
        if (!is_alive) return;

        bool is_attacking = animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");

        { // Process AI Behaviour
            Vector3 relative_position = target.position - transform.position;
            float distance_from_target = Util.distance(transform.position, target.position);

            if (is_provoked && distance_from_target <= nav_mesh_agent.stoppingDistance) // Makes the enemy attack
            {
                // Makes the enemy play Idle animation if it's not attacking
                animator.SetBool("isWalking", false);

                // Makes the enemy look at the player if it's not attacking
                if (!is_attacking)
                {
                    Vector3 look_position = new Vector3(relative_position.x, 0f, relative_position.z); // Allows the enemy to look only on X and Z axis.
                    Quaternion look_rotation = Quaternion.LookRotation(look_position);

                    transform.rotation = Quaternion.Slerp(transform.rotation, look_rotation, look_speed * Time.deltaTime);
                }

                if (can_attack)
                {
                    can_attack = false;
                    animator.SetTrigger("Attack");
                    time_elapsed_since_last_attack = 0f;
                }
            }
            else if (is_provoked && !is_attacking) // Makes the enemy chase the player
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

        { // Process Attack Cooldown
            if (!can_attack && !is_attacking)
            {
                time_elapsed_since_last_attack += Time.deltaTime;
                if (time_elapsed_since_last_attack >= time_to_attack) can_attack = true;
            }
        }
    }

    public void attack_player()
    {
        if (!is_alive) return;

        // Creates an overlap sphere to be the attack hit area
        Collider[] hit_colliders = new Collider[1];
        int colliders_found = Physics.OverlapSphereNonAlloc(attack_point.position, attack_range, hit_colliders, player_layer_mask);

        if (colliders_found > 0 && hit_colliders[0].CompareTag("Player")) GI.player.change_health_amount(-damage);
        else Debug.Log("Player not found");
    }

    public void take_damage(float value)
    {
        if (!is_alive) return;

        if (value < Mathf.Epsilon) value *= -1;
        Debug.Log("Damage received: " + value);

        current_health -= value;
        if (current_health < Mathf.Epsilon)
        {
            nav_mesh_agent.speed = 0f;
            loot_area.SetActive(true);
            GetComponent<CapsuleCollider>().isTrigger = true;
            is_alive = false;
        }
        else
        {
            // Tries to find the player. Start chasing him if it finds him
            RaycastHit hit;
            Vector3 relative_position = target.position - transform.position;
            bool has_hit_colliders = Physics.Raycast(transform.position, relative_position.normalized.normalized, out hit, Mathf.Infinity);
            Debug.DrawRay(transform.position, relative_position, Color.green);

            if (has_hit_colliders && hit.collider.CompareTag("Player")) is_provoked = true;
        }
    }

    void OnDrawGizmos()
    {
        // Draws Detection range.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detection_range);

        // Draws attack hit area
        Gizmos.DrawWireSphere(attack_point.position, attack_range);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(25, 25, 200, 20), "Enemy is attacking: " + animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));
    }
}
