using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    [Header("Audios")]
    public AudioSource Damage;
    public AudioSource Dead;
    public GameObject Sangre;
    public GameObject Charquito;
    public int health = 3;
    public GameObject sprite;
    
    public event Action OnEnemyDied;

    private Animator animator;
    private bool isDead = false;

    private void Awake()
    {
        animator = GetComponent<Animator>(); 
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        Damage.Play();
        if (health <= 0)
        {
            Die();
        }
    }
    public void SangreActiva()
    {
        Sangre.SetActive(true);
    }
    public void SangreDesactivada()
    {
        Sangre.SetActive(false);
    }
   public void CharquitoActivado()
    {
        Charquito.SetActive(true);
    }
        void Die()
        {
            isDead = true;

           
            if (animator != null)
                animator.SetTrigger("Die");
            Dead.Play();
            
            EnemyPatrol patrol = GetComponent<EnemyPatrol>();
            if (patrol != null)
                patrol.enabled = false;

           
            CharacterController controller = GetComponent<CharacterController>();
            if (controller != null)
                controller.enabled = false;

            
            Collider col = GetComponent<Collider>();
            if (col != null)
                col.enabled = false;

            
            OnEnemyDied?.Invoke();

            
            Destroy(sprite,2f);
        Destroy(gameObject, 3f);
        }
    
}