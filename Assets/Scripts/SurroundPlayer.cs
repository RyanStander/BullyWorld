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
    [SerializeField] private CapsuleCollider agentCollider;
    private Transform player;
    [SerializeField] private float baseDistanceFromPlayer = 2f;
    [SerializeField] private int agentIndex; // Unique index for each agent
    [SerializeField] private int totalAgents; // Total number of agents encircling the player
    [SerializeField] private int totalAnims = 8;
    [SerializeField] private Animator anim;
    
    private bool startedRotating = false;
    private string[] npcDialogueLines;
    
    private void OnValidate()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

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
    }

    private void Update()
    {
        // Check if the agent has reached its destination
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !startedRotating)
        {
            startedRotating = true;
            //start couritine to rotate towards player
            StartCoroutine(RotateTowardsPlayer());
        }
    }

    private void EncirclePlayer()
    {
        // Calculate the minimum required distance based on the number of agents and their size
        float colliderRadius = agentCollider.radius;
        float angleStep = 360f / totalAgents;
        float angleInRadians = Mathf.Deg2Rad * angleStep;

        // Ensure that the radius accounts for the size of the agents
        float requiredRadius = (colliderRadius * 5) / (2 * Mathf.Sin(angleInRadians / 2));
        float distanceFromPlayer = Mathf.Max(baseDistanceFromPlayer, requiredRadius);

        // Calculate the angle for this agent
        float agentAngle = agentIndex * angleStep;

        // Convert angle to radians
        float agentAngleInRadians = agentAngle * Mathf.Deg2Rad;

        // Calculate the target position based on the angle
        float xOffset = Mathf.Cos(agentAngleInRadians) * distanceFromPlayer;
        float zOffset = Mathf.Sin(agentAngleInRadians) * distanceFromPlayer;

        Vector3 targetPosition =
            new Vector3(player.position.x + xOffset, player.position.y, player.position.z + zOffset);

        // Set the agent's destination directly to the calculated target position
        agent.SetDestination(targetPosition);
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
    
    public void SetupAgent(string[] npcLines,int index, int total)
    {
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
