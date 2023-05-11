using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyBehaviorSight : MonoBehaviour
{
    public float wanderRadius = 5f;
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float wanderDuration = 5f; 
    public float maxpersistence = 3f;
    private float persistence = 0f;
    public float orbitDistance = 3f;
    public float damage = 15f;
    public float health = 100f;
    private float maxHealth = 0;
    public float panickNum = 20f;

    bool panicked = false;
    public Transform target;
    private Vector3 initialPosition;
    private Vector3 destination;
    private float wanderDelay;
    private float persistenceTimer = 0f;
    [HideInInspector]
    UIHealthBar healthBar;
    Ragdoll ragdoll;
    private NavMeshAgent agent;
    private EnemySight enemySight;
    private Vector3 lastKnownPosition;

    private float dieForce = 5f;

    Animator animator; 
    int randomNumber = 0;
    private float lastMeleeAttackTime = 0f;
    private float meleeAttackCooldown = 3f;
    // Ray's Additions
    [SerializeField]
    private string state;
    private float timeElapsed; // For use in updating Pursue() pathing in intervals
    
    private void Awake()
    {
        // Hookup internals
        ragdoll = GetComponent<Ragdoll>();
        animator = GetComponent<Animator>();
        healthBar = GetComponentInChildren<UIHealthBar>();
        maxHealth = health;
        initialPosition = transform.position;
        destination = initialPosition;
        agent = GetComponent<NavMeshAgent>();
        enemySight = GetComponent<EnemySight>();
        
        var rigidBodies = GetComponentsInChildren<Rigidbody>();
        foreach(var rigidBody in rigidBodies){
            Hitbox hitBox = rigidBody.gameObject.AddComponent<Hitbox>();
            hitBox.health = health;
        }
        
        // Wait for map baking completion
        Debug.Log("Subscribed to wait");
        LevelGeneration.OnReady += OnMapReady;
    }

    private void OnMapReady()
    {
        // Hookup Player / Targeting
        target = GameObject.FindGameObjectWithTag("Player").transform;
        Debug.Log("Map Ready");

        // Start State
        StartCoroutine(nameof(Wander));
    }

    private void Update()
    {
        /*  So the way this works now is:
            - coroutines change enemy states.
            - The stuff inside Update() increment the behavior of each state
            - Exit conditions are built into the coroutines and/or the update()
            
            To add a state, build a transition from a previous state, or an exclusive trigger in Update()
            e.g. Panic state will be the result of falling below a health threshold 
                  -  Create a trigger in Update() to check health % and current state
                    
                 Panic will stop any wander/pursue coroutines
                  -  Create a case statement in Update() to handle mechanics of panicking.
        */
        
        switch (state)
        {
            case "Wandering": // Pick a new wander location, then pick another one when we get there. (excepted by player entering vision)
                if (Vector3.Distance(transform.position, agent.destination) < 0.5f)
                {
                    agent.destination = GetNewWanderDestination();
                }
                break;
            
            case "Pursuing": // Move to the player's location every 1s or if the player moves too far from the destination. (excepted by player exiting vision)
                timeElapsed += Time.deltaTime;
                if ((Vector3.Distance(target.position, agent.destination) > 5f || timeElapsed >= 1f) && PlayerInsideVision())
                {
                    agent.destination = target.position;
                    agent.speed = runSpeed;
                    timeElapsed = 0f;
                }else if ((Vector3.Distance(target.position, agent.destination) < 1f || timeElapsed >= 1f) && PlayerInsideVision() && (Time.time - lastMeleeAttackTime > meleeAttackCooldown))
                {
                    agent.speed = 0;
                    MeleeAttack();
                    lastMeleeAttackTime = Time.time;
                }
                
                break;

            case "Searching": // Pick a new search location, then pick another one when we get there based on previous player destination. (excepted by player entering vision)
                if (Vector3.Distance(transform.position, agent.destination) < 0.5f)
                {
                    agent.destination = GetNewSearchDestination();
                }
                break;
        }
    }
    
    private IEnumerator Wander()
    {
        state = "Wandering";
        agent.speed = walkSpeed;
        animator.SetFloat("Speed", agent.velocity.magnitude);
        yield return new WaitUntil(PlayerInsideVision);

        StartCoroutine(nameof(Pursue));
    }

    private IEnumerator Pursue()
    {
        state = "Pursuing";
        agent.speed = runSpeed;
        animator.SetFloat("Speed", agent.velocity.magnitude);
        yield return new WaitUntil(PlayerOutsideVision); 
        lastKnownPosition = target.position;
        StartCoroutine(nameof(Search));
    }
       private IEnumerator Search()
    {
        state = "Searching";
        agent.speed = walkSpeed;
        animator.SetFloat("Speed", agent.velocity.magnitude);
        yield return new WaitUntil(PlayerInsideVision); 
        
        StartCoroutine(nameof(Pursue));
    } 
    private bool PlayerInsideVision()
    {
        return enemySight.canSeePlayer;
    }
    private bool PlayerOutsideVision()
    {
        return !enemySight.canSeePlayer;
    }
    

    IEnumerator WaitForWanderDuration()
    {
        yield return new WaitForSeconds(wanderDuration);
    }

    IEnumerator SearchDelay()
    {
        Debug.Log("Waiting");
        yield return new WaitForSeconds(3);
    }
    

    void MeleeAttack()
    {
        animator.SetBool("Attack", true);
        var playerDamage = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDamage>();
        if (Vector3.Distance(target.position, agent.destination) < 1f)
        {
            playerDamage.TakeDamage(damage);
        }
        StartCoroutine(ResetAttack());
    }

    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        agent.speed = walkSpeed;
        animator.SetBool("Attack", false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            target = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            target = null;
            persistenceTimer = 0f;
        }
    }

    Vector3 GetNewWanderDestination()
    {
        Vector3 newDestination = Random.insideUnitSphere * (wanderRadius/2);
        newDestination += initialPosition;
        NavMeshHit navMeshHit;
        NavMesh.SamplePosition(newDestination, out navMeshHit, wanderRadius, NavMesh.AllAreas);
        return navMeshHit.position;
    }

    Vector3 GetNewSearchDestination()
    {
        Vector3 direction = lastKnownPosition - transform.position;
        float distance = Random.Range(3, 8);
        Vector3 randomPoint = lastKnownPosition + direction.normalized * distance;
        NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, distance, NavMesh.AllAreas);
        return hit.position;
    }

    private bool isPanicking = false;
    private float panicTimer = 0f;
    private float panicDuration = 10f;
    private float panicDirectionChangeInterval = 2f;
    private float panicStopDuration = 5f;
    private float panicHealthRegenRate = 2.5f;
    private Vector3 panicDestination;
    private float originalSpeed;

    void Panick()
    {
        // Set the panic destination as the opposite direction from the player's position
        panicDestination = transform.position - (target.position - transform.position);

        // Start the panic behavior
        isPanicking = true;
        originalSpeed = agent.speed;
        agent.speed = runSpeed;

        StartCoroutine(PanicRoutine());
    }

    IEnumerator PanicRoutine()
    {
        while (isPanicking)
        {
            // Check if the player is too close or health has decreased
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);
            if (distanceToPlayer <= 10f || health < maxHealth)
            {
                // Stop panicking and return to normal behavior
                StopPanic();
                yield break;
            }

            // Flee from the player's position
            agent.destination = panicDestination;

            // Randomly change the panic direction every panicDirectionChangeInterval seconds
            if (panicTimer % panicDirectionChangeInterval == 0f)
            {
                Vector3 randomDirection = Random.insideUnitSphere * 5f;
                randomDirection.y = 0f;
                panicDestination = transform.position - randomDirection;
            }

            // Increase health by panicHealthRegenRate every second
            health += panicHealthRegenRate * Time.deltaTime;

            // Check if the panic duration has ended
            if (panicTimer >= panicDuration)
            {
                // Stop panicking and hold still for panicStopDuration seconds
                agent.speed = 0f;
                yield return new WaitForSeconds(panicStopDuration);
                StopPanic();
                yield break;
            }

            panicTimer += Time.deltaTime;
            yield return null;
        }
    }

    void StopPanic()
    {
        isPanicking = false;
        agent.speed = originalSpeed;
        panicTimer = 0f;
    }

    public void TakeDamage(float damage, Vector3 direction)
    {
        healthBar.SetHealthBarPercentage(health/maxHealth);
        health -= damage;
        if (health <= 0.0f)
        {
            Die(direction);
        }
    }
    void Die(Vector3 direction)
    {
        ragdoll.ActivateRagdoll();
        direction.y = 1;
        ragdoll.ApplyForce(direction * dieForce);
        healthBar.gameObject.SetActive(false);
        Destroy(gameObject, 12f);
    }
}