using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float distance;
    public float height;
    public float lookpoint;
    public Transform target;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!target)
            return;

        transform.position = Vector3.Lerp(transform.position, target.position - target.forward*distance + target.up*height, Time.deltaTime*7);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime*7);
    }
}