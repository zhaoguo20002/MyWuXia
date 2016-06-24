///////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 ///
///////////////////////////////////////////
using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/TV/Noise")]
public class CameraFilterPack_TV_Noise : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
[Range(0.0001f, 0.5f)]
private float Size = 0.01f;
[Range(-1.5f, 1.5f)]
private float LightBackGround = 0.5f;
[Range(0f, 10f)]
private float Speed = 1f;
[Range(0f, 10f)]
private float Size2 = 1f;
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
ChangeValue = Size;
ChangeValue2 = LightBackGround;
ChangeValue3 = Speed;
ChangeValue4 = Size2;
SCShader = Shader.Find("CameraFilterPack/TV_Noise");
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
material.SetFloat("_Value", Size);
material.SetFloat("_Value2", LightBackGround);
material.SetFloat("_Value3", Speed);
material.SetFloat("_Value4", Size2);
material.SetVector("_ScreenResolution",new Vector4(sourceTexture.width,sourceTexture.height,0.0f,0.0f));
Graphics.Blit(sourceTexture, destTexture, material);
}
else
{
Graphics.Blit(sourceTexture, destTexture);
}
}
void OnValidate(){ChangeValue=Size;ChangeValue2=LightBackGround;ChangeValue3=Speed;ChangeValue4=Size2;}void Update ()
{
if (Application.isPlaying)
{
Size = ChangeValue;
LightBackGround = ChangeValue2;
Speed = ChangeValue3;
Size2 = ChangeValue4;
}
#if UNITY_EDITOR
if (Application.isPlaying!=true)
{
SCShader = Shader.Find("CameraFilterPack/TV_Noise");
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
