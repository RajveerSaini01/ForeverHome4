using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public float damage;
    private Rigidbody rb;
    private Vector3 spawnPos;

    private void Awake()
    {
        spawnPos = Camera.main.transform.forward;
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //rb.AddForce(50f * Camera.main.transform.forward, ForceMode.Force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.collider.gameObject;

        if (!other.CompareTag("Bullet"))
        {
            rb.isKinematic = true;

            if (other.CompareTag("Enemy"))
            {
                Debug.Log("Collided: " + other.name);
                
                other.GetComponent<Hitbox>().OnRaycastHit(damage, spawnPos);
                
                //other.GetComponent<Hitbox>().OnRaycastHit(damage, -collision.GetContact(0).normal );
                //Debug.DrawRay(collision.GetContact(0).point, -collision.GetContact(0).normal, Color.red, 25f);
                ParticleSystem p = GetComponent<ParticleSystem>();
                p.Play();
                
            }
        }
        StartCoroutine(nameof(Explode));
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
