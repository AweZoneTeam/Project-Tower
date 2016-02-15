using UnityEngine;
using UnityStandardAssets.ImageEffects;
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class PortalEffect : PostEffectsBase
{
    private Material portalMaterial;
    public Shader PortalShader = null;
    public override bool CheckResources()
    {
        CheckSupport(false);
        portalMaterial = CheckShaderAndCreateMaterial(PortalShader, portalMaterial);
        if (!isSupported)
            ReportAutoDisable();
        return isSupported;
    }

    public void OnDisable()
    {
        if (portalMaterial)
            DestroyImmediate(portalMaterial);
    }
    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!CheckResources() || portalMaterial == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        Graphics.Blit(source, destination, portalMaterial);
    }
}