using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "FPS/Player Data")]
public class PlayerData : ScriptableObject
{
    
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpHeight = 1.2f;
    public float gravity = -20f;

    
    public float mouseSensitivity = 0.1f;
    public float maxLookAngle = 85f;

    
    public float maxHealth = 100f;
}