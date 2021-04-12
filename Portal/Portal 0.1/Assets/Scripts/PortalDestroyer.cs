using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalDestroyer : MonoBehaviour
{
    public Transform player;
    public AudioClip destroySound;

    Transform transform;
    Vector3 rel;
    AudioSource source;
    bool current;

    void Start()
    {
        transform = gameObject.transform;
        source = GetComponent<AudioSource>();
        current = UpdateSide();
    }

    void FixedUpdate()
    {
        bool updated = UpdateSide();
        // Šli smo na drugo stran
        if (current != updated) {
            source.PlayOneShot(destroySound, 3);
            current = updated;
        }
    }

    bool UpdateSide() {
        rel = transform.InverseTransformPoint(player.position);
        float dot = Vector3.Dot(transform.forward, rel);
        if (dot > 0.0f) return true;
        if (dot < 0.0f) return false;
        return current;
    }
}
