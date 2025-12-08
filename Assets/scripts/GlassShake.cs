using UnityEngine;
using System.Collections;

public class GlassShake : MonoBehaviour
{
    public float intensity = 0.03f;
    public float duration = 0.15f;

    public void Shake()
    {
        StartCoroutine(ShakeRoutine());
    }

    IEnumerator ShakeRoutine()
    {
        Vector3 start = transform.localPosition;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;

            float offset = Mathf.Sin(t * 40f) * intensity;

            // vibration horizontale
            transform.localPosition = start + new Vector3(offset, 0f, 0f);

            yield return null;
        }

        transform.localPosition = start;
    }
}
