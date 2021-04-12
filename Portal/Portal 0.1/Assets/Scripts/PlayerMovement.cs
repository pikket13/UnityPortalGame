using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;  // igralec
    
    public float moveSensitivity = 3.0f;    // horizontalno premikanje
    public float jumpSensitivity = 1.0f;    // vertikalno premikanje
    public float gravity = -10.0f;          // gravitacija
    public string sceneName;
    
    float dx = 0.0f;
    float dz = 0.0f;
    bool jump;
    bool isGrounded;

    Vector3 horizontalVelocity;             // horizontalno premikanje
    Vector3 verticalVelocity;               // vertikalno premikanje

    // za teleportiranje
    [SerializeField]
    private Collider playerCollider;

    private Quaternion halfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    private Quaternion minus = new Quaternion(0, -1, 0, 0);
    private Portal inPortal;
    private Portal outPortal;

    void Update() {
        // Preverimo vhodne podatke in shranimo trenutno stanje igralca
        dx = Input.GetAxis("Horizontal");
        dz = Input.GetAxis("Vertical");
        jump = Input.GetButtonDown("Jump");
        isGrounded = controller.isGrounded;

        // Horizontalno hitrost shranimo v vektor
        // Uporabimo clamp, da diagonalno premikanje ni hitrejše
        horizontalVelocity = Vector3.ClampMagnitude(transform.right * dx + transform.forward * dz, 1.0f);
        // Horizontalno hitrost skaliramo z občutljivostjo
        horizontalVelocity = horizontalVelocity * moveSensitivity;
        // Igralca premaknemo zaradi vpliva horizontalne hitrosti
        controller.Move(horizontalVelocity * Time.deltaTime);

        // Če smo na tleh, nastavimo vertikalno hitrost na 0
        // (zaradi vpliva gravitacije je morda negativna)
        if (isGrounded && verticalVelocity.y < 0) {
            verticalVelocity.y = 0;
        }
        
        // Skočimo, če je igralec pritisnil Jump in smo na tleh
        if (isGrounded && jump) {
            verticalVelocity.y += Mathf.Sqrt(jumpSensitivity * -2.0f * gravity);
        }

        // K vertikalni hitrosti prištejemo vpliv gravitacije
        verticalVelocity.y += gravity * Time.deltaTime;
        // Igralca premaknemo zaradi vpliva vertikalne hitrosti
        controller.Move(verticalVelocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    public virtual void WallToWall()
    {
        var inTransform = inPortal.transform;
        var outTransform = outPortal.transform;

        // Spremeni pozicijo
        Vector3 relativePos = inTransform.InverseTransformPoint(transform.position);
        transform.position = outTransform.TransformPoint(relativePos);

        // Spremeni rotacijo
        Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * transform.rotation;
        relativeRot = halfTurn * relativeRot;
        relativeRot = outTransform.rotation * relativeRot;
        relativeRot = Quaternion.Euler(0, relativeRot.eulerAngles.y, 0);
        transform.rotation = relativeRot;

        Physics.SyncTransforms();
    }

    public virtual void FloorToWall()
    {
        var inTransform = inPortal.transform;
        var outTransform = outPortal.transform;

        // Spremeni pozicijo
        Vector3 relativePos = inTransform.InverseTransformPoint(transform.position);
        transform.position = outTransform.TransformPoint(relativePos);

        // Spremeni rotacijo
        transform.rotation = outTransform.rotation * minus;

        Physics.SyncTransforms();
    }

    public void SetIsInPortal(Portal inPortal, Portal outPortal, Collider wallCollider)
    {
        // Definicija portalov
        this.inPortal = inPortal;
        this.outPortal = outPortal;

        // Pustimo prosto premikanje v portalu
        Physics.IgnoreCollision(playerCollider, wallCollider);
    }

    public virtual void Reset(Collider wallCollider)
    {
        // Vkljucimo nazaj collision
        Physics.IgnoreCollision(playerCollider, wallCollider, false);
    }
}
