///////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 ///
///////////////////////////////////////////
using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Colors/DarkColor")]
public class CameraFilterPack_Colors_DarkColor : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
[Range(-5f, 5f)]
public float Alpha = 1f;
[Range(0f, 16f)]
private float Colors = 11f;
[Range(-1f, 1f)]
private float Green_Mod = 1f;
[Range(0f, 10f)]
private float Value4 = 1f;
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
ChangeValue = Alpha;
ChangeValue2 = Colors;
ChangeValue3 = Green_Mod;
ChangeValue4 = Value4;
SCShader = Shader.Find("CameraFilterPack/Colors_DarkColor");
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
material.SetFloat("_Value", Alpha);
material.SetFloat("_Value2", Colors);
material.SetFloat("_Value3", Green_Mod);
material.SetFloat("_Value4", Value4);
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
	ChangeValue=Alpha;
	ChangeValue2=Colors;
	ChangeValue3=Green_Mod;
	ChangeValue4=Value4;
	
}

void Update ()
{
if (Application.isPlaying)
{
Alpha = ChangeValue;
Colors = ChangeValue2;
Green_Mod = ChangeValue3;
Value4 = ChangeValue4;
}
#if UNITY_EDITOR
if (Application.isPlaying!=true)
{
SCShader = Shader.Find("CameraFilterPack/Colors_DarkColor");
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
