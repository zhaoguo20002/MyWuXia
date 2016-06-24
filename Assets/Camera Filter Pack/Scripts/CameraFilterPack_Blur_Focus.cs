////////////////////////////////////////////////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 //////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Blur/Focus")]
public class CameraFilterPack_Blur_Focus : MonoBehaviour {
	#region Variables
	public Shader SCShader;
	private float TimeX = 1.0f;
	private Vector4 ScreenResolution;
	private Material SCMaterial;
	[Range(-1, 1)]
	public float CenterX = 0f;
	[Range(-1, 1)]
	public float CenterY = 0f;
	[Range(0, 10)]
	public float _Size = 5f;
	[Range(0.12f, 64)]
	public float _Eyes = 2f;

	public static float ChangeCenterX ;
	public static float ChangeCenterY ;
	public static float ChangeSize ;
	public static float ChangeEyes ;

	#endregion
	
	#region Properties
	Material material
	{
		get
		{
			if(SCMaterial == null)
			{
				SCMaterial = new Material(SCShader);
				SCMaterial.hideFlags = HideFlags.HideAndDontSave;	
			}
			return SCMaterial;
		}
	}
	#endregion
	void Start () 
	{
		ChangeCenterX = CenterX;
		ChangeCenterY = CenterY;
		ChangeSize = _Size;
		ChangeEyes = _Eyes;
		SCShader = Shader.Find("CameraFilterPack/Blur_Focus");

		if(!SystemInfo.supportsImageEffects)
		{
			enabled = false;
			return;
		}
	}
	
	void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if (SCShader != null)
		{
			TimeX+=Time.deltaTime;
			if (TimeX>100)  TimeX=0;
			material.SetFloat("_TimeX", TimeX);
			material.SetFloat("_CenterX", CenterX);
			material.SetFloat("_CenterY", CenterY);
			float result = Mathf.Round(_Size/0.2f)*0.2f;
			material.SetFloat("_Size", result);
			material.SetFloat("_Circle", _Eyes);
			material.SetVector("_ScreenResolution",new Vector2(Screen.width,Screen.height));
			Graphics.Blit(sourceTexture, destTexture, material);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);
		}
		
		
	}
	
void OnValidate()
{
		ChangeCenterX=CenterX;
		ChangeCenterY=CenterY;
		ChangeSize=_Size;
		ChangeEyes=_Eyes;	
}
	// Update is called once per frame
	void Update () 
	{
		if (Application.isPlaying)
		{
			CenterX = ChangeCenterX ;
			CenterY = ChangeCenterY;
			_Size = ChangeSize;
			_Eyes = ChangeEyes;
		}
		#if UNITY_EDITOR
		if (Application.isPlaying!=true)
		{
			SCShader = Shader.Find("CameraFilterPack/Blur_Focus");

		}
		#endif

	}
	
	void OnDisable ()
	{
		if(SCMaterial)
		{
			DestroyImmediate(SCMaterial);	
		}
		
	}
	
	
}