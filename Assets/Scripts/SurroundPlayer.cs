using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SurroundPlayer : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private NavMeshObstacle obstacle;
    [SerializeField] private CapsuleCollider agentCollider;
    private Transform player;
    [SerializeField] private int agentIndex; // Unique index for each agent
    [SerializeField] private int totalAgents; // Total number of agents encircling the player
    [SerializeField] private float extraSpacing = 0.5f; // Extra spacing between agents
    [SerializeField] private int totalAnims = 8;
    [SerializeField] private Animator anim;
    [SerializeField] private float collisionAvoidanceRadius = 0.3f; 
    [SerializeField] private float checkInterval = 0.5f; // Time interval between checks
    
    private float nextCheckTime = 0f;
    
    private bool startedRotating = false;
    private float baseDistanceFromPlayer = 2f;
    private string[] npcDialogueLines;
    private Vector3 targetPosition;
    
    private void OnValidate()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
        
        if (obstacle == null)
            obstacle = GetComponent<NavMeshObstacle>();

        if (agentCollider == null)
            agentCollider = GetComponent<CapsuleCollider>();
        
        if (anim == null)
            anim = GetComponent<Animator>();
    }

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
        
        EncirclePlayer();
        nextCheckTime = Time.time + (checkInterval * agentIndex / totalAgents);
    }

    private void Update()
    {
        // Check if the agent has reached its destination
        if (agent.enabled && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !startedRotating)
        {
            agent.enabled = false;
            obstacle.enabled = true;
            startedRotating = true;
            //start couritine to rotate towards player
            StartCoroutine(RotateTowardsPlayer());
        }

        if (Time.time >= nextCheckTime)
        {
            AvoidCollisions();
            nextCheckTime = Time.time + checkInterval;
        }
    }

    private void EncirclePlayer()
    {
        float colliderDiameter = agentCollider.radius * 2 + extraSpacing;
        float currentRadius = baseDistanceFromPlayer;
        int agentsPlaced = 0;
        int agentsInCurrentCircle = 0;

        while (agentsPlaced + agentsInCurrentCircle < agentIndex + 1)
        {
            float circumference = 2 * Mathf.PI * currentRadius;
            agentsInCurrentCircle = Mathf.FloorToInt(circumference / colliderDiameter);

            if (agentsPlaced + agentsInCurrentCircle > agentIndex)
            {
                break;
            }

            agentsPlaced += agentsInCurrentCircle;
            currentRadius += colliderDiameter; // Increase radius for the next circle
        }

        int positionInCircle = agentIndex - agentsPlaced;
        float angleStep = 360f / agentsInCurrentCircle;
        float agentAngle = positionInCircle * angleStep;
        float agentAngleInRadians = agentAngle * Mathf.Deg2Rad;

        float xOffset = Mathf.Cos(agentAngleInRadians) * currentRadius;
        float zOffset = Mathf.Sin(agentAngleInRadians) * currentRadius;

        targetPosition = new Vector3(player.position.x + xOffset, player.position.y, player.position.z + zOffset);

        agent.SetDestination(targetPosition);
    }
    
    private void AvoidCollisions()
    {
        foreach (var otherAgent in FindObjectsOfType<NavMeshAgent>())
        {
            if (otherAgent == agent) continue;

            if (Vector3.Distance(otherAgent.transform.position, targetPosition) < collisionAvoidanceRadius)
            {
                // Adjust target position slightly to avoid collision
                Vector3 direction = (targetPosition - player.position).normalized;
                targetPosition += direction * extraSpacing; // Move slightly away
                agent.enabled = true;
                obstacle.enabled = false;
                startedRotating = false;
                agent.SetDestination(targetPosition);
            }
        }
    }

    private IEnumerator RotateTowardsPlayer()
    {
        float elapsedTime = 0f;

        while (elapsedTime < 3f)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            // Calculate the rotation required to face the player
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));

            // Apply the rotation to the agent
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            elapsedTime += Time.deltaTime;
            
            yield return null;
        }
    }
    
    public void SetupAgent(string[] npcLines,int index, int total, float baseDistance)
    {
        baseDistanceFromPlayer = baseDistance;
        agentIndex = index;
        totalAgents = total;
        npcDialogueLines = npcLines;
    }

    public bool IsStillWalking()
    {
        return agent.velocity.magnitude > 0.1f;
    }
    
    public string GetRandomDialogueLine()
    {
        anim.SetInteger("Action",+Random.Range(0, totalAnims));
        anim.SetTrigger("PlayAction");
        
        return npcDialogueLines[UnityEngine.Random.Range(0, npcDialogueLines.Length)];
    }
}
