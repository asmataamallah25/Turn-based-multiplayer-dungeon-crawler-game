using UnityEngine;
using Unity.Netcode;

public class PlayerInitializer : NetworkBehaviour
{
    void Start()
    {
        // Check if this is the player object for the local client
        if (IsOwner)
        {
            // Find the spawn point in the scene
            GameObject spawnPoint = GameObject.Find("SpawnPoint");
            if (spawnPoint != null)
            {
                // Move the player to the spawn point
                transform.position = spawnPoint.transform.position;
            }
        }
    }
}