using UnityEngine;
using UnityStandardAssets.ImageEffects;
using System.Collections.Generic;

/// <summary>
/// Скрипт, управляющий различными 2D эффектами освещения, такими как затемнение при смене времени суток или при входе в полностью замкнутое помещение
/// /// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraDarkController : PostEffectsBase
{
    private Material blackScreen;
    public Shader darkShader = null;


    public float darkness = 0f;

    public void Awake()
    {

    }

    public override bool CheckResources()
    {
        CheckSupport(false);
        blackScreen = CheckShaderAndCreateMaterial(darkShader, blackScreen);
        if (!isSupported)
            ReportAutoDisable();
        return isSupported;
    }

    public void OnDisable()
    {
        if (blackScreen)
            DestroyImmediate(blackScreen);
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!CheckResources() || blackScreen == null)
        {
            Graphics.Blit(source, destination);
            return;
        }
        blackScreen.SetFloat("_Darkness", darkness);
        Graphics.Blit(source, destination, blackScreen);

    }
}
