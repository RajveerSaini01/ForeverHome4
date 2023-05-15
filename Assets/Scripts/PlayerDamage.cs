using System.Collections;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.SceneManagement;

public class PlayerDamage : MonoBehaviour
{
    public float detectionRadius = 1f;
    public float detectionTime = 1f;
    public float cooldownTime = 3f;
    public float health = 100f;
    public Color damageColor = Color.red;

    private float lastDetectionTime = -Mathf.Infinity;
    private bool isOnCooldown = false;

    private void Update()
    {
        Debug.Log("Health: " + health);
        if (isOnCooldown)
        {
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                if (Time.time - lastDetectionTime >= detectionTime)
                {
                    health -= 25f;
                    StartCoroutine(DamageFlash());
                    lastDetectionTime = Time.time;
                    isOnCooldown = true;
                    Invoke(nameof(EndCooldown), cooldownTime);
                }
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health = Mathf.Clamp(health-damage, 0f, health);
        gameObject.GetComponentInChildren<UI>().UpdateHealth(health);
        StartCoroutine(nameof(DamageFlash)) ;
        if (health is 0 or < 0)
        {
            health = 0f;
            Debug.Log("PlayerDamage death");
            SceneManager.LoadScene("DeathScreen");

        }
    }

    private void EndCooldown()
    {
        isOnCooldown = false;
    }

    private IEnumerator DamageFlash()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.material.color;
            renderer.material.color = damageColor;
            yield return new WaitForSeconds(0.2f);
            renderer.material.color = originalColor;
        }
    }
}