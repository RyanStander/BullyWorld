using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NPCDialogueManager : MonoBehaviour
{
    public string dialogueFolder = "DialogueFiles"; // Folder name for storing NPC dialogue files
    [SerializeField] private GameObject[] npcPrefabs;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private float baseDistanceFromPlayer = 1f;
    [SerializeField] private float spawnInterval = 5f;

    private Dictionary<int, string[]>
        npcDialogueLines = new Dictionary<int, string[]>(); // Dictionary to store dialogue lines for each NPC

    private int npcCount = 0; // Number of NPCs found

    void Start()
    {
        LoadDialogueFiles();
        StartCoroutine(CreateNPCs());
    }

    void LoadDialogueFiles()
    {
        // Determine the correct path based on whether we are in the Unity Editor or a build
        string path = Application.dataPath + "/" + dialogueFolder;

        if (!Directory.Exists(path))
        {
            Debug.LogError("Dialogue folder not found: " + path);
            return;
        }

        // Search for all files in the dialogue folder with a .txt extension
        string[] files = Directory.GetFiles(path, "*.txt");

        foreach (string file in files)
        {
            // Read all lines from the dialogue file
            string[] lines = File.ReadAllLines(file);
            npcDialogueLines.Add(npcCount, lines);

            // Increment NPC count
            npcCount++;
        }
    }

    public GameObject GetPrefabForDictionaryItem(int dictionaryIndex)
    {
        // Calculate which prefab to use based on the current dictionary index
        int prefabIndex = (dictionaryIndex * npcPrefabs.Length) / npcDialogueLines.Count;

        // Return the selected prefab
        return npcPrefabs[prefabIndex];
    }

    IEnumerator CreateNPCs()
    {
        for (int i = 0; i < npcCount; i++)
        {
            // find a random position in a circle around this object
            Vector3 position = (Vector3)(Random.insideUnitCircle.normalized * spawnRadius) + transform.position;
            position.y = 0;
            GameObject obj = Instantiate(GetPrefabForDictionaryItem(i), position, Quaternion.identity);
            obj.GetComponent<SurroundPlayer>().SetupAgent(npcDialogueLines[i],i, npcCount,baseDistanceFromPlayer);

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
