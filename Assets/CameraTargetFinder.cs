using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;

public class CameraTargetFinder : MonoBehaviour
{
    // Reference to the virtual camera component
    private CinemachineCamera vCam;

    void Start()
    {
        vCam = GetComponent<CinemachineCamera>();
        // Wait briefly for the server to spawn the player objects
        Invoke(nameof(FindAndFollow), 0.5f);
    }

    void FindAndFollow()
    {
        // Iterate through all connected clients to find the local player
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject != null && client.PlayerObject.IsOwner)
            {
                // Set the virtual camera to follow and look at the local player
                vCam.Follow = client.PlayerObject.transform;
                vCam.LookAt = client.PlayerObject.transform;
                break;
            }
        }
    }
}