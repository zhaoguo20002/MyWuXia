////////////////////////////////////////////////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 //////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Sharpen/Sharpen")]
public class CameraFilterPack_Sharpen_Sharpen : MonoBehaviour {
	#region Variables
	public Shader SCShader;

	[Range(0.001f, 100)]
	public float Value=4.0f;
	[Range(0.001f, 32)]
	public float Value2=1.0f;
	private float TimeX = 1.0f;
	private Vector4 ScreenResolution;
	private Material SCMaterial;

	public static float ChangeValue;
	public static float ChangeValue2;
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
		ChangeValue = Value;
		ChangeValue2 = Value2;
		SCShader = Shader.Find("CameraFilterPack/Sharpen_Sharpen");

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
			material.SetVector("_ScreenResolution",new Vector4(sourceTexture.width,sourceTexture.height,0.0f,0.0f));
			material.SetFloat("_Value", Value);
			material.SetFloat("_Value2", Value2);
			Graphics.Blit(sourceTexture, destTexture, material);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);	
		}
		
		
	}
		void OnValidate()
{
		ChangeValue=Value;
		ChangeValue2=Value2;

}
	// Update is called once per frame
	void Update () 
	{
		if (Application.isPlaying)
		{
			Value = ChangeValue;
			Value2 = ChangeValue2;
		}
		#if UNITY_EDITOR
		if (Application.isPlaying!=true)
		{
			SCShader = Shader.Find("CameraFilterPack/Sharpen_Sharpen");

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