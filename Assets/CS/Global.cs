using UnityEngine;
using System.Collections;
using Game;
using Newtonsoft.Json.Linq;
using System;

public class Global : MonoBehaviour {
    public static float FrameCost = 0.033f;
	public bool TestModel = false;
	GameObject UICamera;
	GameObject showFps;
	GameObject UICanvas;
	GameObject FrameCanvas;
	GameObject FontCanvas;
	GameObject UIEventSystem;
	void Awake () {
		QualitySettings.vSyncCount = -1;
		QualitySettings.maxQueuedFrames = 0;
		Application.targetFrameRate = 30;
        FrameCost = (float)Statics.ClearError(1.0d / (double)Application.targetFrameRate); //每一帧消耗的固定时间
		UICamera = GameObject.Find("UICamera");
		if (UICamera != null) {
			DontDestroyOnLoad(UICamera);
		}
		showFps = GameObject.Find("ShowFPS");
		if (showFps != null) {
			DontDestroyOnLoad(showFps);
		}
		UICanvas = GameObject.Find("UICanvas");
		if (UICanvas != null) {
			DontDestroyOnLoad(UICanvas);
		}
		FrameCanvas = GameObject.Find("FrameCanvas");
		if (FrameCanvas != null) {
			DontDestroyOnLoad(FrameCanvas);
		}
		FontCanvas = GameObject.Find("FontCanvas");
		if (FontCanvas != null) {
			DontDestroyOnLoad(FontCanvas);
		}
		UIEventSystem = GameObject.Find("UIEventSystem");
		if (UIEventSystem != null) {
			DontDestroyOnLoad(UIEventSystem);
		}

		DontDestroyOnLoad(gameObject);
		Statics.Init();
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        DESStatics.Init();
	}

