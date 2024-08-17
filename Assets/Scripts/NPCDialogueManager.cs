using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NPCDialogueManager : MonoBehaviour
{
    public string dialogueFolder = "DialogueFiles"; // Folder name for storing NPC dialogue files
    private Dictionary<int, string[]> npcDialogueLines = new Dictionary<int, string[]>(); // Dictionary to store dialogue lines for each NPC
    private int npcCount = 0; // Number of NPCs found

    void Start()
    {
        LoadDialogueFiles();
        CreateNPCs();
    }

    void LoadDialogueFiles()
    {
        // Determine the correct path based on whether we are in the Unity Editor or a build
        string path = Application.dataPath +"/"+ dialogueFolder;

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

    void CreateNPCs()
    {
        Debug.Log(npcDialogueLines.Count);
        
        for (int i = 0; i < npcCount; i++)
        {
            
        }
    }
}
