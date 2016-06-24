///////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 ///
///////////////////////////////////////////
using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Vision/ThermaVision")]
public class CameraFilterPack_Oculus_ThermaVision : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
[Range(0f, 1f)]
public float Therma_Variation = 0.5f;
[Range(0f, 8f)]
private float Contrast = 3f;
[Range(0f, 4f)]
private float Burn = 0f;
[Range(0f, 16f)]
private float SceneCut = 1f;
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
ChangeValue = Therma_Variation;
ChangeValue2 = Contrast;
ChangeValue3 = Burn;
ChangeValue4 = SceneCut;
SCShader = Shader.Find("CameraFilterPack/Oculus_ThermaVision");
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
material.SetFloat("_Value", Therma_Variation);
material.SetFloat("_Value2", Contrast);
material.SetFloat("_Value3", Burn);
material.SetFloat("_Value4", SceneCut);
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
		ChangeValue=Therma_Variation;
		ChangeValue2=Contrast;
		ChangeValue3=Burn;
		ChangeValue4=SceneCut;

}
void Update ()
{
if (Application.isPlaying)
{
Therma_Variation = ChangeValue;
Contrast = ChangeValue2;
Burn = ChangeValue3;
SceneCut = ChangeValue4;
}
#if UNITY_EDITOR
if (Application.isPlaying!=true)
{
SCShader = Shader.Find("CameraFilterPack/Oculus_ThermaVision");
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
