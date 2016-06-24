///////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 ///
///////////////////////////////////////////
using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/TV/WideScreenHorizontal")]
public class CameraFilterPack_TV_WideScreenHorizontal : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
[Range(0f, 0.8f)]
public float Size = 0.55f;
[Range(0.001f, 0.4f)]
public float Smooth = 0.01f;
[Range(0f, 10f)]
private float StretchX = 1f;
[Range(0f, 10f)]
private float StretchY = 1f;
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
ChangeValue2 = Smooth;
ChangeValue3 = StretchX;
ChangeValue4 = StretchY;
SCShader = Shader.Find("CameraFilterPack/TV_WideScreenHorizontal");
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
material.SetFloat("_Value2", Smooth);
material.SetFloat("_Value3", StretchX);
material.SetFloat("_Value4", StretchY);
material.SetVector("_ScreenResolution",new Vector4(sourceTexture.width,sourceTexture.height,0.0f,0.0f));
Graphics.Blit(sourceTexture, destTexture, material);
}
else
{
Graphics.Blit(sourceTexture, destTexture);
}
}
void OnValidate(){ChangeValue=Size;ChangeValue2=Smooth;ChangeValue3=StretchX;ChangeValue4=StretchY;}void Update ()
{
if (Application.isPlaying)
{
Size = ChangeValue;
Smooth = ChangeValue2;
StretchX = ChangeValue3;
StretchY = ChangeValue4;
}
#if UNITY_EDITOR
if (Application.isPlaying!=true)
{
SCShader = Shader.Find("CameraFilterPack/TV_WideScreenHorizontal");
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
