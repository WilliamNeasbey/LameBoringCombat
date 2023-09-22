using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private Animator anim;
    public float health = 50f;
    //AudioSource audioData; 
    public AudioSource audioSource;
    public AudioClip clip;
    public float volume = 1;
    public AudioClip audioClip;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void GetHit()
    {
        anim.SetTrigger("Hit");
        health -= 10;
        if (health <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        //gameObject.SetActive(false);
        Destroy(gameObject);
        AudioSource.PlayClipAtPoint(audioClip, transform.position, volume);
      
    }
}
