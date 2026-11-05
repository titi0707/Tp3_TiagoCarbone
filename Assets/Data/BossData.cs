using UnityEngine;

[CreateAssetMenu(fileName = "BossData", menuName = "FPS/Boss Data")]
public class BossData : ScriptableObject
{
   
    public float maxHealth = 600f;

    public float moveSpeed = 5f;

   
    public int cylinderSize = 6;
    public int minBurst = 2;
    public int maxBurst = 3;
    public float aimTime = 0.6f;
    public float timeBetweenShots = 0.35f;
    public float reloadTime = 3f;

    
    public float bulletSpeed = 22f;
    public float bulletDamage = 12f;
    public int maxBounces = 1;
    public float ricochetChance = 0.4f;

    
    public float phase2SpeedMultiplier = 1.4f;
    public float phase2RicochetChance = 0.7f;
}