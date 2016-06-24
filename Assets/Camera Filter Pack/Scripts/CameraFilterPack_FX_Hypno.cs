///////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 ///
///////////////////////////////////////////
using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/FX/Hypno")]
public class CameraFilterPack_FX_Hypno : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
[Range(0f, 1f)]
public float Speed = 1f;
[Range(-2f, 2f)]
public float Red = 0f;
[Range(-2f, 2f)]
public float Green = 1f;
[Range(-2f, 2f)]
public float Blue = 1f;
public static float ChangeValue;
public static float ChangeValue2;
public static float ChangeValue3;
public static float ChangeValue4;
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
ChangeValue = Speed;
ChangeValue2 = Red;
ChangeValue3 = Green;
ChangeValue4 = Blue;
SCShader = Shader.Find("CameraFilterPack/FX_Hypno");
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
material.SetFloat("_Value", Speed);
material.SetFloat("_Value2", Red);
material.SetFloat("_Value3", Green);
material.SetFloat("_Value4", Blue);
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
		ChangeValue=Speed;
		ChangeValue2=Red;
		ChangeValue3=Green;
		ChangeValue4=Blue;
	
}
void Update ()
{
if (Application.isPlaying)
{
Speed = ChangeValue;
Red = ChangeValue2;
Green = ChangeValue3;
Blue = ChangeValue4;
}
#if UNITY_EDITOR
if (Application.isPlaying!=true)
{
SCShader = Shader.Find("CameraFilterPack/FX_Hypno");
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
