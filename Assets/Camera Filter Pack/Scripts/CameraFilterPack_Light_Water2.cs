///////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 ///
///////////////////////////////////////////
using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Light/Water2")]
public class CameraFilterPack_Light_Water2 : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
[Range(0f, 10f)]
public float Speed = 0.2f;
[Range(0f, 10f)]
public float Speed_X = 0.2f;
[Range(0f, 1f)]
public float Speed_Y = 0.3f;
[Range(0f, 10f)]
public float Intensity = 2.4f;
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
ChangeValue2 = Speed_X;
ChangeValue3 = Speed_Y;
ChangeValue4 = Intensity;
SCShader = Shader.Find("CameraFilterPack/Light_Water2");
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
material.SetFloat("_Value2", Speed_X);
material.SetFloat("_Value3", Speed_Y);
material.SetFloat("_Value4", Intensity);
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
		ChangeValue2=Speed_X;
		ChangeValue3=Speed_Y;
		ChangeValue4=Intensity;

}

void Update ()
{
if (Application.isPlaying)
{
Speed = ChangeValue;
Speed_X = ChangeValue2;
Speed_Y = ChangeValue3;
Intensity = ChangeValue4;
}
#if UNITY_EDITOR
if (Application.isPlaying!=true)
{
SCShader = Shader.Find("CameraFilterPack/Light_Water2");
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
