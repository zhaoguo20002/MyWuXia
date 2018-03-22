using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(MeshRenderer))]
public class TextureFrameController : MonoBehaviour {
    public List<Texture> Textures;
    public float SkipTime = 0.05f;
    public int CurrentIndex = 0;
    public bool IsLoop = true;
    MeshRenderer render;
    float date;
    bool pause;
	// Use this for initialization
	void Start () {
        render = GetComponent<MeshRenderer>();
        pause = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (pause)
        {
            return;
        }
        float dt = Time.fixedTime;
        if (dt - date > SkipTime)
        {
            date = dt;
            render.material.SetTexture("_MainTex", Textures[CurrentIndex++]);
            CurrentIndex %= Textures.Count;
            if (!IsLoop && CurrentIndex == 0)
            {
                pause = true;
            }
        }
	}
}
