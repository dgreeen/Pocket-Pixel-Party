using UnityEngine;

/// <summary>
/// Manages the player's respawn position and logic.
/// It prioritizes a directly assigned respawn point, falls back to finding a 'Respawn' tag,
/// and finally uses the player's initial position if neither is found.
/// </summary>
public class PlayerRespawn : MonoBehaviour
{
    [Tooltip("Assign the transform where the player should respawn. If left empty, will search for a 'Respawn' tag.")]
    [SerializeField] private Transform respawnPoint;

    private Vector3 _fallbackPosition;
    private Quaternion _fallbackRotation;

    void Awake()
    {
        // Store the object's initial position as the ultimate fallback.
        _fallbackPosition = transform.position;
        _fallbackRotation = transform.rotation;

        // 1. Check for a directly assigned respawn point.
        if (respawnPoint != null)
        {
            Debug.Log($"PlayerRespawn using assigned respawn point: {respawnPoint.name}");
            return; // Success, no more work needed.
        }

        // 2. If none is assigned, try to find one by tag.
        GameObject respawnObject = GameObject.FindGameObjectWithTag("Respawn");
        if (respawnObject != null)
        {
            respawnPoint = respawnObject.transform;
            Debug.Log($"PlayerRespawn found respawn point via tag: {respawnPoint.name}");
        }
        else // 3. If both checks fail, issue the warning.
        {
            Debug.LogWarning("Respawn point not assigned and no GameObject with tag 'Respawn' found. Using the player's initial position as a fallback.");
        }
    }

    public void Respawn()
    {
        Vector3 targetPosition = respawnPoint != null ? respawnPoint.position : _fallbackPosition;
        Quaternion targetRotation = respawnPoint != null ? respawnPoint.rotation : _fallbackRotation;

        transform.position = targetPosition;
        transform.rotation = targetRotation;

        Debug.Log($"Player respawned at position {targetPosition}.");
    }
}