using UnityEngine;

public class TriangleScript : MonoBehaviour
{

    private float speed;
    private float separationStrength;
    private float cohesionStrength;
    private float adhesionStrength;
    private float visionRadius;

    private float separationRadius = 1.5f;
    private Collider2D[] trianglesInVision;

    private float rotationSpeed = 100.0f;
    private float targetRotation;
    private float angle;
    private float screenLeft, screenRight, screenTop, screenBottom;

    private Rigidbody2D rb;

    public void setParameters(float sp, float sep, float coh, float adh, float view)
    {
        speed = sp;
        separationStrength = sep;
        cohesionStrength = coh;
        adhesionStrength = adh;
        visionRadius = view;
    }

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
                                 rb.linearVelocity
                                + separation(separationStrength)
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

        rb.linearVelocity = velocityVector;
        rb.MoveRotation(angle);

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
                separationVector += diff.normalized * (separationRadius - diff.magnitude);
                count++;
            }
        }
        if (count > 1) separationVector /= count;
        separationVector = separationVector.normalized * separationStrength;

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
        cohesionVector = ((cohesionVector - (Vector2)transform.position).normalized) * cohesionStrength;
        return cohesionVector;
    }

    Vector2 adhesion(float adhesionStrength)
    {
        Vector2 adhesionVector = Vector2.zero;
        foreach (var obj in trianglesInVision)
        {
            Rigidbody2D obj_rb = obj.GetComponent<Rigidbody2D>();
            adhesionVector += (Vector2)(obj_rb.linearVelocity);
        }
        if (trianglesInVision.Length > 0) adhesionVector /= trianglesInVision.Length;
        adhesionVector = adhesionVector.normalized * adhesionStrength;
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
