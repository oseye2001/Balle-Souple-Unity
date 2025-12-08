using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallKickOnFirstContact : MonoBehaviour
{
    [Header("Poussée après le premier impact")]
    public Vector3 kickDirection = new Vector3(1f, 0f, 0f); // direction sur la table
    public float kickForce = 2f; // intensité de la poussée

    [Header("Réaction de l'objet métal")]
    public float metalTorque = 5f;      // force de rotation autour de Y
    public float metalUpImpulse = 1f;   // petit saut vers le haut

    [Header("Réaction du verre")]
    public float glassTorque = 3f;      // rotation plus douce que le métal
    public float glassUpImpulse = 0.5f; // petit mouvement vers le haut

    [Header("Fin de scène")]
    public string groundTag = "Ground"; // tag du sol en bas
    private bool hasFallen = false;     // pour ne déclencher le tilt qu'une seule fois

    [Header("Son de rebond")]
    public AudioSource bounceAudio;
    public float minImpactSpeed = 1.5f;

    private bool kicked = false;
    private bool hitMetal = false;      // pour ne pas répéter l'effet 1000 fois
    private bool hitGlass = false;      // idem pour le verre
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // ----- Son de rebond en fonction de la vitesse d'impact -----
        float impactSpeed = collision.relativeVelocity.magnitude;

        if (impactSpeed > minImpactSpeed)
        {
            if (bounceAudio != null)
            {
                bounceAudio.pitch = Random.Range(0.95f, 1.05f);
                bounceAudio.Play();
            }
        }

        // 1) Premier contact avec la table (ou autre) -> donner la poussée à la balle
        if (!kicked)
        {
            rb.AddForce(kickDirection.normalized * kickForce, ForceMode.Impulse);
            kicked = true;
        }

        // 2) Si on touche l'objet métal -> réaction du métal + zoom caméra + son métallique
        if (!hitMetal && collision.collider.CompareTag("Metal"))
        {
            hitMetal = true;

            // ----- Réaction physique du métal -----
            Rigidbody metalRb = collision.rigidbody;
            if (metalRb == null)
            {
                metalRb = collision.collider.GetComponent<Rigidbody>();
            }

            if (metalRb != null)
            {
                // Petit saut vers le haut
                Vector3 upImpulse = Vector3.up * metalUpImpulse;
                metalRb.AddForce(upImpulse, ForceMode.Impulse);

                // Rotation/vibration autour de l'axe Y
                Vector3 torque = Vector3.up * metalTorque;
                metalRb.AddTorque(torque, ForceMode.Impulse);
            }

            // ----- Zoom caméra -----
            CameraZoom zoom = FindObjectOfType<CameraZoom>();
            if (zoom != null)
            {
                zoom.TriggerImpact();
            }

            // ----- Son métallique -----
            AudioSource metalAudio = collision.collider.GetComponent<AudioSource>();
            if (metalAudio != null)
            {
                metalAudio.pitch = Random.Range(1.03f, 1.08f);
                metalAudio.Play();
            }
        }

        // 3) Si on touche le verre -> petite réaction + son plus aigu
        if (!hitGlass && collision.collider.CompareTag("Glass"))
        {
            hitGlass = true;

            // ----- Réaction physique du verre -----
            Rigidbody glassRb = collision.rigidbody;
            if (glassRb == null)
            {
                glassRb = collision.collider.GetComponent<Rigidbody>();
            }

            if (glassRb != null)
            {
                // Petit mouvement vers le haut
                Vector3 upImpulse = Vector3.up * glassUpImpulse;
                glassRb.AddForce(upImpulse, ForceMode.Impulse);

                // Rotation plus douce autour de Y
                Vector3 torque = Vector3.up * glassTorque;
                glassRb.AddTorque(torque, ForceMode.Impulse);
            }

            // ----- Son du verre (plus aigu, plus fragile) -----
            AudioSource glassAudio = collision.collider.GetComponent<AudioSource>();
            if (glassAudio != null)
            {
                glassAudio.pitch = Random.Range(1.3f, 1.5f); // plus aigu que le métal
                glassAudio.Play();
            }

            // ----- Shake du verre -----
            GlassShake shaker = collision.collider.GetComponent<GlassShake>();
            if (shaker != null)
            {
                shaker.Shake();
            }
        }

        // 4) Fin de scène : la balle touche le sol en bas -> tilt caméra
        if (!hasFallen && collision.collider.CompareTag(groundTag))
        {
            hasFallen = true;

            CameraEndTilt tilt = FindObjectOfType<CameraEndTilt>();
            if (tilt != null)
            {
                tilt.TriggerTilt();
            }
        }
    }
}
