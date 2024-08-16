using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class SurroundPlayer : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private CapsuleCollider agentCollider;
    [SerializeField] private float baseDistanceFromPlayer = 2f;
    [SerializeField] private int agentIndex; // Unique index for each agent
    [SerializeField] private int totalAgents; // Total number of agents encircling the player

    private void OnValidate()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (agentCollider == null)
            agentCollider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        EncirclePlayer();
    }
    
    private void Update()
    {
        // Check if the agent has reached its destination
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            RotateTowardsPlayer();
        }
    }

    private void EncirclePlayer()
    {
        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

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

        Vector3 targetPosition = new Vector3(playerPosition.x + xOffset, playerPosition.y, playerPosition.z + zOffset);

        // Set the agent's destination directly to the calculated target position
        agent.SetDestination(targetPosition);
    }
    
    private void RotateTowardsPlayer()
    {
        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 directionToPlayer = (playerPosition - transform.position).normalized;

        // Calculate the rotation required to face the player
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));

        // Apply the rotation to the agent
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
}
