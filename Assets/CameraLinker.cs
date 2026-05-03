using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;

public class CameraLinker : MonoBehaviour
{
    private CinemachineCamera vCam;

    void Start()
    {
        // Get reference to the Cinemachine camera component
        vCam = GetComponent<CinemachineCamera>();
    }

    void Update()
    {
        // Check if the local player object is spawned and camera is not set
        if (NetworkManager.Singleton.LocalClient != null && 
            NetworkManager.Singleton.LocalClient.PlayerObject != null && 
            vCam.Follow == null)
        {
            // Set the player as the target for the camera
            Transform playerTransform = NetworkManager.Singleton.LocalClient.PlayerObject.transform;
            vCam.Follow = playerTransform;
            vCam.LookAt = playerTransform;
        }
    }
}