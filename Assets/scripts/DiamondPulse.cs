using UnityEngine;

public class DiamondPulse : MonoBehaviour
{
    public Material diamondMat;
    public float speed = 1f;

    void Start()
    {
        // Si tu préfères l'assigner automatiquement :
        if (diamondMat == null)
        {
            Renderer r = GetComponent<Renderer>();
            if (r != null)
            {
                // on duplique le matériau pour cet objet
                diamondMat = r.material;
            }
        }

        if (diamondMat != null)
        {
            diamondMat.EnableKeyword("_EMISSION");
        }
    }

    void Update()
    {
        if (diamondMat == null) return;

        // pulse entre 0.2 et 0.5
        float pulse = Mathf.PingPong(Time.time * speed, 0.3f) + 0.2f;

        Color baseColor = new Color(0.7f, 1f, 1f); // bleu très clair
        Color finalEmission = baseColor * pulse;

        diamondMat.SetColor("_EmissionColor", finalEmission);
    }
}
