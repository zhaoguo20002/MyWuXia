///////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 ///
///////////////////////////////////////////
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Split Screen/SideBySide")]
public class CameraFilterPack_Blend2Camera_SplitScreen : MonoBehaviour {
	#region Variables
	private string ShaderName="CameraFilterPack/Blend2Camera_SplitScreen";
	public Shader SCShader;
	public Camera Camera2; 
	private float TimeX = 1.0f;

	private Material SCMaterial;
	[Range(0f, 1f)]
	public float SwitchCameraToCamera2 = 0f;
	[Range(0f, 1f)] 
	public float BlendFX = 1f;
	[Range(-3f, 3f)]
	public float SplitX = 0.5f;
	[Range(-3f, 3f)]
	public float SplitY = 0.5f;
	[Range(0f, 2f)]
	public float Smooth = 0.1f;
	[Range(-3.14f, 3.14f)]
	public float Rotation = 3.14f;
	private bool ForceYSwap = false;
	public static float ChangeValue;
	public static float ChangeValue2;
	public static float ChangeValue3;
	public static float ChangeValue4;
	public static float ChangeValue5;
	public static float ChangeValue6;
	public static bool ChangeValue7;
	private RenderTexture Camera2tex;
	private Vector2 ScreenSize;
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
		ScreenSize.x = Screen.width;
		ScreenSize.y = Screen.height;
	
		if (Camera2 !=null)
		{DestroyImmediate(Camera2.targetTexture);
			Camera2tex=new RenderTexture((int)ScreenSize.x ,(int)ScreenSize.y, 24); 
			Camera2.targetTexture=Camera2tex;
		}
		
		ChangeValue = BlendFX;
		ChangeValue2=SwitchCameraToCamera2;
		ChangeValue3=SplitX;
		ChangeValue6=SplitY;
		ChangeValue4=Smooth;
		ChangeValue5=Rotation;
		ChangeValue7 = ForceYSwap;
		SCShader = Shader.Find(ShaderName);
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
			if (Camera2 != null) material.SetTexture("_MainTex2",Camera2tex);
			material.SetFloat("_TimeX", TimeX);
			material.SetFloat("_Value", BlendFX);
			material.SetFloat("_Value2", SwitchCameraToCamera2);
			material.SetFloat("_Value3", SplitX);
			material.SetFloat("_Value6", SplitY);
			material.SetFloat("_Value4", Smooth);
			material.SetFloat("_Value5", Rotation);
			material.SetInt ("_ForceYSwap", ForceYSwap ? 0:1 );
			Graphics.Blit(sourceTexture, destTexture, material);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);
		}
	}
	void OnValidate()
	{	
		ChangeValue=BlendFX;
		ChangeValue2=SwitchCameraToCamera2;
		ChangeValue3=SplitX;
		ChangeValue6=SplitY;
		ChangeValue7=ForceYSwap;
		ChangeValue4=Smooth;
		ChangeValue5=Rotation;

	}
	void Update ()
	{
		ScreenSize.x = Screen.width;
		ScreenSize.y = Screen.height;
		if (Application.isPlaying)
		{
			BlendFX = ChangeValue;
			SwitchCameraToCamera2 = ChangeValue2;
			SplitX = ChangeValue3;
			SplitY = ChangeValue6;
			Smooth = ChangeValue4;
			Rotation = ChangeValue5;
			ForceYSwap=ChangeValue7;
		}
		#if UNITY_EDITOR
		if (Application.isPlaying!=true)
		{
			SCShader = Shader.Find(ShaderName);
		}
		#endif
	}
	void OnEnable () { Start (); }
	void OnDisable ()
	{
		if (Camera2 !=null) { DestroyImmediate(Camera2.targetTexture); Camera2.targetTexture=null; }
		if(SCMaterial)
		{
			DestroyImmediate(SCMaterial);
		}
	}
}
