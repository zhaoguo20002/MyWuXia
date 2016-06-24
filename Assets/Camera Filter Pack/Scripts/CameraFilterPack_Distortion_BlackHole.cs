////////////////////////////////////////////////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 //////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Distortion/BlackHole")]
public class CameraFilterPack_Distortion_BlackHole : MonoBehaviour {
	#region Variables
	public Shader SCShader;
	private float TimeX = 1.0f;
	private Vector4 ScreenResolution;
	private Material SCMaterial;
	[Range(-1,1)]
	public float PositionX = 0f;
	[Range(-1,1)]
	public float PositionY = 0f;
	[Range(0,20)]
	public float Size = 1.5f;
	[Range(0,180)]
	public float Distortion = 30f;

	public static float ChangePositionX ;
	public static float ChangePositionY ;
	public static float ChangeSize ;
	public static float ChangeDistortion;

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
		ChangePositionX = PositionX;
		ChangePositionY = PositionY;
		ChangeSize 		= Size;
		ChangeDistortion= Distortion;

		SCShader = Shader.Find("CameraFilterPack/Distortion_BlackHole");

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
			material.SetFloat("_PositionX", PositionX);
			material.SetFloat("_PositionY", PositionY);
			material.SetFloat("_Distortion", Size);
			material.SetFloat("_Distortion2", Distortion);
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
		ChangePositionX=PositionX;
		ChangePositionY=PositionY;
		ChangeSize=Size;
		ChangeDistortion=Distortion;
}
	// Update is called once per frame
	void Update () 
	{
		if (Application.isPlaying)
		{
			PositionX 	= ChangePositionX;
			PositionY 	= ChangePositionY;
			Size 		= ChangeSize;
			Distortion 	= ChangeDistortion;
		}
		#if UNITY_EDITOR
		if (Application.isPlaying!=true)
		{
			SCShader = Shader.Find("CameraFilterPack/Distortion_BlackHole");

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