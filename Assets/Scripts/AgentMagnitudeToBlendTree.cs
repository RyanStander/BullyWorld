using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentMagnitudeToBlendTree : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float blendSpeed = 5f;

    private void OnValidate()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        float agentMagnitude = agent.velocity.magnitude;
        float normalizedMagnitude = agentMagnitude / agent.speed;

        // Smoothly blend between the idle and walk animations based on the agent's velocity
        animator.SetFloat("Speed", normalizedMagnitude, blendSpeed, Time.deltaTime);
    }
}
