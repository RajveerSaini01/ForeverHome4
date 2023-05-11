using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Hitbox : MonoBehaviour
{
    public EnemyBehaviorSight behavior;
    [HideInInspector]
    SkinnedMeshRenderer skinnedmeshRenderer;
    [HideInInspector]
    public float health;
    public float blinkIntensity;
    public float blinkDuration;
    float blinkTimer;
    private void Start() {
        skinnedmeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (skinnedmeshRenderer == null)
        {
            skinnedmeshRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
        }
        skinnedmeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }
    public void OnRaycastHit(float damage, Vector3 direction){
        Debug.Log("Got Shot!");
        blinkTimer = blinkDuration;
        behavior.TakeDamage(damage, direction);
    }
    private void Update(){
        blinkTimer -= Time.deltaTime;
        float lerp = Mathf.Clamp01(blinkTimer/blinkDuration);
        float intensity = lerp * blinkIntensity;
        skinnedmeshRenderer.material.color = Color.red * intensity;
    }
}
