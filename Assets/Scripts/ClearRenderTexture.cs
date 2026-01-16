using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ClearRenderTexture : MonoBehaviour
{
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void OnPreRender()
    {
        if (cam.targetTexture == null) return;

        RenderTexture.active = cam.targetTexture;
        GL.Clear(true, true, cam.backgroundColor);
        RenderTexture.active = null;
    }
}
