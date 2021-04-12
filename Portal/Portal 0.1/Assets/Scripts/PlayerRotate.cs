using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    // Ko gledamo levo-desno, želimo rotirati tudi model igralca
    // Ko gledamo gor-dol, rotiramo samo objekt, ki je vezan na skripto (običajno kamera)
    public Transform player;
    
    public float leftRightsensitivity = 2.0f;
    public float upDownSensitivity = 2.0f;
    
    // Trenutna rotacija gor-dol (okoli x osi)
    float upDownRotation = 0.0f;
    
    void Start() {
        // Miškin kazalec se ne premakne na rob zaslona, kar
        // omogoča "neskončno" premikanje v levo ali desno
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void FixedUpdate() {
        // 1. Rotacija okoli y osi, ko gledamo levo-desno
        float mouseX = Input.GetAxis("Mouse X") * leftRightsensitivity;
        player.Rotate(Vector3.up * mouseX);
        
        // 2. Trenutni rotaciji okoli x osi odštejemo mouseY in omejimo
        // kot na 90 stopinj na obe strani. 
        float mouseY = Input.GetAxis("Mouse Y") * upDownSensitivity;
        upDownRotation = Mathf.Clamp(upDownRotation - mouseY, -90.0f, 90.0f);
        transform.localRotation = Quaternion.Euler(upDownRotation, 0.0f, 0.0f);
    }

}
