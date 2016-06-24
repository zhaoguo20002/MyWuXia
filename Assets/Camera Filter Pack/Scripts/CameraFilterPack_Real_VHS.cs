////////////////////////////////////////////////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 //////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu ("CameraFilterPack/VHS/Real VHS HQ")]
public class CameraFilterPack_Real_VHS : MonoBehaviour {
	#region Variables
	public Shader SCShader;
	private Material SCMaterial;
	private Texture2D VHS;
	private Texture2D VHS2;
	[Range(0, 1)]
	public float TRACKING=0.212f;
	[Range(0, 1.5f)]
	public float Constrast=1f;

	
	public static float ChangeDistortion;

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
		SCShader = Shader.Find("CameraFilterPack/Real_VHS");
		VHS = Resources.Load ("CameraFilterPack_VHS1") as Texture2D;
		VHS2 = Resources.Load ("CameraFilterPack_VHS2") as Texture2D;

		
		if(!SystemInfo.supportsImageEffects)
		{
			enabled = false;
			return;
		}
	}



	static public Texture2D GetRTPixels(Texture2D t, RenderTexture rt, int sx,int sy) 
	{
		
		// Remember currently active render texture
		RenderTexture currentActiveRT = RenderTexture.active;
		
		// Set the supplied RenderTexture as the active one
		RenderTexture.active = rt;

		// Create a new Texture2D and read the RenderTexture image into it
	//	Debug.Log (rt.width + " " + rt.height);
		t.ReadPixels(new Rect(0, 0, t.width, t.height), 0, 0);
		
		// Restorie previously active render texture
		RenderTexture.active = currentActiveRT;
		return t;
	}

	void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if(SCShader != null)
		{
			material.SetTexture("VHS", VHS);
			material.SetTexture("VHS2", VHS2);
			material.SetFloat("TRACKING", TRACKING);
			material.SetFloat("CONTRAST", 1-Constrast);
			int rtW = 382;
			int rtH = 576;
			RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);
			buffer.filterMode=FilterMode.Trilinear;
			Graphics.Blit(sourceTexture, buffer, material);
			Graphics.Blit(buffer, destTexture);
			RenderTexture.ReleaseTemporary(buffer);

		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);	
		}
		
		
	}

	// Update is called once per frame
	void Update () 
	{
		#if UNITY_EDITOR
		if (Application.isPlaying!=true)
		{
			SCShader = Shader.Find("CameraFilterPack/Real_VHS");
			VHS = Resources.Load ("CameraFilterPack_VHS1") as Texture2D;
			VHS2 = Resources.Load ("CameraFilterPack_VHS2") as Texture2D;
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