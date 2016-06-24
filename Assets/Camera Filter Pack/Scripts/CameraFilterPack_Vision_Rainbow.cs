///////////////////////////////////////////
//  CameraFilterPack - by VETASOFT 2015 ///
///////////////////////////////////////////
using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Vision/Rainbow")]
public class CameraFilterPack_Vision_Rainbow : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
[Range(0f, 10f)]
public float Speed = 1f;
[Range(0f, 1f)]
public float PosX = 0.5f;
[Range(0f, 1f)]
public float PosY = 0.5f;
[Range(0f, 5f)]
public float Colors = 0.5f;
[Range(0f, 1f)]
public float Vision = 0.5f;

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
ChangeValue = Speed;
ChangeValue2 = PosX;
ChangeValue3 = PosY;
ChangeValue4 = Colors;
ChangeValue5 = Vision;
SCShader = Shader.Find("CameraFilterPack/Vision_Rainbow");
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
material.SetFloat("_Value2", PosX);
material.SetFloat("_Value3", PosY);
material.SetFloat("_Value4", Colors);
material.SetFloat("_Value5", Vision);
material.SetVector("_ScreenResolution",new Vector4(sourceTexture.width,sourceTexture.height,0.0f,0.0f));
Graphics.Blit(sourceTexture, destTexture, material);
}
else
{
Graphics.Blit(sourceTexture, destTexture);
}
}
	void OnValidate(){ChangeValue=Speed;ChangeValue2=PosX;ChangeValue3=PosY;ChangeValue4=Colors;ChangeValue5=Vision;}void Update ()
{
if (Application.isPlaying)
{
Speed = ChangeValue;
PosX = ChangeValue2;
PosY = ChangeValue3;
Colors = ChangeValue4;
Vision = ChangeValue5;
}
#if UNITY_EDITOR
if (Application.isPlaying!=true)
{
SCShader = Shader.Find("CameraFilterPack/Vision_Rainbow");
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
