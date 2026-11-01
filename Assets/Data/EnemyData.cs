using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "FPS/Enemy Data")]
public class EnemyData : ScriptableObject
{
   
    public float patrolSpeed = 2f;
    public float waitTimeAtPoint = 1.5f;

    
    public float detectionRange = 15f;
    public float viewAngle = 110f;
    public float eyeHeight = 0.8f;
    public float loseSightTime = 3f;

    
    public float chaseSpeed = 4.5f;
    public float attackRange = 2f;
    public float attackDamage = 10f;
    public float attackCooldown = 1f;

    
    public float maxHealth = 100f;
}