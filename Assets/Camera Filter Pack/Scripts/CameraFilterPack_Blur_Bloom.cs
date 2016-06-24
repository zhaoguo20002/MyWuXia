////////////////////////////////////////////////////////////////////////////////////
// CameraFilterPack v2.0 - by VETASOFT 2015 //////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Blur/Bloom")]
public class CameraFilterPack_Blur_Bloom : MonoBehaviour {
	#region Variables
	public Shader SCShader;
	private float TimeX = 1.0f;
	private Vector4 ScreenResolution;
	private Material SCMaterial;
	[Range(0, 10)]
	public float Amount = 4.5f;
	[Range(0, 1)]
	public float Glow = 0.5f;

	public static float ChangeAmount;
	public static float ChangeGlow;
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
		ChangeAmount 	= Amount;
		ChangeGlow 		= Glow;

		SCShader = Shader.Find("CameraFilterPack/Blur_Bloom");

		if(!SystemInfo.supportsImageEffects)
		{
			enabled = false;
			return;
		}
	}
	
	void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if(SCShader != null)
		{
			TimeX+=Time.deltaTime;
			if (TimeX>100)  TimeX=0;
			material.SetFloat("_TimeX", TimeX);
			material.SetFloat("_Amount", Amount);
			material.SetFloat("_Glow", Glow);
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
		ChangeAmount=Amount;
		ChangeGlow=Glow;
		
}
	// Update is called once per frame
	void Update () 
	{
		if (Application.isPlaying)
		{
			Amount 	= ChangeAmount;
			Glow	= ChangeGlow;	

		}

		#if UNITY_EDITOR
		if (Application.isPlaying!=true)
		{
			SCShader = Shader.Find("CameraFilterPack/Blur_Bloom");

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