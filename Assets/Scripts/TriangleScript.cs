using UnityEngine;

public class TriangleScript : MonoBehaviour
{

    [SerializeField] private float speed = 10.0f;
    [SerializeField] private float separationStrength = 3.0f;
    [SerializeField] private float cohesionStrength = 0.5f;
    [SerializeField] private float adhesionStrength = 0.3f;
    [SerializeField] private float visionRadius = 4.5f;

    private float separationRadius = 1.5f;
    private Collider2D[] trianglesInVision;

    private float rotationSpeed = 100.0f;
    private float targetRotation;
    private float angle;
    private float screenLeft, screenRight, screenTop, screenBottom;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        CalculateScreenBounds();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Wrap();
        trianglesInVision = visibleTriangles(visionRadius);

        Vector2 velocityVector = (
                                 separation(separationStrength)
                                + cohesion(cohesionStrength)
                                + adhesion(adhesionStrength)
                                );
        if (velocityVector.magnitude < 0.5f)
        {
            velocityVector = velocityVector.normalized * 0.5f;
        }

        if (velocityVector.magnitude > speed)
        {
            velocityVector = velocityVector.normalized * speed;
        }
        targetRotation = Vector2.SignedAngle(transform.up, velocityVector);
        angle = Mathf.MoveTowardsAngle(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

        Vector2 pos = rb.position + (Vector2)velocityVector * speed * Time.fixedDeltaTime;

        rb.MovePositionAndRotation(pos, angle);

    }

    Vector2 separation(float separationStrength)
    {
        Collider2D[] nearbyTriangles = Physics2D.OverlapCircleAll(transform.position, separationRadius);
        Vector2 separationVector = Vector2.zero;
        int count = 0;

        foreach (var obj in nearbyTriangles)
        {
            if (obj.gameObject != this.gameObject)
            {
                Vector2 diff = (Vector2)(transform.position - obj.transform.position); // Vector pointing away
                separationVector += diff;
                count++;
            }
        }
        if (count > 1) separationVector /= count;
        separationVector = separationVector * separationStrength;

        return separationVector;
    }

    Collider2D[] visibleTriangles(float visionRadius)
    {
        Collider2D[] trianglesInVision = Physics2D.OverlapCircleAll(transform.position, visionRadius);
        return trianglesInVision;
    }
    Vector2 cohesion(float cohesionStrength)
    {
        Vector2 cohesionVector = Vector2.zero;
        foreach (var obj in trianglesInVision)
        {
            cohesionVector += (Vector2)(obj.transform.position);
        }
        if (trianglesInVision.Length > 0) cohesionVector /= trianglesInVision.Length;
        cohesionVector = ((cohesionVector - (Vector2)transform.position)) * cohesionStrength; 
        return cohesionVector;
    }

    Vector2 adhesion(float adhesionStrength)
    {
        Vector2 adhesionVector = Vector2.zero;
        foreach (var obj in trianglesInVision)
        {
            adhesionVector += (Vector2)(obj.transform.up);
        }
        if (trianglesInVision.Length > 0) adhesionVector /= trianglesInVision.Length;
        adhesionVector *= adhesionStrength;
        return adhesionVector;
    }

    void CalculateScreenBounds()
    {
        Camera cam = Camera.main;
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        screenLeft = -halfWidth + halfWidth * 0.1f;
        screenRight = halfWidth - halfWidth * 0.1f;
        screenTop = halfHeight - halfHeight * 0.1f;
        screenBottom = -halfHeight + halfHeight * 0.1f;
    }

    void Wrap()
    {
        Vector2 pos = rb.position;
        bool flag = false;

        if (pos.x < screenLeft)
        {
            pos.x = screenRight;
            flag = true;
        }
        else if (pos.x > screenRight)
        {
            pos.x = screenLeft;
            flag = true;
        }

        if (pos.y > screenTop)
        {
            pos.y = screenBottom;
            flag = true;
        }
        else if (pos.y < screenBottom)
        {
            pos.y = screenTop;
            flag = true;
        }

        if (flag) rb.position = pos;
    }
}
