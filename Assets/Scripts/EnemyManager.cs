using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyManager : MonoBehaviour
{
    private Enemy currentTarget;
    public Transform player;
    public NavMeshAgent playerAgent;
    public CharacterSkills characterSkills;
    public float damageInterval = 1f; // How often player takes damage
    public int playerHealth = 100;
    public int enemyDamage = 5; // Damage per second when idle near enemy
    public float proximityThreshold = 2f; // Distance to enemy to start damage
    private bool takingDamage = false;
    public List<Enemy> enemies = new List<Enemy>(); // Tracks active enemies

    private void Start()
    {
        // Ensure playerAgent is assigned
        if (playerAgent == null)
        {
            playerAgent = player.GetComponent<NavMeshAgent>();
        }
        enemies = new List<Enemy>(FindObjectsOfType<Enemy>());

        //enemies = FindObjectOfType<Enemy>().ToList();
    }


    //public void SetTarget(string enemyName)
    //{
    //    Enemy[] enemies = FindObjectsOfType<Enemy>();

    //    foreach (Enemy enemy in enemies)
    //    {
    //        if (enemy.enemyName.ToLower() == enemyName.ToLower())
    //        {
    //            currentTarget = enemy;
    //            Debug.Log($"Target changed to {enemyName}");

    //            // Move the player using its own movement script
    //            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
    //            if (playerMovement != null)
    //            {
    //                playerMovement.MoveTo(currentTarget.transform.position);

    //            }
    //            else
    //            {
    //                Debug.LogError("PlayerMovement script is missing on the player!");
    //            }
    //            return;
    //        }
    //    }

    //    Debug.LogError($"Enemy {enemyName} not found!");
    //}

    public void SetTarget(string enemyName)
    {
        // Filter enemies that match the given name and are still active
        Enemy target = enemies.FirstOrDefault(e => e != null && e.enemyName.ToLower() == enemyName.ToLower());

        if (target != null)
        {
            currentTarget = target;
            Debug.Log($"Target changed to {enemyName}");

            // Move the player using its own movement script
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.MoveTo(currentTarget.transform.position);
            }
            else
            {
                Debug.LogError("PlayerMovement script is missing on the player!");
            }
        }
        else
        {
            Debug.LogError($"No {enemyName} enemies left!");
        }
    }


    private void Update()
    {
        if (currentTarget != null)
        {
            float distance = Vector3.Distance(player.position, currentTarget.transform.position);

            // Stop moving when close enough
            if (!playerAgent.pathPending && playerAgent.remainingDistance <= playerAgent.stoppingDistance)
            {
                playerAgent.ResetPath();
            }

            // Start damaging the player if close and no skill is active
            if (distance < proximityThreshold && !takingDamage && !characterSkills.IsShieldActive())
            {
                StartCoroutine(TakeDamageOverTime());
            }
        }
    }

    private IEnumerator TakeDamageOverTime()
    {
        takingDamage = true;

        while (takingDamage && playerHealth > 0)
        {
            playerHealth -= enemyDamage;
            Debug.Log($"Player took damage! Remaining Health: {playerHealth}");
            Debug.Log($"Player Transform:" + playerAgent.transform.position);
            Debug.Log($"Player Transform:" + player.position);

            if (playerHealth <= 0)
            {
                //Debug.Log("Game Over!");
                //UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
                //yield break;
                Debug.Log("Dead");
            }

            yield return new WaitForSeconds(damageInterval);
        }

        takingDamage = false;
    }

    public Enemy GetCurrentTarget()
    {
        return currentTarget;
    }

    public bool HasRemainingEnemy(string enemyType)
    {
        return enemies.Any(e => e != null && e.enemyName.ToLower() == enemyType.ToLower());
    }

    //: Removes an enemy when it dies**
    public void RemoveEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }
}
