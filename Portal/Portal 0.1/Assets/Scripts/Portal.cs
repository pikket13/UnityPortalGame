using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Portal : MonoBehaviour
{
    [SerializeField]
    private Portal otherPortal;

    [SerializeField]
    private Renderer outlineRenderer;

    [SerializeField]
    private Color portalColor;

    [SerializeField]
    private LayerMask placementMask;

    [SerializeField]
    private Collider wallCollider;

    [SerializeField]
    private PlayerMovement player;

    private PlayerMovement colliding = null;
    private bool isPlaced;

    private Material material;
    private new Renderer renderer;
    private new BoxCollider collider;

    private void Awake()
    {
        // Inicializacija
        collider = GetComponent<BoxCollider>();
        renderer = GetComponent<Renderer>();
        material = renderer.material;
    }

    private void Start()
    {
        // Nastavi barvo portaloma
        material.SetColor("_Colour", portalColor);
        outlineRenderer.material.SetColor("_OutlineColour", portalColor);
    }

    private void Update()
    {
        if (colliding)
        {
            Vector3 playerPos = transform.InverseTransformPoint(colliding.transform.position);

            // Ce se je igralec ze sprehodil skozi portal, ga teleportiramo na drugo stran
            if (playerPos.z > 0.0f)
            {
                if (this.wallCollider.tag == "Floor" && otherPortal.wallCollider.tag != "Floor")
                {
                    colliding.FloorToWall();
                }
                else
                {
                    colliding.WallToWall();
                }
            }
        }
    }

    // Dobi igralca in ga nastavi v spremenljivko colliding. Klice setIsInPortal, ki nastavi portale in ugasne collision
    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            colliding = player;
            player.SetIsInPortal(this, otherPortal, wallCollider);
        }
    }

    // Colliding nastavi na null. Klice Reset, ki nazaj prizge collision
    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<PlayerMovement>();

        if (player != null)
        {
            colliding = null;
            player.Reset(wallCollider);
        }
    }

    // Postavi portal
    public void PlacePortal(Vector3 pos, Collider wallCollider)
    {
        this.wallCollider = wallCollider;
        if (this.wallCollider.tag == "Floor")
        {
            RotatePortal(pos);
            transform.position = pos;
            transform.position -= transform.forward * 0.06f;
            isPlaced = true;
        }
        else if (this.wallCollider.tag == "Wall")
        {
            transform.rotation = this.wallCollider.transform.rotation;
            transform.position = pos;
            transform.position -= transform.forward * 0.001f;
            isPlaced = true;
        }
    }

    // Pomagac za postavljanje tal
    public void RotatePortal(Vector3 pos)
    {
        float xDist = pos.x - player.transform.position.x;
        float zDist = pos.z - player.transform.position.z;

        if (Mathf.Abs(xDist) > Mathf.Abs(zDist))
        {
            if (xDist > 0)
            {
                transform.rotation = Quaternion.Euler(90, 0, -90);
            }
            else
            {
                transform.rotation = Quaternion.Euler(90, 0, 90);
            }
        }
        else
        {
            if (zDist > 0)
            {
                transform.rotation = Quaternion.Euler(90, 0, 0);
            }
            else 
            {
                transform.rotation = Quaternion.Euler(90, 0, 180);
            }
        }
    }

    // Pomagaci
    public bool IsPlaced()
    {
        return isPlaced;
    }

    public void SetTexture(RenderTexture tex)
    {
        material.mainTexture = tex;
    }

    public bool IsRendererVisible()
    {
        return renderer.isVisible;
    }
}