	void Start() {
		if (TestModel) {
			return;
		}
        Messenger.Broadcast(NotifyTypes.ShowMainPanel);

//        string receipt = "ewoJInNpZ25hdHVyZSIgPSAiQXd2Umh6NnhSYlI4azdwbFJ3ek9ZYlFhZlVEYlEwM3cyTU44TWNJZjhWTHhPNXo5RGxGYzhZSUs4VEsxTXZaSU1WUnE1aVU2NGZFT1VwMEgwaC9mcERFc3MxZHlpNXpua0tKMy9mREkvWkt0VXhhTU00TjAzYkhXbSsxeE5iWHZGeXBjajBXVUtzblZ2L1N5WndHZjFRZFJZbUREYkc5Y0lvTnNZejVFRVhTd1BoSzVJaDV1NWVTY0tVNVRjWnd2azk3QXR6NE5BcXRpK0E1S05IamNOZitCeTNvRXJlRDYwMDgvS3Zqb045cHRQQndHTHcyWEFKc1U1UnJPdnJhMGRBMUN2TUVpOUNzR2Y4NXU0LzRxdkcwN2doQ29yRkxEcDRMbDIzdFVMZzAxUGJQc1dJQjNXRmVtNzR3aGVSUzdrb2c0WWRWWFF4REF6Z3o0TDAvSjJjd0FBQVdBTUlJRmZEQ0NCR1NnQXdJQkFnSUlEdXRYaCtlZUNZMHdEUVlKS29aSWh2Y05BUUVGQlFBd2daWXhDekFKQmdOVkJBWVRBbFZUTVJNd0VRWURWUVFLREFwQmNIQnNaU0JKYm1NdU1Td3dLZ1lEVlFRTERDTkJjSEJzWlNCWGIzSnNaSGRwWkdVZ1JHVjJaV3h2Y0dWeUlGSmxiR0YwYVc5dWN6RkVNRUlHQTFVRUF3dzdRWEJ3YkdVZ1YyOXliR1IzYVdSbElFUmxkbVZzYjNCbGNpQlNaV3hoZEdsdmJuTWdRMlZ5ZEdsbWFXTmhkR2x2YmlCQmRYUm9iM0pwZEhrd0hoY05NVFV4TVRFek1ESXhOVEE1V2hjTk1qTXdNakEzTWpFME9EUTNXakNCaVRFM01EVUdBMVVFQXd3dVRXRmpJRUZ3Y0NCVGRHOXlaU0JoYm1RZ2FWUjFibVZ6SUZOMGIzSmxJRkpsWTJWcGNIUWdVMmxuYm1sdVp6RXNNQ29HQTFVRUN3d2pRWEJ3YkdVZ1YyOXliR1IzYVdSbElFUmxkbVZzYjNCbGNpQlNaV3hoZEdsdmJuTXhFekFSQmdOVkJBb01Da0Z3Y0d4bElFbHVZeTR4Q3pBSkJnTlZCQVlUQWxWVE1JSUJJakFOQmdrcWhraUc5dzBCQVFFRkFBT0NBUThBTUlJQkNnS0NBUUVBcGMrQi9TV2lnVnZXaCswajJqTWNqdUlqd0tYRUpzczl4cC9zU2cxVmh2K2tBdGVYeWpsVWJYMS9zbFFZbmNRc1VuR09aSHVDem9tNlNkWUk1YlNJY2M4L1cwWXV4c1FkdUFPcFdLSUVQaUY0MWR1MzBJNFNqWU5NV3lwb041UEM4cjBleE5LaERFcFlVcXNTNCszZEg1Z1ZrRFV0d3N3U3lvMUlnZmRZZUZScjZJd3hOaDlLQmd4SFZQTTNrTGl5a29sOVg2U0ZTdUhBbk9DNnBMdUNsMlAwSzVQQi9UNXZ5c0gxUEttUFVockFKUXAyRHQ3K21mNy93bXYxVzE2c2MxRkpDRmFKekVPUXpJNkJBdENnbDdaY3NhRnBhWWVRRUdnbUpqbTRIUkJ6c0FwZHhYUFEzM1k3MkMzWmlCN2o3QWZQNG83UTAvb21WWUh2NGdOSkl3SURBUUFCbzRJQjF6Q0NBZE13UHdZSUt3WUJCUVVIQVFFRU16QXhNQzhHQ0NzR0FRVUZCekFCaGlOb2RIUndPaTh2YjJOemNDNWhjSEJzWlM1amIyMHZiMk56Y0RBekxYZDNaSEl3TkRBZEJnTlZIUTRFRmdRVWthU2MvTVIydDUrZ2l2Uk45WTgyWGUwckJJVXdEQVlEVlIwVEFRSC9CQUl3QURBZkJnTlZIU01FR0RBV2dCU0lKeGNKcWJZWVlJdnM2N3IyUjFuRlVsU2p0ekNDQVI0R0ExVWRJQVNDQVJVd2dnRVJNSUlCRFFZS0tvWklodmRqWkFVR0FUQ0IvakNCd3dZSUt3WUJCUVVIQWdJd2diWU1nYk5TWld4cFlXNWpaU0J2YmlCMGFHbHpJR05sY25ScFptbGpZWFJsSUdKNUlHRnVlU0J3WVhKMGVTQmhjM04xYldWeklHRmpZMlZ3ZEdGdVkyVWdiMllnZEdobElIUm9aVzRnWVhCd2JHbGpZV0pzWlNCemRHRnVaR0Z5WkNCMFpYSnRjeUJoYm1RZ1kyOXVaR2wwYVc5dWN5QnZaaUIxYzJVc0lHTmxjblJwWm1sallYUmxJSEJ2YkdsamVTQmhibVFnWTJWeWRHbG1hV05oZEdsdmJpQndjbUZqZEdsalpTQnpkR0YwWlcxbGJuUnpMakEyQmdnckJnRUZCUWNDQVJZcWFIUjBjRG92TDNkM2R5NWhjSEJzWlM1amIyMHZZMlZ5ZEdsbWFXTmhkR1ZoZFhSb2IzSnBkSGt2TUE0R0ExVWREd0VCL3dRRUF3SUhnREFRQmdvcWhraUc5Mk5rQmdzQkJBSUZBREFOQmdrcWhraUc5dzBCQVFVRkFBT0NBUUVBRGFZYjB5NDk0MXNyQjI1Q2xtelQ2SXhETUlKZjRGelJqYjY5RDcwYS9DV1MyNHlGdzRCWjMrUGkxeTRGRkt3TjI3YTQvdncxTG56THJSZHJqbjhmNUhlNXNXZVZ0Qk5lcGhtR2R2aGFJSlhuWTR3UGMvem83Y1lmcnBuNFpVaGNvT0FvT3NBUU55MjVvQVE1SDNPNXlBWDk4dDUvR2lvcWJpc0IvS0FnWE5ucmZTZW1NL2oxbU9DK1JOdXhUR2Y4YmdwUHllSUdxTktYODZlT2ExR2lXb1IxWmRFV0JHTGp3Vi8xQ0tuUGFObVNBTW5CakxQNGpRQmt1bGhnd0h5dmozWEthYmxiS3RZZGFHNllRdlZNcHpjWm04dzdISG9aUS9PamJiOUlZQVlNTnBJcjdONFl0UkhhTFNQUWp2eWdhWndYRzU2QWV6bEhSVEJoTDhjVHFBPT0iOwoJInB1cmNoYXNlLWluZm8iID0gImV3b0pJbTl5YVdkcGJtRnNMWEIxY21Ob1lYTmxMV1JoZEdVdGNITjBJaUE5SUNJeU1ERTNMVEE1TFRBeElERTVPalU1T2pNeUlFRnRaWEpwWTJFdlRHOXpYMEZ1WjJWc1pYTWlPd29KSW5WdWFYRjFaUzFwWkdWdWRHbG1hV1Z5SWlBOUlDSTJPREZoT1dSbE1EYzVNek5oTURBek1UUXhZakppWldJd04yVmpPR1EzTVRCbU1ESTNPR0ZpSWpzS0NTSnZjbWxuYVc1aGJDMTBjbUZ1YzJGamRHbHZiaTFwWkNJZ1BTQWlNVEF3TURBd01ETXpNREV6TXpVM05DSTdDZ2tpWW5aeWN5SWdQU0FpTUNJN0Nna2lkSEpoYm5OaFkzUnBiMjR0YVdRaUlEMGdJakV3TURBd01EQXpNekF4TXpNMU56UWlPd29KSW5GMVlXNTBhWFI1SWlBOUlDSXhJanNLQ1NKdmNtbG5hVzVoYkMxd2RYSmphR0Z6WlMxa1lYUmxMVzF6SWlBOUlDSXhOVEEwTXpJeE1UY3lORGcwSWpzS0NTSjFibWx4ZFdVdGRtVnVaRzl5TFdsa1pXNTBhV1pwWlhJaUlEMGdJalEzUVRsQ05VTTFMVGd3UVRZdE5EY3hOeTA0TXpFNUxUWXdSVE0zTnpOQ1JFTTNPQ0k3Q2draWNISnZaSFZqZEMxcFpDSWdQU0FpWTI5dExtTnZkWEpoWjJVeU1ERTNMbkJ5YjNCZk5DSTdDZ2tpYVhSbGJTMXBaQ0lnUFNBaU1USTNPREkyTnpBM015STdDZ2tpWW1sa0lpQTlJQ0pqYjIwdVkyOTFjbUZuWlRJd01UY3ViWGwzZFhocFlTSTdDZ2tpY0hWeVkyaGhjMlV0WkdGMFpTMXRjeUlnUFNBaU1UVXdORE15TVRFM01qUTROQ0k3Q2draWNIVnlZMmhoYzJVdFpHRjBaU0lnUFNBaU1qQXhOeTB3T1Mwd01pQXdNam8xT1Rvek1pQkZkR012UjAxVUlqc0tDU0p3ZFhKamFHRnpaUzFrWVhSbExYQnpkQ0lnUFNBaU1qQXhOeTB3T1Mwd01TQXhPVG8xT1Rvek1pQkJiV1Z5YVdOaEwweHZjMTlCYm1kbGJHVnpJanNLQ1NKdmNtbG5hVzVoYkMxd2RYSmphR0Z6WlMxa1lYUmxJaUE5SUNJeU1ERTNMVEE1TFRBeUlEQXlPalU1T2pNeUlFVjBZeTlIVFZRaU93cDkiOwoJImVudmlyb25tZW50IiA9ICJTYW5kYm94IjsKCSJwb2QiID0gIjEwMCI7Cgkic2lnbmluZy1zdGF0dXMiID0gIjAiOwp9";
//        byte[] rec = System.Convert.FromBase64String(receipt);
//        string jsonStr = System.Text.Encoding.UTF8.GetString(rec);
//        JObject json = Statics.GetObjCJson(jsonStr);
//        Debug.Log(json["purchase-info"].ToString());
//        rec = System.Convert.FromBase64String(json["purchase-info"].ToString());
//        jsonStr = System.Text.Encoding.UTF8.GetString(rec);
//        json = Statics.GetObjCJson(jsonStr);
//        Debug.Log(json.ToString());
//        MaiHandler.Post(jsonStr.IndexOf("\"Sandbox\";") >= 0 ? "https://sandbox.itunes.apple.com/verifyReceipt" : "https://buy.itunes.apple.com/verifyReceipt", 
//            new JObject(new JProperty("receipt-data", receipt)), (text) =>
//        {
//            JObject json = JObject.Parse(text);
//            JObject receiptObj = JObject.Parse(json["receipt"].ToString());
//            int status = (int)json["status"];
//            if (status == 0)
//            {
//                Debug.Log(receiptObj["product_id"].ToString() + "," + receiptObj["bid"].ToString());
//            }
//        }, null);
	}

//    System.DateTime backDelayDate = System.DateTime.MinValue;
//    double backDelayTimeout = 12;
    public void OnApplicationPause(bool pause)
    {
        Debug.Log("OnApplicationPause: " + pause);
        if (pause) { //程序进入后台
            MaiHandler.StopSession();
//            backDelayDate = System.DateTime.Now;
        }
        else { //程序被唤醒
            MaiHandler.StartSession();
//            //大于等于5级每次切换回游戏都弹一次插屏广告
//            if (DbManager.Instance.HostData != null && DbManager.Instance.HostData.Lv >= 5 && (System.DateTime.Now - backDelayDate).TotalSeconds > backDelayTimeout) {
//                backDelayDate = System.DateTime.Now;
//                MaiHandler.ShowInterstitial(false);
//                MaiHandler.SendEvent("StartInterstitialForBack", DbManager.Instance.HostData.Lv.ToString());
//            }
        }
    }

	/// <summary>
	/// 处理场景加载完成回调
	/// </summary>
	/// <param name="level">Level.</param>
	void OnLevelWasLoaded(int level) {
		if (UICamera != null) {
			UICamera.GetComponent<Camera>().clearFlags = level > 0 ? CameraClearFlags.Depth : CameraClearFlags.SolidColor;
		}
		if (level > 1) {
			if (UserModel.CurrentUserData != null) {
				DbManager.Instance.CheckEnterArea(UserModel.CurrentUserData.CurrentAreaSceneName);
			}
		}
    }
}
