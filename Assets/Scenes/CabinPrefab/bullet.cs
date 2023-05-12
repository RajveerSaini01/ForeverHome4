using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public float damage;
    private Rigidbody rb;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        //rb.AddForce(50f * Camera.main.transform.forward, ForceMode.Force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided: " + collision.collider.gameObject.name);
        rb.isKinematic = true;

        if (collision.collider.gameObject.CompareTag("Enemy"))
        {
            collision.collider.gameObject.GetComponent<Hitbox>().OnRaycastHit(damage, -collision.contacts[0].normal);
        }
        
        Destroy(gameObject);
    }
}
