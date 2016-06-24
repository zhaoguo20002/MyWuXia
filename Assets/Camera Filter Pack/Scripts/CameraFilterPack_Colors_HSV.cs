////////////////////////////////////////////////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 //////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Colors/HSV")]
public class CameraFilterPack_Colors_HSV : MonoBehaviour {
	#region Variables
	public Shader SCShader;
	[Range(0, 360)]
	public float _HueShift = 180f;
	[Range(-32, 32)]
	public float _Saturation = 1f;
	[Range(-32, 32)]
	public float _ValueBrightness = 1f;
	private Material SCMaterial;

	public static float ChangeHueShift ;
	public static float ChangeSaturation ;
	public static float ChangeValueBrightness;

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
		ChangeHueShift  		= _HueShift;
		ChangeSaturation 		= _Saturation;
		ChangeValueBrightness 	= _ValueBrightness;
		SCShader = Shader.Find("CameraFilterPack/Colors_HSV");

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
			material.SetFloat("_HueShift", _HueShift);
			material.SetFloat("_Sat", _Saturation);
			material.SetFloat("_Val", _ValueBrightness);

			Graphics.Blit(sourceTexture, destTexture, material);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);	
		}
		
		
	}
void OnValidate()
{
	ChangeHueShift=_HueShift;
		ChangeSaturation=_Saturation;
		ChangeValueBrightness=_ValueBrightness;
	
		
}
	// Update is called once per frame
	void Update () 
	{
		if (Application.isPlaying)
		{
			_HueShift = ChangeHueShift;
			_Saturation = ChangeSaturation;
			_ValueBrightness = ChangeValueBrightness;
		}
		#if UNITY_EDITOR
		if (Application.isPlaying!=true)
		{
			SCShader = Shader.Find("CameraFilterPack/Colors_HSV");
			material.SetFloat("_HueShift", _HueShift);
			material.SetFloat("_Sat", _Saturation);
			material.SetFloat("_Val", _ValueBrightness);

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