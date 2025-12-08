using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallDeformer : MonoBehaviour
{
    [Header("Paramètres physiques (ressort)")]
    public float stiffness = 30f;          // raideur k
    public float damping = 8f;             // amortissement c
    public float maxCompression = 0.25f;   // compression maximale

    [Header("Impact minimum pour déformer")]
    public float minImpactSpeed = 0.5f;

    [Header("Rebond lié à la compression")]
    public float reboundFactor = 3f;       // plus c'est grand, plus le rebond est fort

    private float compression = 0f;        // état actuel de compression
    private float velocity = 0f;           // vitesse de compression
    private Vector3 baseScale;
    private bool isColliding = false;
    private Rigidbody rb;

    // direction de l'impact (normale du contact)
    private Vector3 impactNormal = Vector3.up;

    void Start()
    {
        baseScale = transform.localScale;
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        float impactSpeed = collision.relativeVelocity.magnitude;

        if (impactSpeed > minImpactSpeed)
        {
            // normale du premier point de contact (direction du choc)
            impactNormal = collision.contacts[0].normal.normalized;

            // normalisation de l'impact -> niveau de compression
            float normalized = Mathf.Clamp01(impactSpeed / 10f);
            compression = maxCompression * normalized;
            velocity = 0f;
            isColliding = true;

            // Rebond lié à la compression :
            // plus la balle est écrasée, plus on renvoie d'énergie,
            // dans la direction de l'impact, modulée par l'angle
            if (rb != null)
            {
                // angleFactor : 1 si choc vertical, proche de 0 si très oblique
                float angleFactor = Mathf.Abs(Vector3.Dot(Vector3.up, impactNormal));
                float reboundImpulse = compression * reboundFactor * angleFactor;

                rb.AddForce(impactNormal * reboundImpulse, ForceMode.Impulse);
            }
        }
    }

    void Update()
    {
        if (!isColliding && Mathf.Abs(compression) < 0.0001f)
        {
            compression = 0f;
            velocity = 0f;
            return;
        }

        // Loi du ressort amorti : F = -k*x - c*v
        float force = -stiffness * compression - damping * velocity;

        velocity += force * Time.deltaTime;
        compression += velocity * Time.deltaTime;

        // stop si trop faible
        if (Mathf.Abs(compression) < 0.0001f)
        {
            compression = 0f;
        }

        // appliquer la déformation (squash) en fonction de la direction d'impact
        float squash = Mathf.Max(0f, compression);

        // on utilise la normale en valeur absolue pour répartir la compression
        Vector3 nAbs = new Vector3(
            Mathf.Abs(impactNormal.x),
            Mathf.Abs(impactNormal.y),
            Mathf.Abs(impactNormal.z)
        );

        // axes perpendiculaires (approximation, suffisant visuellement)
        Vector3 perp = Vector3.one - nAbs;

        // on part de l'échelle de base
        Vector3 newScale = baseScale;

        // on écrase dans les axes alignés avec la normale
        newScale -= Vector3.Scale(baseScale, nAbs) * squash;

        // on élargit un peu dans les axes perpendiculaires
        newScale += Vector3.Scale(baseScale, perp) * (squash * 0.5f);

        // sécurité pour éviter des scales négatives
        newScale.x = Mathf.Max(0.05f, newScale.x);
        newScale.y = Mathf.Max(0.05f, newScale.y);
        newScale.z = Mathf.Max(0.05f, newScale.z);

        transform.localScale = newScale;

        // on arrête quand le ressort est au repos
        if (compression == 0f && Mathf.Abs(velocity) < 0.0001f)
        {
            isColliding = false;
        }
    }
}
