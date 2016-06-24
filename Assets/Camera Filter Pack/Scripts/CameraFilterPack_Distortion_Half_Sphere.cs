////////////////////////////////////////////////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 //////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Distortion/Half_Sphere")]
public class CameraFilterPack_Distortion_Half_Sphere : MonoBehaviour {
	#region Variables
	public Shader SCShader;
	private float TimeX = 1.0f;
	[Range(1, 6)]
	private Vector4 ScreenResolution;
	private Material SCMaterial;

	public float SphereSize = 2.5f;
	[Range(-1, 1)]
	public float SpherePositionX = 0f;
	[Range(-1, 1)]
	public float SpherePositionY = 0f;
	[Range(1, 10)]
	public float Strength = 5f;

	public static float ChangeSphereSize;
	public static float ChangeSpherePositionX;
	public static float ChangeSpherePositionY;
	public static float ChangeStrength;

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

		ChangeSphereSize = SphereSize;
		ChangeSpherePositionX = SpherePositionX;
		ChangeSpherePositionY = SpherePositionY;
		ChangeStrength = Strength;


		SCShader = Shader.Find("CameraFilterPack/Distortion_Half_Sphere");

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
			material.SetFloat("_SphereSize", SphereSize);
			material.SetFloat("_SpherePositionX", SpherePositionX);
			material.SetFloat("_SpherePositionY", SpherePositionY);
			material.SetFloat("_Strength", Strength);
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
		ChangeSphereSize=SphereSize;
		ChangeSpherePositionX=SpherePositionX;
		ChangeSpherePositionY=SpherePositionY;
		ChangeStrength=Strength;
	
}
	// Update is called once per frame
	void Update () 
	{
		if (Application.isPlaying)
		{
			SphereSize = ChangeSphereSize;
			SpherePositionX = ChangeSpherePositionX;
			SpherePositionY = ChangeSpherePositionY;
			Strength = ChangeStrength;
		}
		#if UNITY_EDITOR
		if (Application.isPlaying!=true)
		{
			SCShader = Shader.Find("CameraFilterPack/Distortion_Half_Sphere");

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