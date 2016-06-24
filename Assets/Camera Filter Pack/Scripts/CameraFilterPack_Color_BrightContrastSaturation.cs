////////////////////////////////////////////////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 //////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Colors/BrightContrastSaturation")]
public class CameraFilterPack_Color_BrightContrastSaturation : MonoBehaviour {
	#region Variables
	public Shader SCShader;
	private float TimeX = 1.0f;
	private Vector4 ScreenResolution;
	private Material SCMaterial;

	[Range(0.0f, 10.0f)] public float Brightness = 2.0f;
	[Range(0.0f, 10.0f)] public float Saturation = 1.5f;
	[Range(0.0f, 10.0f)] public float Contrast = 1.5f;

	public static float ChangeBrightness ;
	public static float ChangeSaturation ;
	public static float ChangeContrast ;

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
		ChangeBrightness = Brightness;
		ChangeSaturation = Saturation;
		ChangeContrast   = Contrast;

		SCShader = Shader.Find("CameraFilterPack/Color_BrightContrastSaturation");

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
			material.SetFloat("_Brightness", Brightness);
			material.SetFloat("_Saturation", Saturation);
			material.SetFloat("_Contrast", Contrast);
			material.SetFloat("_TimeX", TimeX);
			material.SetVector("_ScreenResolution",new Vector4(sourceTexture.width,sourceTexture.height,0.0f,0.0f));
			Graphics.Blit(sourceTexture, destTexture, material);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);	
		}
		
		
	}
void OnValidate()
{
	ChangeBrightness=Brightness;
	ChangeSaturation=Saturation;
	ChangeContrast=Contrast;
	
}
	// Update is called once per frame
	void Update () 
	{
		if (Application.isPlaying)
		{
			Brightness  = ChangeBrightness ;
			Saturation  = ChangeSaturation ;
			Contrast 	= ChangeContrast;
		}
		#if UNITY_EDITOR
		if (Application.isPlaying!=true)
		{
			SCShader = Shader.Find("CameraFilterPack/Color_BrightContrastSaturation");

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