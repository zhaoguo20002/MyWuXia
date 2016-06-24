///////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 ///
///////////////////////////////////////////
using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Pixelisation/Dot")]
public class CameraFilterPack_Pixelisation_Dot : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
[Range(0.0001f, 0.5f)]
public float Size = 0.005f;
[Range(0f, 1f)]
public float LightBackGround = 0.3f;
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
SCShader = Shader.Find("CameraFilterPack/Pixelisation_Dot");
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
SCShader = Shader.Find("CameraFilterPack/Pixelisation_Dot");
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
