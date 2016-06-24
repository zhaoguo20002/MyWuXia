///////////////////////////////////////////
//  CameraFilterPack - by VETASOFT 2015 ///
///////////////////////////////////////////
using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Distortion/Aspiration")]
public class CameraFilterPack_Distortion_Aspiration : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
[Range(0f, 1f)]
public float Value = 0.8f;
[Range(-1f, 1f)]
public float PosX = 0.5f;
[Range(-1f, 1f)]
public float PosY = 0.5f;
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
ChangeValue = Value;
ChangeValue2 = PosX;
ChangeValue3 = PosY;
ChangeValue4 = Value4;
SCShader = Shader.Find("CameraFilterPack/Distortion_Aspiration");
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
material.SetFloat("_Value", 1-Value);
material.SetFloat("_Value2", PosX);
material.SetFloat("_Value3", PosY);
material.SetFloat("_Value4", Value4);
material.SetVector("_ScreenResolution",new Vector4(sourceTexture.width,sourceTexture.height,0.0f,0.0f));
Graphics.Blit(sourceTexture, destTexture, material);
}
else
{
Graphics.Blit(sourceTexture, destTexture);
}
}
void OnValidate(){ChangeValue=Value;ChangeValue2=PosX;ChangeValue3=PosY;ChangeValue4=Value4;}void Update ()
{
if (Application.isPlaying)
{
Value = ChangeValue;
PosX = ChangeValue2;
PosY = ChangeValue3;
Value4 = ChangeValue4;
}
#if UNITY_EDITOR
if (Application.isPlaying!=true)
{
SCShader = Shader.Find("CameraFilterPack/Distortion_Aspiration");
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
