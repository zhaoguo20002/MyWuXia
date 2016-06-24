///////////////////////////////////////////
//  CameraFilterPack - by VETASOFT 2015 ///
///////////////////////////////////////////
using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/FX/DigitalMatrix")]
public class CameraFilterPack_FX_DigitalMatrix : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
[Range(0.4f, 5f)]
public float Size = 1f;
[Range(-10f, 10f)]
public float Speed = 1f;
[Range(-1f, 1f)]
public float ColorR = -1f;
[Range(-1f, 1f)]
public float ColorG = 1f;
[Range(-1f, 1f)]
public float ColorB = -1f;
public static float ChangeValue;
public static float ChangeValue2;
public static float ChangeValue3;
public static float ChangeValue4;
public static float ChangeValue5;
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
ChangeValue2 = ColorR;
ChangeValue3 = ColorG;
ChangeValue4 = ColorB;
ChangeValue5=Speed;
SCShader = Shader.Find("CameraFilterPack/FX_DigitalMatrix");
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
material.SetFloat("_Value2", ColorR);
material.SetFloat("_Value3", ColorG);
material.SetFloat("_Value4", ColorB);
material.SetFloat("_Value5", Speed);
material.SetVector("_ScreenResolution",new Vector4(sourceTexture.width,sourceTexture.height,0.0f,0.0f));
Graphics.Blit(sourceTexture, destTexture, material);
}
else
{
Graphics.Blit(sourceTexture, destTexture);
}
}
	void OnValidate(){ChangeValue=Size;ChangeValue2=ColorR;ChangeValue3=ColorG;ChangeValue4=ColorB; ChangeValue5=Speed; }void Update ()
{
if (Application.isPlaying)
{
Size = ChangeValue;
ColorR = ChangeValue2;
ColorG = ChangeValue3;
ColorB = ChangeValue4;
Speed=ChangeValue5;
}
#if UNITY_EDITOR
if (Application.isPlaying!=true)
{
SCShader = Shader.Find("CameraFilterPack/FX_DigitalMatrix");
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
