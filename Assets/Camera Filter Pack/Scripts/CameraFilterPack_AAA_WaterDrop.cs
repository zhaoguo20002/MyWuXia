////////////////////////////////////////////////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 //////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/AAA/WaterDrop")]
public class CameraFilterPack_AAA_WaterDrop : MonoBehaviour {
	#region Variables
	public Shader SCShader;
	private float TimeX = 1.0f;
	[Range(8, 64)]
	public float Distortion = 8.0f;
	[Range(0, 7)]
	public float SizeX = 1.0f;
	[Range(0, 7)]
	public float SizeY = 0.5f;
	[Range(0, 10)]
	public float Speed = 1f;
	private Material SCMaterial;
	private Texture2D Texture2;

	public static float ChangeDistortion;
	public static float ChangeSizeX;
	public static float ChangeSizeY;
	public static float ChangeSpeed;

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
		ChangeDistortion = Distortion;
		ChangeSizeX=SizeX;
		ChangeSizeY=SizeY;
		ChangeSpeed=Speed;

		Texture2 = Resources.Load ("CameraFilterPack_WaterDrop") as Texture2D;
		
		SCShader = Shader.Find("CameraFilterPack/AAA_WaterDrop");
		
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
			material.SetFloat("_Distortion", Distortion);
			material.SetFloat("_SizeX", SizeX);
			material.SetFloat("_SizeY", SizeY);
			material.SetFloat("_Speed", Speed);
			material.SetTexture("_MainTex2", Texture2);

			Graphics.Blit(sourceTexture, destTexture, material);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);	
		}
		
		
	}
	void OnValidate()
{
		ChangeDistortion=Distortion;
		ChangeSizeX=SizeX;
		ChangeSizeY=SizeY;
		ChangeSpeed=Speed;
}
	// Update is called once per frame
	void Update () 
	{
		if (Application.isPlaying)
		{
			Distortion = ChangeDistortion;
			SizeX = ChangeSizeX;
			SizeY = ChangeSizeY;
			Speed = ChangeSpeed;
		}
		#if UNITY_EDITOR
		if (Application.isPlaying!=true)
		{
			SCShader = Shader.Find("CameraFilterPack/AAA_WaterDrop");
			Texture2 = Resources.Load ("CameraFilterPack_WaterDrop") as Texture2D;
			
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