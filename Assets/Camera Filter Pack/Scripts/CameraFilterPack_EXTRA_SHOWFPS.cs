///////////////////////////////////////////
//  CameraFilterPack - by VETASOFT 2015 ///
///////////////////////////////////////////
using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/EXTRA/SHOWFPS")]
public class CameraFilterPack_EXTRA_SHOWFPS : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
[Range(8f, 42f)]
public float Size = 12f;
[Range(0, 100)]
private int FPS = 1;
[Range(0f, 10f)]
private float Value3 = 1f;
[Range(0f, 10f)]
private float Value4 = 1f;


public static float ChangeValue;
public static int ChangeValue2;
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
FPS = 0;
StartCoroutine( FPSX() );
ChangeValue = Size;
ChangeValue2 = FPS;
ChangeValue3 = Value3;
ChangeValue4 = Value4;
SCShader = Shader.Find("CameraFilterPack/EXTRA_SHOWFPS");
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
material.SetFloat("_Value2", (float)FPS);
material.SetFloat("_Value3", Value3);
material.SetFloat("_Value4", Value4);
material.SetVector("_ScreenResolution",new Vector4(sourceTexture.width,sourceTexture.height,0.0f,0.0f));
Graphics.Blit(sourceTexture, destTexture, material);
}
else
{
Graphics.Blit(sourceTexture, destTexture);
}
}
void OnValidate(){ChangeValue=Size;ChangeValue2=FPS;ChangeValue3=Value3;ChangeValue4=Value4;}

private float accum   = 0f; // FPS accumulated over the interval
private int   frames  = 0; // Frames drawn over the interval
	public  float frequency = 0.5F; // The update frequency of the fps

	IEnumerator FPSX()
	{
		while( true )
		{
			// Update the FPS
			float fps = accum/frames;
			FPS = (int)fps;
			ChangeValue2=(int)fps;
			accum = 0.0F;
			frames = 0;

			yield return new WaitForSeconds( frequency );
		}
	}

void Update ()
{

		accum += Time.timeScale/ Time.deltaTime;
		++frames;

if (Application.isPlaying)
{
Size = ChangeValue;
FPS = ChangeValue2;
Value3 = ChangeValue3;
Value4 = ChangeValue4;
}
else FPS = 9999;
#if UNITY_EDITOR
if (Application.isPlaying!=true)
{
SCShader = Shader.Find("CameraFilterPack/EXTRA_SHOWFPS");
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
