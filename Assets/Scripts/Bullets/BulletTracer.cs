using UnityEngine;

public class BulletTracer : MonoBehaviour
{
    [SerializeField] float speed = 200f;

    Vector3 target;
    bool arrived;
    TrailRenderer trail;

    void Awake()
    {
        trail = GetComponent<TrailRenderer>();
    }

    public void Init(Vector3 target)
    {
        this.target = target;
    }

    void Update()
    {
        if (arrived) return;

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (transform.position == target)
        {
            arrived = true;
            Destroy(gameObject, trail != null ? trail.time : 0f);
        }
    }
}