using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    float time_elapsed_since_last_attack;
    public bool is_alive = true, is_provoked, can_attack = true;

    [HideInInspector] public Transform target;
    [HideInInspector] public NavMeshAgent nav_mesh_agent;
    [HideInInspector] public Animator animator;

    public float look_speed = 10f;
    public float health = 100f;
    public float damage = 25f;
    public float terror_damage = 15f;
    public float terror_reduced_when_killed = -10f;

    [Header("Detection Settings")]
    public float detection_range = 4f;
    public Transform detection_point;

    [Header("Attack Settings")]
    public Transform attack_point;
    public LayerMask player_layer_mask;
    public float attack_range = .5f;
    public float time_to_attack = 1f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        nav_mesh_agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        target = GI.player.transform;
    }

    void Update()
    {
        if (!is_alive) return;
        if (GI.pause_game.game_paused) return;

        bool is_attacking = animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"); // Gets the name of the current playing animation

        { // AI Behaviour
            Vector3 relative_position = target.position - transform.position;
            float distance_from_target = Util.distance(transform.position, target.position);
            float target_distance_from_detection_point = Vector3.Distance(detection_point.position, target.position);

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

                // Attack
                if (can_attack && GI.enemy_manager.can_attack())
                {
                    can_attack = false;
                    animator.SetTrigger("Attack");
                    time_elapsed_since_last_attack = 0f;
                }
            }
            else if (is_provoked && !is_attacking) // Makes the enemy chase the player
            {
                // TODO Only makes the enemy look at the player if the raycast takes the player
                Vector3 look_position = new Vector3(relative_position.x, 0f, relative_position.z); // Allows the enemy to look only on X and Z axis.
                Quaternion look_rotation = Quaternion.LookRotation(look_position);

                transform.rotation = Quaternion.Slerp(transform.rotation, look_rotation, look_speed * Time.deltaTime);
                nav_mesh_agent.SetDestination(target.position);

                animator.SetBool("isWalking", true);
            }
            else if (target_distance_from_detection_point <= detection_range) // Checks if the target is close enough
            {
                // Checks if there is an wall between player and enemy
                // TODO Launches the raycast from the enemy's head instead of the middle of he's body, so the raycast don't collides with small props
                RaycastHit hit;
                bool has_hit_colliders = Physics.Raycast(transform.position, relative_position.normalized, out hit, distance_from_target);
                Debug.DrawRay(transform.position, relative_position, Color.green);

                if (has_hit_colliders && hit.collider.CompareTag("Player")) is_provoked = true;
            }
        }

        { // Attack Cooldown
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

        if (colliders_found > 0 && hit_colliders[0].CompareTag("Player"))
        {
            GI.player.change_health_amount(-damage);
            GI.player.change_terror_amount(terror_damage);
        }
        else Debug.Log("Player not found");
    }

    public void take_damage(float value)
    {
        if (!is_alive) return;

        Debug.Log("Damage received: " + value);

        health -= value;
        if (health < Mathf.Epsilon)
        {
            nav_mesh_agent.speed = 0f;
            GetComponent<CapsuleCollider>().isTrigger = true;
            GI.player.change_terror_amount(terror_reduced_when_killed);
            is_alive = false;
            gameObject.SetActive(false); // Using in Sandbox
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
        Gizmos.DrawWireSphere(detection_point.position, detection_range);

        // Draws attack hit area
        Gizmos.DrawWireSphere(attack_point.position, attack_range);
    }

    //private void OnGUI()
    //{
    //    GUI.Label(new Rect(25, 25, 200, 20), "Enemy is attacking: " + animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));
    //}
}
