using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class CharacterSkills : MonoBehaviour
{
    public EnemyManager enemyManager;
    //public Animator animator;
    public ParticleSystem shieldEffect;
    public float shieldDuration = 3f;
    public ParticleSystem fireballEffect; // Fireball attack effect
    private bool isShieldActive = false;
    private bool isShooting = false;
    private float fireRate = 1f; // Adjust attack speed

    public void Start()
    {
        fireballEffect.Stop();
        shieldEffect.Stop();
    }
    public void UseSkillOne()
    {
        if (!isShooting) // Start shooting only if not already shooting
        {
            isShooting = true;
            StartCoroutine(FireballRoutine());
        }
    }

    private IEnumerator FireballRoutine()
    {
        while (isShooting) // Keep shooting while active
        {
            Enemy target = enemyManager.GetCurrentTarget();
            if (target != null)
            {
                Debug.Log("Skill One Activated: Fireball!");
                target.TakeDamage(10);
                if (fireballEffect != null) { 
                fireballEffect.Play();
                    Debug.Log("fireballEffect is playing");
                }
            }
            yield return new WaitForSeconds(fireRate); // Delay between attacks
        }
    }

    public void StopShooting() // Call this to stop attacking
    {
        isShooting = false;
    }



    public void UseSkillTwo()
    {
        if (!isShieldActive)
        {
            Debug.Log("Skill Two Activated: Shield!");
            
            StopShooting();
            isShieldActive = true;
            shieldEffect.Play();

            StartCoroutine(DisableShieldAfterTime());
        }
    }

    private IEnumerator DisableShieldAfterTime()
    {
        yield return new WaitForSeconds(shieldDuration);
        isShieldActive = false;
        shieldEffect.Stop();
        Debug.Log("Shield expired.");
    }

    public bool IsShieldActive()
    {
        return isShieldActive;
    }
}

    //public void UseSkillOne()
    //{
    //    Enemy target = enemyManager.GetCurrentTarget();
    //    if (target != null)
    //    {
    //        Debug.Log("Skill One Activated: Fireball!");
    //  //      animator.SetTrigger("Attack");

    //        target.TakeDamage(20); // Adjust damage value as needed
    //    }
    //}