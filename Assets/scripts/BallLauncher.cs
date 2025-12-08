using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallLauncher : MonoBehaviour
{
    public Vector3 initialForce = new Vector3(1f, 0f, 0.5f); // direction
    public float forceMultiplier = 3f; // intensité du coup

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(initialForce.normalized * forceMultiplier, ForceMode.Impulse);
    }
}
