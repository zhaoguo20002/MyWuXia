///////////////////////////////////////////
//  CameraFilterPack - by VETASOFT 2015 ///
///////////////////////////////////////////
using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Drawing/Lines")]
public class CameraFilterPack_Drawing_Lines : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
[Range(0.1f, 10f)]
public float Number = 1f;
[Range(0f, 1f)]
public float Random = 0.5f;
[Range(0f, 10f)]
private float PositionY = 1f;
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
ChangeValue = Number;
ChangeValue2 = Random;
ChangeValue3 = PositionY;
ChangeValue4 = Value4;
SCShader = Shader.Find("CameraFilterPack/Drawing_Lines");
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
material.SetFloat("_Value", Number);
material.SetFloat("_Value2", Random);
material.SetFloat("_Value3", PositionY);
material.SetFloat("_Value4", Value4);
material.SetVector("_ScreenResolution",new Vector4(sourceTexture.width,sourceTexture.height,0.0f,0.0f));
Graphics.Blit(sourceTexture, destTexture, material);
}
else
{
Graphics.Blit(sourceTexture, destTexture);
}
}
void OnValidate(){ChangeValue=Number;ChangeValue2=Random;ChangeValue3=PositionY;ChangeValue4=Value4;}void Update ()
{
if (Application.isPlaying)
{
Number = ChangeValue;
Random = ChangeValue2;
PositionY = ChangeValue3;
Value4 = ChangeValue4;
}
#if UNITY_EDITOR
if (Application.isPlaying!=true)
{
SCShader = Shader.Find("CameraFilterPack/Drawing_Lines");
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
