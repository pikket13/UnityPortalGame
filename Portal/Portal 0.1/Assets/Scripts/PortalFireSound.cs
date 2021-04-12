using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalFireSound : MonoBehaviour
{
    public AudioClip bluePortal;
    public AudioClip redPortal;

    AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown ("Fire1")) {
            source.PlayOneShot(bluePortal, 1);
        }

        if (Input.GetButtonDown ("Fire2")) {
            source.PlayOneShot(redPortal, 1);
        }
    }
}
