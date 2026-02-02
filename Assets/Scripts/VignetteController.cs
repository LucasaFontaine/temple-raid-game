using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignetteController : MonoBehaviour
{
    public Volume volume; // Drag your Volume here
    private Vignette vignette;

    void Start()
    {
        // Get the vignette override
        if (volume.profile.TryGet<Vignette>(out Vignette v))
            vignette = v;
    }

    public void SetVignetteActive(bool active)
    {
        if (vignette != null)
            vignette.active = active;
    }
}
