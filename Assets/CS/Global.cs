using UnityEngine;
using System.Collections;
using Game;
using Newtonsoft.Json.Linq;

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

//        string receipt = "ewoJInNpZ25hdHVyZSIgPSAiQXd2Umh6NnhSYlI4azdwbFJ3ek9ZYlFhZlVEYlEw\r\r\nM3cyTU44TWNJZjhWTHhPNXo5RGxGYzhZSUs4VEsxTXZaSU1WUnE1aVU2NGZFT1Vw\r\r\nMEgwaC9mcERFc3MxZHlpNXpua0tKMy9mREkvWkt0VXhhTU00TjAzYkhXbSsxeE5i\r\r\nWHZGeXBjajBXVUtzblZ2L1N5WndHZjFRZFJZbUREYkc5Y0lvTnNZejVFRVhTd1Bo\r\r\nSzVJaDV1NWVTY0tVNVRjWnd2azk3QXR6NE5BcXRpK0E1S05IamNOZitCeTNvRXJl\r\r\nRDYwMDgvS3Zqb045cHRQQndHTHcyWEFKc1U1UnJPdnJhMGRBMUN2TUVpOUNzR2Y4\r\r\nNXU0LzRxdkcwN2doQ29yRkxEcDRMbDIzdFVMZzAxUGJQc1dJQjNXRmVtNzR3aGVS\r\r\nUzdrb2c0WWRWWFF4REF6Z3o0TDAvSjJjd0FBQVdBTUlJRmZEQ0NCR1NnQXdJQkFn\r\r\nSUlEdXRYaCtlZUNZMHdEUVlKS29aSWh2Y05BUUVGQlFBd2daWXhDekFKQmdOVkJB\r\r\nWVRBbFZUTVJNd0VRWURWUVFLREFwQmNIQnNaU0JKYm1NdU1Td3dLZ1lEVlFRTERD\r\r\nTkJjSEJzWlNCWGIzSnNaSGRwWkdVZ1JHVjJaV3h2Y0dWeUlGSmxiR0YwYVc5dWN6\r\r\nRkVNRUlHQTFVRUF3dzdRWEJ3YkdVZ1YyOXliR1IzYVdSbElFUmxkbVZzYjNCbGNp\r\r\nQlNaV3hoZEdsdmJuTWdRMlZ5ZEdsbWFXTmhkR2x2YmlCQmRYUm9iM0pwZEhrd0ho\r\r\nY05NVFV4TVRFek1ESXhOVEE1V2hjTk1qTXdNakEzTWpFME9EUTNXakNCaVRFM01E\r\r\nVUdBMVVFQXd3dVRXRmpJRUZ3Y0NCVGRHOXlaU0JoYm1RZ2FWUjFibVZ6SUZOMGIz\r\r\nSmxJRkpsWTJWcGNIUWdVMmxuYm1sdVp6RXNNQ29HQTFVRUN3d2pRWEJ3YkdVZ1Yy\r\r\nOXliR1IzYVdSbElFUmxkbVZzYjNCbGNpQlNaV3hoZEdsdmJuTXhFekFSQmdOVkJB\r\r\nb01Da0Z3Y0d4bElFbHVZeTR4Q3pBSkJnTlZCQVlUQWxWVE1JSUJJakFOQmdrcWhr\r\r\naUc5dzBCQVFFRkFBT0NBUThBTUlJQkNnS0NBUUVBcGMrQi9TV2lnVnZXaCswajJq\r\r\nTWNqdUlqd0tYRUpzczl4cC9zU2cxVmh2K2tBdGVYeWpsVWJYMS9zbFFZbmNRc1Vu\r\r\nR09aSHVDem9tNlNkWUk1YlNJY2M4L1cwWXV4c1FkdUFPcFdLSUVQaUY0MWR1MzBJ\r\r\nNFNqWU5NV3lwb041UEM4cjBleE5LaERFcFlVcXNTNCszZEg1Z1ZrRFV0d3N3U3lv\r\r\nMUlnZmRZZUZScjZJd3hOaDlLQmd4SFZQTTNrTGl5a29sOVg2U0ZTdUhBbk9DNnBM\r\r\ndUNsMlAwSzVQQi9UNXZ5c0gxUEttUFVockFKUXAyRHQ3K21mNy93bXYxVzE2c2Mx\r\r\nRkpDRmFKekVPUXpJNkJBdENnbDdaY3NhRnBhWWVRRUdnbUpqbTRIUkJ6c0FwZHhY\r\r\nUFEzM1k3MkMzWmlCN2o3QWZQNG83UTAvb21WWUh2NGdOSkl3SURBUUFCbzRJQjF6\r\r\nQ0NBZE13UHdZSUt3WUJCUVVIQVFFRU16QXhNQzhHQ0NzR0FRVUZCekFCaGlOb2RI\r\r\nUndPaTh2YjJOemNDNWhjSEJzWlM1amIyMHZiMk56Y0RBekxYZDNaSEl3TkRBZEJn\r\r\nTlZIUTRFRmdRVWthU2MvTVIydDUrZ2l2Uk45WTgyWGUwckJJVXdEQVlEVlIwVEFR\r\r\nSC9CQUl3QURBZkJnTlZIU01FR0RBV2dCU0lKeGNKcWJZWVlJdnM2N3IyUjFuRlVs\r\r\nU2p0ekNDQVI0R0ExVWRJQVNDQVJVd2dnRVJNSUlCRFFZS0tvWklodmRqWkFVR0FU\r\r\nQ0IvakNCd3dZSUt3WUJCUVVIQWdJd2diWU1nYk5TWld4cFlXNWpaU0J2YmlCMGFH\r\r\nbHpJR05sY25ScFptbGpZWFJsSUdKNUlHRnVlU0J3WVhKMGVTQmhjM04xYldWeklH\r\r\nRmpZMlZ3ZEdGdVkyVWdiMllnZEdobElIUm9aVzRnWVhCd2JHbGpZV0pzWlNCemRH\r\r\nRnVaR0Z5WkNCMFpYSnRjeUJoYm1RZ1kyOXVaR2wwYVc5dWN5QnZaaUIxYzJVc0lH\r\r\nTmxjblJwWm1sallYUmxJSEJ2YkdsamVTQmhibVFnWTJWeWRHbG1hV05oZEdsdmJp\r\r\nQndjbUZqZEdsalpTQnpkR0YwWlcxbGJuUnpMakEyQmdnckJnRUZCUWNDQVJZcWFI\r\r\nUjBjRG92TDNkM2R5NWhjSEJzWlM1amIyMHZZMlZ5ZEdsbWFXTmhkR1ZoZFhSb2Iz\r\r\nSnBkSGt2TUE0R0ExVWREd0VCL3dRRUF3SUhnREFRQmdvcWhraUc5Mk5rQmdzQkJB\r\r\nSUZBREFOQmdrcWhraUc5dzBCQVFVRkFBT0NBUUVBRGFZYjB5NDk0MXNyQjI1Q2xt\r\r\nelQ2SXhETUlKZjRGelJqYjY5RDcwYS9DV1MyNHlGdzRCWjMrUGkxeTRGRkt3TjI3\r\r\nYTQvdncxTG56THJSZHJqbjhmNUhlNXNXZVZ0Qk5lcGhtR2R2aGFJSlhuWTR3UGMv\r\r\nem83Y1lmcnBuNFpVaGNvT0FvT3NBUU55MjVvQVE1SDNPNXlBWDk4dDUvR2lvcWJp\r\r\nc0IvS0FnWE5ucmZTZW1NL2oxbU9DK1JOdXhUR2Y4YmdwUHllSUdxTktYODZlT2Ex\r\r\nR2lXb1IxWmRFV0JHTGp3Vi8xQ0tuUGFObVNBTW5CakxQNGpRQmt1bGhnd0h5dmoz\r\r\nWEthYmxiS3RZZGFHNllRdlZNcHpjWm04dzdISG9aUS9PamJiOUlZQVlNTnBJcjdO\r\r\nNFl0UkhhTFNQUWp2eWdhWndYRzU2QWV6bEhSVEJoTDhjVHFBPT0iOwoJInB1cmNo\r\r\nYXNlLWluZm8iID0gImV3b0pJbTl5YVdkcGJtRnNMWEIxY21Ob1lYTmxMV1JoZEdV\r\r\ndGNITjBJaUE5SUNJeU1ERTNMVEE1TFRBeElERTVPalU1T2pNeUlFRnRaWEpwWTJF\r\r\ndlRHOXpYMEZ1WjJWc1pYTWlPd29KSW5WdWFYRjFaUzFwWkdWdWRHbG1hV1Z5SWlB\r\r\nOUlDSTJPREZoT1dSbE1EYzVNek5oTURBek1UUXhZakppWldJd04yVmpPR1EzTVRC\r\r\nbU1ESTNPR0ZpSWpzS0NTSnZjbWxuYVc1aGJDMTBjbUZ1YzJGamRHbHZiaTFwWkNJ\r\r\nZ1BTQWlNVEF3TURBd01ETXpNREV6TXpVM05DSTdDZ2tpWW5aeWN5SWdQU0FpTUNJ\r\r\nN0Nna2lkSEpoYm5OaFkzUnBiMjR0YVdRaUlEMGdJakV3TURBd01EQXpNekF4TXpN\r\r\nMU56UWlPd29KSW5GMVlXNTBhWFI1SWlBOUlDSXhJanNLQ1NKdmNtbG5hVzVoYkMx\r\r\nd2RYSmphR0Z6WlMxa1lYUmxMVzF6SWlBOUlDSXhOVEEwTXpJeE1UY3lORGcwSWpz\r\r\nS0NTSjFibWx4ZFdVdGRtVnVaRzl5TFdsa1pXNTBhV1pwWlhJaUlEMGdJalEzUVRs\r\r\nQ05VTTFMVGd3UVRZdE5EY3hOeTA0TXpFNUxUWXdSVE0zTnpOQ1JFTTNPQ0k3Q2dr\r\r\naWNISnZaSFZqZEMxcFpDSWdQU0FpWTI5dExtTnZkWEpoWjJVeU1ERTNMbkJ5YjNC\r\r\nZk5DSTdDZ2tpYVhSbGJTMXBaQ0lnUFNBaU1USTNPREkyTnpBM015STdDZ2tpWW1s\r\r\na0lpQTlJQ0pqYjIwdVkyOTFjbUZuWlRJd01UY3ViWGwzZFhocFlTSTdDZ2tpY0hW\r\r\neVkyaGhjMlV0WkdGMFpTMXRjeUlnUFNBaU1UVXdORE15TVRFM01qUTROQ0k3Q2dr\r\r\naWNIVnlZMmhoYzJVdFpHRjBaU0lnUFNBaU1qQXhOeTB3T1Mwd01pQXdNam8xT1Rv\r\r\nek1pQkZkR012UjAxVUlqc0tDU0p3ZFhKamFHRnpaUzFrWVhSbExYQnpkQ0lnUFNB\r\r\naU1qQXhOeTB3T1Mwd01TQXhPVG8xT1Rvek1pQkJiV1Z5YVdOaEwweHZjMTlCYm1k\r\r\nbGJHVnpJanNLQ1NKdmNtbG5hVzVoYkMxd2RYSmphR0Z6WlMxa1lYUmxJaUE5SUNJ\r\r\neU1ERTNMVEE1TFRBeUlEQXlPalU1T2pNeUlFVjBZeTlIVFZRaU93cDkiOwoJImVu\r\r\ndmlyb25tZW50IiA9ICJTYW5kYm94IjsKCSJwb2QiID0gIjEwMCI7Cgkic2lnbmlu\r\r\nZy1zdGF0dXMiID0gIjAiOwp9";
//        receipt = receipt.Replace("\r", "");
//        receipt = receipt.Replace("\n", "");
//        byte[] rec = System.Convert.FromBase64String(receipt);
//        string jsonStr = System.Text.Encoding.UTF8.GetString(rec);
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
