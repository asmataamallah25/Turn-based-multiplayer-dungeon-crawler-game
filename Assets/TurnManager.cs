using UnityEngine;
using Unity.Netcode;

public class TurnManager : NetworkBehaviour
{
    // Tracks the current player's turn index
    public NetworkVariable<int> activePlayerIndex = new NetworkVariable<int>(0);
    
    // The countdown timer, synchronized across all clients
    // Set to 5 seconds for the turn duration
    public NetworkVariable<float> turnTimer = new NetworkVariable<float>(5.0f);

    private void Update()
    {
        // Only the server handles logic to ensure synchronization
        if (!IsServer) return;

        if (turnTimer.Value > 0)
        {
            turnTimer.Value -= Time.deltaTime;
        }
        else
        {
            // Time is up, switch to next player
            EndTurn();
        }
    }

    private void EndTurn()
    {
        // Reset timer to 5 seconds and move to next player index
        turnTimer.Value = 5.0f;
        activePlayerIndex.Value = (activePlayerIndex.Value + 1) % NetworkManager.Singleton.ConnectedClientsList.Count;
    }
}