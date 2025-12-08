using UnityEngine;
using System.Collections;

public class CameraZoom : MonoBehaviour
{
    public Camera cam;

    [Header("Zoom settings")]
    public float zoomIn = 52f;
    public float zoomOut = 60f;
    public float speed = 2f;

    [Header("Pan settings")]
    public float panAngle = 2f;      // rotation en degrés
    public float panDuration = 0.1f; // durée du pan

    private bool triggered = false;
    private bool panning = false;    // pour éviter de relancer pendant l'effet
    private Quaternion baseRotation; // rotation initiale pour revenir doucement

    void Start()
    {
        if (cam == null)
            cam = Camera.main;

        cam.fieldOfView = zoomOut;

        // On mémorise la rotation d'origine
        baseRotation = cam.transform.localRotation;
    }

    void Update()
    {
        // --- Gérer le zoom ---
        if (triggered)
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoomIn, Time.deltaTime * speed);
        else
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoomOut, Time.deltaTime * (speed * 0.5f));

        // --- Revenir doucement à la rotation normale quand zoom fini ---
        if (!triggered && !panning)
            cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, baseRotation, Time.deltaTime * 2f);
    }

    public void TriggerImpact()
    {
        triggered = true;

        // lancer le pan seulement si déjà pas en cours
        if (!panning)
            StartCoroutine(PanRoutine());
    }

    public void ResetZoom()
    {
        triggered = false;
    }

    IEnumerator PanRoutine()
    {
        panning = true;

        // rotation de départ
        Quaternion startRot = cam.transform.localRotation;

        // angle cible (rotation en Y)
        Quaternion targetRot = Quaternion.Euler(
            startRot.eulerAngles.x,
            startRot.eulerAngles.y + panAngle,
            startRot.eulerAngles.z
        );

        float t = 0f;

        // Pan (0 → target)
        while (t < panDuration)
        {
            t += Time.deltaTime;
            float a = t / panDuration;
            cam.transform.localRotation = Quaternion.Lerp(startRot, targetRot, a);
            yield return null;
        }

        // petite pause optionnelle pour laisser le pan visible :
        yield return new WaitForSeconds(0.02f);

        // retour (target → baseRotation)
        t = 0f;
        while (t < panDuration)
        {
            t += Time.deltaTime;
            float a = t / panDuration;
            cam.transform.localRotation = Quaternion.Lerp(targetRot, baseRotation, a);
            yield return null;
        }

        // fin
        cam.transform.localRotation = baseRotation;
        panning = false;
    }
}
