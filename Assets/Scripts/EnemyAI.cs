using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum GuardState
{
    patrolling = 0,
    rotating = 1,
    finishedPatrolling = 2
};
public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private Animator guardAnim;

    //[SerializeField]
    //private GameObject caughtScreen;

    [SerializeField]
    private float _sightDistance,
                  _attackRange,
                  _walkpointRange,
                  walkSpeed,
                  runSpeed;

    [Range(0, 200)]
    [SerializeField]
    private float speed = 100;

    [SerializeField]
    private bool playerInsideRange,
                 playerInsideAttackRange;

    [SerializeField]
    private Transform player;

    [SerializeField]
    private LayerMask whatIsPlayer,
                     whatIsGround;

    public GuardState guardState = GuardState.patrolling;

    private bool walkpointSet;

    private Vector3 walkpoint;

    public static bool foundPlayer,
                       caught;



    [SerializeField]
    private Vector3[] checkpoints;

    [SerializeField]
    private float[] checkpointSpeed;


    [SerializeField]
    private float travelDuration = 1f,
                  travelDurationUp = 2f;

    [SerializeField]
    private float wait = 1f;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        guardAnim = GetComponent<Animator>();
        caught = false;
        //StartCoroutine(Checkpoint());
    }

    //private IEnumerator Checkpoint()
    //{
    //    // Loop 
    //    while (Application.isPlaying)
    //    {

    //        for (int i = 0; i < checkpoints.Length; i++)
    //        {
    //            //Travel from 1 to 2
    //            checkpointSpeed[i] = 0f;
    //            while (checkpointSpeed[i] < travelDuration)
    //            {
    //                transform.position = Vector3.Lerp(checkpoints[i], checkpoints[i + 1], checkpointSpeed[i] / travelDuration % checkpoints.Length);
    //                checkpointSpeed[i] += Time.deltaTime;
    //                yield return null;
    //            }

    //            // In case the counter isn't equal to the travelDuration
    //            transform.position = checkpoints[i + 1 % checkpoints.Length];

    //            // wait
    //            yield return new WaitForSeconds(wait);

    //            //Travel from 1 to 2
    //            checkpoints[i] = checkpoints[0];
    //            checkpointSpeed[i] = 0f;
    //            while (checkpointSpeed[i] < travelDuration)
    //            {
    //                transform.position = Vector3.Lerp(checkpoints[3], checkpoints[i], checkpointSpeed[i] / travelDuration);
    //                checkpointSpeed[i] += Time.deltaTime;
    //                yield return null;
    //            }

    //            // In case the counter isn't equal to the travelDuration
    //            transform.position = checkpoints[0];

    //            // wait
    //            yield return new WaitForSeconds(wait);
    //        }
    //    }
    //}
    public void FixedUpdate()
    {
        switch (guardState)
        {
            case GuardState.patrolling:
                playerInsideRange = Physics.CheckSphere(transform.position, _sightDistance, whatIsPlayer);
                playerInsideAttackRange = Physics.CheckSphere(transform.position, _attackRange, whatIsPlayer);
                if (!EnemyFov.isInFov) //Checkpoints();
                //if ()  Patroling();
                if (!playerInsideRange && !foundPlayer) EnemyFov.isInFov = false;
                if (EnemyFov.isInFov) ChasePlayer();
                if (agent.velocity.magnitude < 0.15f) walkpointSet = false;
                break;
            case GuardState.rotating:
                agent.isStopped = true;
                transform.Rotate(Vector3.up * 100 * Time.deltaTime, Space.Self);
                break;
            case GuardState.finishedPatrolling:
                transform.Rotate(Vector3.up * 20 * Time.deltaTime, Space.Self);
                walkpointSet = false;
                agent.isStopped = false;
                guardState = GuardState.patrolling;
                break;
            default:
                break;
        }
    }

    public void Checkpoints()
    {
        for (int i = 0; i < checkpoints.Length; i++)
        {
            //Travel from 1 to 2
            checkpointSpeed[i] = 0f;
            if (checkpointSpeed[i] < travelDuration)
            {
                Vector3 checkpointToCheckpoint = new Vector3(transform.position.x + 6, transform.position.y, transform.position.z + 6);
                agent.SetDestination(Vector3.Lerp(checkpoints[i], checkpoints[i + 1], checkpointSpeed[i] / travelDuration));
                checkpointSpeed[i] += Time.deltaTime;
            }
        }
    }

    public void Patroling()
    {
        agent.speed = walkSpeed;
        //guardAnim.Play("Walking");


        if (!walkpointSet) SearchWalkPoint();
        if (walkpointSet) agent.SetDestination(walkpoint);

        Vector3 distanceToWalkPoint = transform.position - walkpoint;

        // walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f) walkpointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomX = Random.Range(-_walkpointRange, _walkpointRange);
        float randomZ = Random.Range(-_walkpointRange, _walkpointRange);

        walkpoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);


        if (Physics.Raycast(walkpoint, -transform.up, 2f, whatIsGround)) walkpointSet = true;
    }

    public void ChasePlayer()
    {
        if (!foundPlayer)
        {
            //SoundManager.PlaySound("Shout");
        }
        foundPlayer = true;
        if (playerInsideRange) EnemyFov.isInFov = true;
        else { foundPlayer = false; EnemyFov.isInFov = false; }
        agent.speed = runSpeed;
        agent.SetDestination(player.position);
        // guardAnim.Play("Running");
        //SoundManager.audioSource.volume = 1;
        //if (SoundManager.audioSource.isPlaying == false && foundPlayer == true)
        //{
        //    SoundManager.PlaySound("ChasingMusic");
        //}
        //else if (SoundManager.audioSource.isPlaying == true && foundPlayer == false)
        //{
        //    SoundManager.audioSource.Stop();
        //}

        if (playerInsideAttackRange)
        {
            Attacking();
        }

    }

    public void Attacking()
    {
        //caught = true;
        //caughtScreen.SetActive(true);
    }
}

