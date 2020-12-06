using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = System.Random;

public class EnemyController : MonoBehaviour
{
    public delegate void OnEnemyAction();
    public static OnEnemyAction OnEnemyDeath;

    public enum States
    {
        Idle,
        Follow,
        Hit,
        Stunned,
        Dead,
        Wander,
        allStates
    }

    public bool isGuard;
    public States currentState;
    public States lastState;
    private Animator animator;

    public GameObject bloodSplat;
    public Transform bloodSplatPosition;

    Transform target;
    public NavMeshAgent agent;

    public float timer;
    public float deathTime;

    public float sanityChangeValue;
    public float damage;
    public float distance;
    public GameObject targetRig;

    private bool doOnce;
    private bool doOnce2;
    private Player player;
    public GameObject parent;

    public float stunMaxTime;
    public float stunTimer;

    public RagdollHelper ragdoll;
    public SphereCollider detectionCollider;
    public SphereCollider instantKOCol;
    public Rigidbody instantKORB;
    public EnemySight sight;
    public float enemiesAlertRadius;
    public LayerMask alertLayer;

    [Header("UI"), Space]
    public GameObject stunIcon;
    public Image fillBar;
    private float fillTimer;
    [Header("Surprise Settings")]
    public bool isSurpriseEnemy;
    public GameObject activator;

    [Header("Wander Settings")]
    public Transform[] waypoints;
    public Transform currentWaypoint;
    public float idleMaxTime;
    public float idleTimer;

    public int rndIndex;
    public int previousIndex;

    public bool canAlarm;
    public Vector3 originalKOPosition;
    public Vector3 originalKOScale;
    public Quaternion originalKORotation;
    // Start is called before the first frame update
    void Start()
    {
        SetRigidbodyState(true);
        SetColliderState(true);
        
        GetComponent<Animator>().enabled = true;

        target = GameManager.Get().playerGO.transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameManager.Get().playerGO.GetComponent<Player>();

        if (isSurpriseEnemy)
        {
            Invoke("Stun", 1.0f);
        }

        if (!isGuard)
        {
            stunIcon.SetActive(false);
            originalKOPosition = instantKOCol.gameObject.transform.localPosition;
            originalKOScale = instantKOCol.gameObject.transform.localScale;
            originalKORotation = instantKOCol.gameObject.transform.localRotation;
        }
        

    }

    void Update()
    {
        distance = Vector3.Distance(transform.position, target.position);

        //test ragdoll
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    ragdoll.ragdolled = true;
        //}

        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    ragdoll.ragdolled = false;
        //}

        switch (currentState)
        {
            case States.Idle:
                if (waypoints.Length>0)
                {
                    idleTimer += Time.deltaTime;
                    if (idleTimer > idleMaxTime)
                    {
                        ChangeState(States.Wander);
                        idleTimer = 0;
                    }
                }
                break;
            case States.Follow:
                MovementUpdate();
                break;
            case States.Hit:
                break;
            case States.Stunned:
                if (!isSurpriseEnemy)
                {
                    stunTimer += Time.deltaTime;
                    
                    if (fillBar)
                    {
                        fillTimer -= Time.deltaTime;
                        fillBar.fillAmount = fillTimer / stunMaxTime;
                    }

                    if (stunTimer > stunMaxTime)
                    {
                        ragdoll.ragdolled = false;
                        stunTimer = 0;
                        GetComponent<BoxCollider>().enabled = true;
                        stunIcon.SetActive(false);
                        doOnce = false;
                        //ChangeState(States.Idle);
                    }
                }
                else
                {
                    if (!activator || activator.GetComponent<Door>() && !activator.GetComponent<Door>().isOpen)
                    {
                        ragdoll.ragdolled = false;
                        isSurpriseEnemy = false;
                        //transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                        stunTimer = 0;
                        GetComponent<BoxCollider>().enabled = true;
                    }
                }
                break;
            case States.Wander:
                if (agent.remainingDistance< agent.stoppingDistance)
                {
                    ChangeState(States.Idle);
                }
                break;
            case States.Dead:
                break;
            default:
                break;
        }

        if (distance <= agent.stoppingDistance+0.5f && (currentState!=States.Dead && currentState != States.Stunned) && (sight.playerInSight || sight.playerHeard))
        {
            Attack();
            FaceTarget();
            doOnce = true;
        }
        else if (currentState != States.Dead && currentState != States.Stunned && lastState!=States.Stunned && currentState != States.Wander && currentState!=States.Idle)
        {
            if (doOnce)
            {
                doOnce = false;
                
                ChangeState(States.Follow);
            }
        }
    }

    private void Attack()
    {
        if (doOnce2)
        {
            ChangeState(States.Hit);
            agent.speed = 0;
            agent.acceleration = 35;
            agent.isStopped = true;
            agent.ResetPath();
            doOnce2 = false;
        }
    }

    public void ResumeNavMeshAgentSpeed()
    {
        if (isGuard)
        {
            agent.speed = 4;
            agent.acceleration = 35;
        }
        else
        {
            agent.speed = 5;
            agent.acceleration = 10;
        }
        agent.isStopped = false;
    }

    public void StopNavMeshAgent()
    {
        agent.speed = 0;
        agent.acceleration = 0;
        agent.isStopped = true;
    }

    public void DoDamage()
    {
        //RaycastHit hit;

        //if (Physics.Raycast(transform.position + (transform.up*1.5f), transform.forward, out hit, 18))
        //{
        //    if (hit.transform.tag=="Player")
        //    {

        //    }
        //}
        //Debug.DrawRay(transform.position + (transform.up * 1.5f), transform.forward, Color.cyan,Time.deltaTime);
        AkSoundEngine.PostEvent("Attack_E", this.gameObject);
        if (distance <= agent.stoppingDistance+0.5f)
        {
            player.ReceiveDamage(damage);
        }
    }

