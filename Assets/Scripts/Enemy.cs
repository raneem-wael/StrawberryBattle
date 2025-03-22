using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public int health = 50; // Adjust health as needed
    public EnemyManager manager;
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{enemyName} took {damage} damage! Remaining Health: {health}");

        if (health <= 0)
        {
            manager.RemoveEnemy(this);
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{enemyName} defeated!");
        Destroy(gameObject);
    }
}
