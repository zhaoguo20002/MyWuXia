using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(MeshRenderer))]
public class TextureFrameController : MonoBehaviour {
    public List<Texture> Textures;
    public float SkipTime = 0.05f;
    public int CurrentIndex = 0;
    MeshRenderer render;
    float date;
	// Use this for initialization
	void Start () {
        render = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        float dt = Time.fixedTime;
        if (dt - date > SkipTime)
        {
            date = dt;
            render.material.SetTexture("_MainTex", Textures[CurrentIndex++]);
            CurrentIndex %= Textures.Count;
        }
	}
}
