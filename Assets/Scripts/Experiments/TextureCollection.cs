using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using System.Collections.Generic;

public class TextureCollection : MonoBehaviour {

    public List<RenderTexture> textures=new List<RenderTexture>();
    Shader lightMapBuild, lightMapCombiner;


    public Camera cam;

	void Start ()
    {
        cam = GameObject.FindGameObjectWithTag(Tags.cam).GetComponent<Camera>();
	}
	
	void Update () {
	
	}
}
