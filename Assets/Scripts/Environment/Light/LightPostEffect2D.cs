using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class LightPostEffect2D : MonoBehaviour
{
    private Material lightScreen;
    private Camera lightCam;

    private Camera thisCam;
    public LayerMask lightLayers;
    RenderTexture renderLightTexture;


    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!Init())
            return;

        lightCam.CopyFrom(thisCam);
        lightCam.backgroundColor = Color.black;
        lightCam.clearFlags = CameraClearFlags.Color;
        lightCam.cullingMask = lightLayers;

        renderLightTexture = RenderTexture.GetTemporary(lightCam.pixelWidth, lightCam.pixelHeight, 0, RenderTextureFormat.DefaultHDR);

        lightCam.targetTexture = renderLightTexture;
        lightCam.Render();

        lightScreen.SetTexture("_LightTex", renderLightTexture);

        Graphics.Blit(source, destination, lightScreen);

        RenderTexture.ReleaseTemporary(renderLightTexture);
    }

    private bool Init()
    {
        if (!thisCam)
        {
            thisCam = GetComponent<Camera>();

            if (!thisCam)
                return false;
        }

        if (!lightScreen)
        {
            Shader shader = Shader.Find("Project Tower/LightPostEffectShader");
            if (!shader)
                return false;
            lightScreen = new Material(shader);
        }

        if (!lightCam)
        {
            lightCam = new GameObject().AddComponent<Camera>();
            lightCam.enabled = false;
            lightCam.gameObject.hideFlags = HideFlags.HideAndDontSave;
        }

        return true;
    }

    internal void OnDisable()
    {
        if (lightCam)
            DestroyImmediate(lightCam.gameObject);
    }
}
