using UnityEngine;
using System.Collections;

public class CameraEndTilt : MonoBehaviour
{
    public Transform camPivot;
    public float tiltAngle = 2f;
    public float duration = 0.3f;

    private float baseX;

    void Start()
    {
        // on prend le parent par défaut (CameraRig)
        if (camPivot == null)
            camPivot = transform;

        // rotation X initiale
        baseX = camPivot.localEulerAngles.x;
    }

    public void TriggerTilt()
    {
        StartCoroutine(TiltRoutine());
    }

    IEnumerator TiltRoutine()
    {
        float startX = baseX;
        float targetX = baseX + tiltAngle;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = t / duration;
            float x = Mathf.Lerp(startX, targetX, alpha);
            Vector3 e = camPivot.localEulerAngles;
            camPivot.localRotation = Quaternion.Euler(x, e.y, e.z);
            yield return null;
        }
    }
}
