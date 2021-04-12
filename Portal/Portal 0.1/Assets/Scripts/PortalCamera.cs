using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    [SerializeField]
    private Portal bluePortal;
    [SerializeField]
    private Portal redPortal;

    [SerializeField]
    private Camera portalCamera;

    private RenderTexture bluePortalTex;
    private RenderTexture redPortalTex;

    private Camera mainCamera;

    private const int recursionLevel = 5;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();

        // Inicializacija
        bluePortalTex = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        redPortalTex = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
    }

    private void Start()
    {
        bluePortal.SetTexture(bluePortalTex);
        redPortal.SetTexture(redPortalTex);
    }

    private void OnPreRender()
    {
        if (!bluePortal.IsPlaced() || !redPortal.IsPlaced())
        {
            return;
        }

        // Ce sta portala vidna, se zacne izris

        if (bluePortal.IsRendererVisible())
        {
            portalCamera.targetTexture = bluePortalTex;
            for (int i = recursionLevel - 1; i >= 0; --i)
            {
                StartRender(bluePortal, redPortal, i);
            }
        }

        if (redPortal.IsRendererVisible())
        {
            portalCamera.targetTexture = redPortalTex;
            for (int i = recursionLevel - 1; i >= 0; --i)
            {
                StartRender(redPortal, bluePortal, i);
            }
        }
    }

    private void StartRender(Portal start, Portal end, int level)
    {

        // Shranimo lastnosti portalov
        Transform startTransform = start.transform;
        Transform endTransform = end.transform;

        // Shranimo lastnosti kamere
        Transform cameraTransform = portalCamera.transform;
        cameraTransform.position = transform.position;
        cameraTransform.rotation = transform.rotation;

        // Izracunamo polozaj in rotacijo na trenutnem nivoju rekurzije
        for (int i = 0; i <= level; ++i)
        {
            // Nastavimo kamero za portal, relativno na igralca
            Vector3 relT = startTransform.InverseTransformPoint(cameraTransform.position);
            relT = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relT;
            cameraTransform.position = endTransform.TransformPoint(relT);

            // Nastavimo rotacijo kamere, da gleda v portal
            Quaternion relR = Quaternion.Inverse(startTransform.rotation) * cameraTransform.rotation;
            relR = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relR;
            cameraTransform.rotation = endTransform.rotation * relR;
        }

        // Izracunamo oblique view frostrum 
        ObliqueViewMatrix(endTransform);

        portalCamera.Render();
    }

    private void ObliqueViewMatrix(Transform transform)
    {
        Plane p = new Plane(-transform.forward, transform.position);
        Vector4 clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace =Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlane;
        portalCamera.projectionMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
    }
}
