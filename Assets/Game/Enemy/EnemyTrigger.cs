using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    public EnemyAI[] enemies_to_provoke;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (EnemyAI enemy in enemies_to_provoke)
            {

                if (enemy.is_alive)
                {
                    if (!enemy.gameObject.activeSelf) enemy.gameObject.SetActive(true);
                    enemy.is_provoked = true;
                }
            }
        }
    }
}
