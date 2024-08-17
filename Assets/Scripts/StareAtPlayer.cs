using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StareAtPlayer : MonoBehaviour
{
    [SerializeField] private Transform head;
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 lookAtOffset;
    [SerializeField] private float maxAngle = 90f;
    [SerializeField] private float turnSpeed = 5f;

    private Quaternion originalRotation;
    
    private void OnValidate()
    {
        if (head == null)
            head = GetComponent<Transform>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        // Store the original rotation of the head
        originalRotation = head.localRotation;
    }

    private void Update()
    {
        StareAt();
    }

    private void StareAt()
    {
        // Calculate the target position with the offset applied
        Vector3 targetPosition = player.position + lookAtOffset;

        // Calculate the direction to the target position
        Vector3 directionToPlayer = targetPosition - head.position;

        // Calculate the angle between the head's forward direction and the direction to the target position
        float angleToPlayer = Vector3.Angle(head.forward, directionToPlayer);

        // If the player is within the max angle range
        if (angleToPlayer <= maxAngle)
        {
            // Calculate the target rotation to look at the player with offset
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            // Smoothly rotate the head towards the target position
            head.rotation = Quaternion.Slerp(head.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
        else
        {
            // Return the head to its original rotation if the player is out of range
            head.localRotation = Quaternion.Slerp(head.localRotation, originalRotation, turnSpeed * Time.deltaTime);
        }
    }
}
