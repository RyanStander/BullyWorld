using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetInsulted : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI insultTextField;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float insultInterval = 10f;
    [SerializeField] private float removeInsultAfter = 5f;
    [SerializeField] private float maxDistance = 3f;
    private float insultTimestamp;

    private void Update()
    {
        if (Time.time >= insultTimestamp + insultInterval)
        {
            GameObject centermostNPC = GetCentermostNPCToInsult();
            if (centermostNPC != null)
            {
                // Assuming you have a method or field to get the insult text from the NPC
                SurroundPlayer dialogue = centermostNPC.GetComponent<SurroundPlayer>();
                insultTextField.text =
                    dialogue.GetRandomDialogueLine(); // Assuming GetInsult() is a method returning the insult
                
                StartCoroutine(ClearTextAfterDelay());
            }

            insultTimestamp = Time.time;
        }
    }

    private GameObject GetCentermostNPCToInsult()
    {
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC"); // Assuming NPCs are tagged as "NPC"
        GameObject centermostNPC = null;
        float minDistanceFromCenter = Mathf.Infinity;

        foreach (GameObject npc in npcs)
        {
            SurroundPlayer dialogueLine = npc.GetComponent<SurroundPlayer>();
            if (dialogueLine != null)
            {
                // Calculate the distance from the player to the NPC
                float distanceToPlayer = Vector3.Distance(transform.position, npc.transform.position);

                // Ignore NPCs that are further than the maximum distance
                if (distanceToPlayer > maxDistance)
                {
                    continue;
                }
                
                // Convert NPC position to screen position
                Vector3 screenPosition = mainCamera.WorldToScreenPoint(npc.transform.position);

                // Ignore NPCs behind the camera
                if (screenPosition.z < 0)
                {
                    continue;
                }

                // Calculate the distance from the center of the screen
                Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
                float distanceFromCenter = Vector2.Distance(screenPosition, screenCenter);

                // Check if this is the closest NPC to the center
                if (distanceFromCenter < minDistanceFromCenter)
                {
                    minDistanceFromCenter = distanceFromCenter;
                    centermostNPC = npc;
                }
            }
        }

        return centermostNPC;
    }
    
    private IEnumerator ClearTextAfterDelay()
    {
        yield return new WaitForSeconds(removeInsultAfter+insultInterval);
        insultTextField.text = "";
    }
}