    public void PlayFootStep()
    {
        AkSoundEngine.PostEvent("Steps_E", this.gameObject);
    }

    public void PlayIdle()
    {
        AkSoundEngine.PostEvent("Enemy_Idle", this.gameObject);
    }

    public void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 2f);
    }

    private void DeathUpdate()
    {
        if (timer > deathTime)
        {
            //gameObject.SetActive(false);
            Destroy(parent.gameObject);
        }
    }

    private void MovementUpdate()
    {
        FaceTarget();
        agent.SetDestination(target.position);
        targetRig.transform.position = target.transform.position;
    }

    public void ChangeState(States newState)
    {
        lastState = currentState;
        currentState = newState;

        if (currentState!=lastState)
        {
            doOnce2 = true;
        }
        //manage animation
        switch (currentState)
        {
            case States.Idle:
                animator.SetBool("Idle", true);
                animator.SetBool("Follow", false);
                animator.SetBool("Hit", false);
                animator.SetBool("Wander", false);
                idleTimer = 0;
                agent.isStopped = true;
                break;
            case States.Follow:
                ResumeNavMeshAgentSpeed();
                animator.SetBool("Idle", false);
                animator.SetBool("Follow", true);
                animator.SetBool("Hit", false);
                animator.SetBool("Wander", false);
                agent.isStopped = false;
                agent.SetDestination(target.position);
                break;
            case States.Hit:
                animator.SetBool("Idle", false);
                animator.SetBool("Follow", false);
                animator.SetBool("Hit", true);
                animator.SetBool("Wander", false);
                agent.isStopped = true;
                break;
            case States.Stunned:
                animator.SetBool("Idle", false);
                animator.SetBool("Follow", false);
                animator.SetBool("Hit", false);
                animator.SetBool("Wander", false);
                agent.isStopped = true;
                break;
            case States.Wander:
                animator.SetBool("Idle", false);
                animator.SetBool("Follow", false);
                animator.SetBool("Hit", false);
                animator.SetBool("Wander", true);

                if (waypoints.Length>0)
                {
                    agent.isStopped = false;
                    agent.speed = 2;
                    agent.acceleration = 5;
                    previousIndex = rndIndex;
                    rndIndex = UnityEngine.Random.Range(0, waypoints.Length);
                    while (previousIndex==rndIndex)
                    {
                        rndIndex = UnityEngine.Random.Range(0, waypoints.Length);
                    }
                    agent.SetDestination(waypoints[rndIndex].position);
                    currentWaypoint = waypoints[rndIndex].transform;
                }
                break;
            case States.Dead:
                break;
            default:
                break;
        }
    }

    void SetRigidbodyState(bool state)
    {

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = state;
        }

        //GetComponent<Rigidbody>().isKinematic = !state;
        if (!isGuard)
        {
            instantKORB.isKinematic = true;
        }
        
        //instantKOCol.gameObject.SetActive(false);
    }


    void SetColliderState(bool state)
    {

        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = state;
        }

        //GetComponent<Collider>().enabled = !state;

    }

    public void Die()
    {
        Debug.Log(currentState.ToString());

        AkSoundEngine.PostEvent("Death_E", this.gameObject);

        SetRigidbodyState(false);
        if(stunIcon)
        {
            stunIcon.SetActive(false);
        }
        
        SetColliderState(true);
        agent.isStopped = true;
        if (!isGuard)
        {
            GetComponent<BoxCollider>().enabled = false;
        }
        GetComponent<Animator>().enabled = false;

        ChangeState(States.Dead);

        Invoke("CreateBlood", 3.0f);

        AlertEnemies();

        if (gameObject != null)
        {
            Destroy(parent.gameObject, deathTime);
        }

        if (OnEnemyDeath != null)
        {
            OnEnemyDeath();
        }
    }

    public void AlertEnemies()
    {
        if (canAlarm)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemiesAlertRadius, alertLayer);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.GetComponent<EnemyController>() != null)
                    hitCollider.GetComponent<EnemyController>().ChangeState(States.Follow);
            }
        }
    }

    public void Stun()
    {
        if(currentState != States.Stunned && currentState != States.Dead)
        {
            Debug.Log(currentState.ToString());

            //if (!isSurpriseEnemy)
            //{
            //    //AkSoundEngine.PostEvent("Hit_E_Wrench", this.gameObject);
            //}

            SetRigidbodyState(false);
            SetColliderState(true);
            agent.isStopped = true;
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<Animator>().enabled = false;
            ragdoll.ragdolled = true;

            ChangeState(States.Stunned);
            if (!isSurpriseEnemy)
            {
                stunIcon.SetActive(true);
                fillTimer = stunMaxTime;
            }

            instantKORB.isKinematic = true;
            
        }
    }

    public void WakeUp()
    {
        stunTimer = 0;
        SetRigidbodyState(true);
        agent.isStopped = false;
        GetComponent<BoxCollider>().enabled = true;

        ragdoll.ragdolled = false;

        
        detectionCollider.enabled = true;
        instantKOCol.enabled = true;

        if (sight.playerInSight || sight.playerHeard)
        {
            ChangeState(States.Follow);
        }
        else
        {
            ChangeState(States.Idle);
        }

    }

    private void CreateBlood()
    {
        if (bloodSplat)
        {
            GameObject newBlood = Instantiate(bloodSplat, bloodSplatPosition.position, bloodSplat.transform.rotation);
            newBlood.transform.position = newBlood.transform.position + newBlood.transform.forward * -1.3f;
        }
    }

}