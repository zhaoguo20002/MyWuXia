using UnityEngine;
using System.Collections.Generic;
using System;
using Game;
using Newtonsoft.Json.Linq;
using admob;
using System.Collections;

#if (UNITY_IOS && !UNITY_EDITOR)
using System.Runtime.InteropServices;
#endif
#if UNITY_ANDROID
using anysdk;
#endif

/// <summary>
/// 内购封装
/// </summary>
public class MaiHandler : MonoBehaviour {
    static bool isInitialized = false;
    const string price6 = "com.courage2017.yuan_6";
    const string price18 = "com.courage2017.yuan_18";
    const string prop1 = "com.courage2017.prop_1";
    const string prop3 = "com.courage2017.prop_3";
    const string prop4 = "com.courage2017.prop_4";
    const string prop5 = "com.courage2017.prop_5";
    const string worker10 = "com.courage2017.worker_10";
    static string _mai_ProductId = "";
    static string _mai_OrderId = "";
    static string _mai_Receipt = "";
    static Admob ad;
    static System.Action rewardedVideoCallback;
    static int reloadTimes;
    static bool showInterstitialLoading;
    static bool showRewardedVideoLoading;

    void Start() {
        init();
        initAdmob();
    }

    void init() {
        if (!isInitialized)
        {
            isInitialized = true;
            IOSInAppPurchaseManager.Instance.AddProductId(price6);
            IOSInAppPurchaseManager.Instance.AddProductId(price18);
            IOSInAppPurchaseManager.Instance.AddProductId(prop1);
            IOSInAppPurchaseManager.Instance.AddProductId(prop3);
            IOSInAppPurchaseManager.Instance.AddProductId(prop4);
            IOSInAppPurchaseManager.Instance.AddProductId(prop5);
            IOSInAppPurchaseManager.Instance.AddProductId(worker10);

            //Event Use Examples
            IOSInAppPurchaseManager.OnTransactionComplete += OnTransactionComplete;

            IOSInAppPurchaseManager.Instance.LoadStore();
            StartSession();
        }
    }

    void initAdmob()
    {
        ad = Admob.Instance();
        ad.rewardedVideoEventHandler += onRewardedVideoEvent;
        ad.interstitialEventHandler += onInterstitialEvent;
        //ca-app-pub-5547105749855252~7626858520
        ad.initAdmob("ca-app-pub-5547105749855252/4869968719", "ca-app-pub-5547105749855252/2148081380");//all id are admob test id,change those to your
        ad.setGender(AdmobGender.MALE);
        Debug.Log("admob inited -------------");

    }
    void onInterstitialEvent(string eventName, string msg)
    {
        LoadingBlockCtrl.Hide();
        Debug.Log("handler onAdmobEvent---" + eventName + "   " + msg);
        switch (eventName)
        {
            case "onAdFailedToLoad":
                if (showInterstitialLoading)
                {
                    if (reloadTimes-- > 0)
                    {
                        LoadingBlockCtrl.Show();
                        ad.loadInterstitial();
                    }
                    else
                    {
                        ConfirmCtrl.Show("没有找到可以播放的广告，是否继续看广告？", () => {
                            reloadTimes = 1;
                            LoadingBlockCtrl.Show();
                            ad.loadInterstitial();
                        }, null, "要看", "不看");
                    }
                }
                break;
            case "onAdLoaded":
                ad.showInterstitial();
                SendEvent("InterstitialLoaded", DbManager.Instance.HostData.Lv.ToString());
                break;
        }
    }
    void onRewardedVideoEvent(string eventName, string msg)
    {
        LoadingBlockCtrl.Hide();
        Debug.Log("handler onRewardedVideoEvent---" + eventName + "  rewarded: " + msg);
        switch (eventName)
        {
            case "onAdFailedToLoad":
                if (showRewardedVideoLoading)
                {
                    if (reloadTimes-- > 0)
                    {
                        LoadingBlockCtrl.Show();
                        ad.loadRewardedVideo("ca-app-pub-5547105749855252/2214749748");
                    }
                    else
                    {
                        ConfirmCtrl.Show("没有找到可以播放的广告，是否继续看广告？\n(Wifi不行可以切换4G试试)\n(也可以尝试断开重连下Wifi)", () => {
                            reloadTimes = 1;
                            LoadingBlockCtrl.Show();
                            ad.loadRewardedVideo("ca-app-pub-5547105749855252/2214749748");
                        }, null, "要看", "不看");
                    }
                }
                break;
            case "onAdLoaded":
                ad.showRewardedVideo();
                SendEvent("RewardedVideoLoaded", DbManager.Instance.HostData.Lv.ToString());
                break;
            case "onRewarded":
                if (rewardedVideoCallback != null)
                {
                    rewardedVideoCallback();
                    SendEvent("EndRewardedVideo", DbManager.Instance.HostData.Lv.ToString());
                }
                break;
        }
    }

    public static void StartRewardedVideo(System.Action callback, bool showLoading = true) {
        rewardedVideoCallback = callback;
        showRewardedVideoLoading = showLoading;
        if (ad.isRewardedVideoReady())
        {
            ad.showRewardedVideo();
        }
        else
        {
            reloadTimes = 1;
            if (showRewardedVideoLoading)
            {
                LoadingBlockCtrl.Show();
            }
            ad.loadRewardedVideo("ca-app-pub-5547105749855252/2214749748");
        }
    }

    public static void ShowInterstitial(bool showLoading = true) {
//        if (ad.isInterstitialReady())
//        {
//            ad.showInterstitial();
//        }
//        else
//        {
            showInterstitialLoading = showLoading;
            reloadTimes = 1;
            if (showInterstitialLoading)
            {
                LoadingBlockCtrl.Show();
            }
            ad.loadInterstitial();
//        }
    }

    /// <summary>
    /// 设置账户
    /// </summary>
    public static void SetAccount(RoleData role) {
        if (role == null)
        {
            Debug.Log("账户打点异常");
            return;
        }
        TDGAAccount account = TDGAAccount.SetAccount(TalkingDataGA.GetDeviceId() + "_" + role.Id);
        account.SetAccountName(role.Name);
        account.SetAccountType(AccountType.REGISTERED);
        account.SetLevel(role.Lv);
        account.SetAge(1);
        account.SetGender(role.Gender == GenderType.Male ? Gender.MALE : Gender.FEMALE);
        account.SetGameServer("1");
    }

    /// <summary>
    /// 用于跟踪用户使用中的打开应用和页面跳转的数据
    /// </summary>
    public static void StartSession() {
        TalkingDataGA.OnStart("792A360F7039485E8B3E7D48EC1677D9", "IOS");
    }

    /// <summary>
    /// 用于跟踪用户离开页面和退出应用的数据
    /// </summary>
    public static void StopSession() {
        TalkingDataGA.OnEnd();
    }

    /// <summary>
    /// 发送自定义事件
    /// </summary>
    /// <param name="eventType">Event type.</param>
    /// <param name="param0">Param0.</param>
    /// <param name="param1">Param1.</param>
    public static void SendEvent(string eventType, string param0, string param1 = "") {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("Param0", param0);
        dic.Add("Param1", param1);
        dic.Add("Date", DateTime.Now.ToString());
        TalkingDataGA.OnEvent(eventType, dic);
    }

    /// <summary>
    /// 检测是否有没有提交的充值成功请求
    /// </summary>
    public static void UnlockProducts(string proId) {
        if (!string.IsNullOrEmpty(proId))
        {
            string orderId;
            switch(proId) {
                case price6:
                    DbManager.Instance.GotSilver(50000);
                    Messenger.Broadcast<string>(NotifyTypes.GetStorePanelData, UserModel.CurrentUserData.CurrentCitySceneId);
                    AlertCtrl.Show("银子 +50000");
                    orderId = Statics.GetNowTimeStamp().ToString();
                    TDGAVirtualCurrency.OnChargeRequest(orderId, price6, 6, "CH", 6, "iap");
                    TDGAVirtualCurrency.OnChargeSuccess(orderId);
                    break;
                case price18:
                    DbManager.Instance.GotSilver(200000);
                    Messenger.Broadcast<string>(NotifyTypes.GetStorePanelData, UserModel.CurrentUserData.CurrentCitySceneId);
                    AlertCtrl.Show("银子 +200000");
                    orderId = Statics.GetNowTimeStamp().ToString();
                    TDGAVirtualCurrency.OnChargeRequest(orderId, price18, 18, "CH", 18, "iap");
                    TDGAVirtualCurrency.OnChargeSuccess(orderId);
                    break;
                case prop1:
                    PropItemContainer.SendRewards(PropType.NocturnalClothing, 10);
//                    IOSNativePopUpManager.showMessage("提示", "获得了10件夜行衣");
                    orderId = Statics.GetNowTimeStamp().ToString();
                    TDGAVirtualCurrency.OnChargeRequest(orderId, prop1, 1, "CH", 1, "iap");
                    TDGAVirtualCurrency.OnChargeSuccess(orderId);
                    break;
                case prop3:
                    PropItemContainer.SendRewards(PropType.Bodyguard, 10);
//                    IOSNativePopUpManager.showMessage("提示", "获得了10位镖师");
                    orderId = Statics.GetNowTimeStamp().ToString();
                    TDGAVirtualCurrency.OnChargeRequest(orderId, prop3, 1, "CH", 1, "iap");
                    TDGAVirtualCurrency.OnChargeSuccess(orderId);
                    break;
                case prop4:
                    PropItemContainer.SendRewards(PropType.LimePowder, 10);
//                    IOSNativePopUpManager.showMessage("提示", "获得了10包石灰粉");
                    orderId = Statics.GetNowTimeStamp().ToString();
                    TDGAVirtualCurrency.OnChargeRequest(orderId, prop4, 1, "CH", 1, "iap");
                    TDGAVirtualCurrency.OnChargeSuccess(orderId);
                    break;
                case prop5:
                    PropItemContainer.SendRewards(PropType.Scout, 10);
//                    IOSNativePopUpManager.showMessage("提示", "获得了10个探子");
                    orderId = Statics.GetNowTimeStamp().ToString();
                    TDGAVirtualCurrency.OnChargeRequest(orderId, prop5, 1, "CH", 1, "iap");
                    TDGAVirtualCurrency.OnChargeSuccess(orderId);
                    break;
                case worker10:
                    DbManager.Instance.SetPlusWorkerNum(DbManager.Instance.GetPlusWorkerNum() + 10);
                    DbManager.Instance.SetMaxWorkerNum(DbManager.Instance.GetMaxWorkerNum() + 10);
                    DbManager.Instance.SetWorkerNum(DbManager.Instance.GetWorkerNum() + 10);
                    WorkshopPanelCtrl.MakeWorkerNumChange(DbManager.Instance.GetWorkerNum(), DbManager.Instance.GetMaxWorkerNum());
                    AlertCtrl.Show("成功招募了10个家丁");
                    orderId = Statics.GetNowTimeStamp().ToString();
                    TDGAVirtualCurrency.OnChargeRequest(orderId, worker10, 3, "CH", 3, "iap");
                    TDGAVirtualCurrency.OnChargeSuccess(orderId);
                    break;
                default:
                    IOSNativePopUpManager.showMessage("提示", "不匹配的内购项目");
                    SendEvent("ProductIdError", DbManager.Instance.HostData.Lv.ToString(), DbManager.Instance.HostData.Name);
                    break;
            }
        }
    }

    static void clearIAPCache() {
        _mai_ProductId = "";
        _mai_OrderId = "";
        _mai_Receipt = "";
    }

    private static void OnTransactionComplete (IOSStoreKitResult result) {

//        Debug.Log("OnTransactionComplete: " + result.ProductIdentifier);
//        Debug.Log("OnTransactionComplete: state: " + result.State);
//        Debug.Log("-----------------------------start");
//        Debug.Log("OnTransactionComplete: Receipt: " + result.Receipt);
        switch(result.State) {
            case InAppPurchaseState.Purchased:
            case InAppPurchaseState.Restored:
                //Our product been succsesly purchased or restored
                //So we need to provide content to our user depends on productIdentifier
                _mai_ProductId = result.ProductIdentifier;
                _mai_Receipt = result.Receipt;
                PlayerPrefs.SetString("LatestReceipt", _mai_Receipt); //标记receipt，用于补单

                VerificationReceipt(_mai_Receipt);

//                Debug.Log("=========================, 新 OnTransactionComplete");
//                Debug.Log("=========================, 新 result.ProductIdentifier = " + result.ProductIdentifier);
//                Debug.Log("=========================, 新 result.State = " + result.State);
//                Debug.Log("=========================, 新 result.TransactionIdentifier = " + result.TransactionIdentifier);
//                Debug.Log("=========================, 新 result.Receipt = " + result.Receipt);
                break;
            case InAppPurchaseState.Deferred:
                //iOS 8 introduces Ask to Buy, which lets parents approve any purchases initiated by children
                //You should update your UI to reflect this deferred state, and expect another Transaction Complete  to be called again with a new transaction state 
                //reflecting the parent’s decision or after the transaction times out. Avoid blocking your UI or gameplay while waiting for the transaction to be updated.
                IOSNativePopUpManager.showMessage("提示", "充值超时");
                LoadingBlockCtrl.Hide();
                clearIAPCache();
                break;
            case InAppPurchaseState.Failed:
                //Our purchase flow is failed.
                //We can unlock intrefase and repor user that the purchase is failed. 
//                Debug.Log("Transaction failed with error, code: " + result.Error.Code);
//                Debug.Log("Transaction failed with error, description: " + result.Error.Description);
                IOSNativePopUpManager.showMessage("提示", "充值未完成");
                LoadingBlockCtrl.Hide();
                clearIAPCache();
                break;
        }
    }

    /// <summary>
    /// 支付
    /// </summary>
    public static void PayForProduct(string productId) {
        string ipaValue = string.Format(productId);
        PaymentManagerExample.buyItem(ipaValue);
        LoadingBlockCtrl.Show();
    }

    public static void VerificationReceipt(string receipt) {
        byte[] rec = Convert.FromBase64String(receipt);
        string jsonStr = System.Text.Encoding.UTF8.GetString(rec);
        Post(jsonStr.IndexOf("\"Sandbox\";") >= 0 ? "https://sandbox.itunes.apple.com/verifyReceipt" : "https://buy.itunes.apple.com/verifyReceipt", 
            new JObject(new JProperty("receipt-data", _mai_Receipt)), (text) =>
        {
            JObject json = JObject.Parse(text);
            int status = (int)json["status"];
            if (status == 0)
            {
                JObject receiptObj = JObject.Parse(json["receipt"].ToString());
                if (receiptObj["bid"].ToString() == "com.courage2017.mywuxia") {
                    if (string.IsNullOrEmpty(PlayerPrefs.GetString(receiptObj["original_transaction_id"].ToString()))) {
                        UnlockProducts(receiptObj["product_id"].ToString());
                        PlayerPrefs.SetString(receiptObj["original_transaction_id"].ToString(), "true");
                    }
                    else {
                        IOSNativePopUpManager.showMessage("提示", "已使用过的内购通知");
                        SendEvent("ReceiptUsed", PlayerPrefs.GetString(receiptObj["unique_identifier"].ToString()), DbManager.Instance.HostData.Name);
                    }
                }
                else {
                    IOSNativePopUpManager.showMessage("提示", "不属于本游戏的内购项目");
                    SendEvent("NotMyReceipt", receiptObj["bid"].ToString(), DbManager.Instance.HostData.Name);
                }
            }
            else {
                IOSNativePopUpManager.showMessage("提示", "付费服务器验证未通过");
                SendEvent("ReceiptError", status.ToString(), DbManager.Instance.HostData.Name);
            }
            PlayerPrefs.SetString("LatestReceipt", ""); //标记receipt为已经处理完毕
            LoadingBlockCtrl.Hide();
        }, null);
    }

    public static void CheckReceipt() {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("LatestReceipt")))
        {
            string receipt = PlayerPrefs.GetString("LatestReceipt").Replace("\r", "");
            byte[] rec = System.Convert.FromBase64String(receipt);
            string jsonStr = System.Text.Encoding.UTF8.GetString(rec);
            JObject json = Statics.GetObjCJson(jsonStr);
            rec = System.Convert.FromBase64String(json["purchase-info"].ToString());
            jsonStr = System.Text.Encoding.UTF8.GetString(rec);
            json = Statics.GetObjCJson(jsonStr);
//            Debug.Log("------------------------------resend");
//            Debug.Log(json.ToString());
            if (json["bid"].ToString() == "com.courage2017.mywuxia") {
                if (string.IsNullOrEmpty(PlayerPrefs.GetString(json["original-transaction-id"].ToString()))) {
                    UnlockProducts(json["product-id"].ToString());
                    PlayerPrefs.SetString(json["original-transaction-id"].ToString(), "true");
                }
                else {
                    IOSNativePopUpManager.showMessage("提示", "已使用过的内购通知");
                    SendEvent("ReceiptUsed", PlayerPrefs.GetString(json["original-transaction-id"].ToString()), DbManager.Instance.HostData.Name);
                }
            }
            else {
                IOSNativePopUpManager.showMessage("提示", "不属于本游戏的内购项目");
                SendEvent("NotMyReceipt", json["bid"].ToString(), DbManager.Instance.HostData.Name);
            }
            PlayerPrefs.SetString("LatestReceipt", ""); //标记receipt为已经处理完毕
            SendEvent("VerificationReceipt", DbManager.Instance.HostData.Lv.ToString(), DbManager.Instance.HostData.Name);
        }
    }

    static bool myRemoteCertificateValidationCallback(System.Object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) {
        bool isOk = true;
        // If there are errors in the certificate chain, look at each error to determine the cause.
        if (sslPolicyErrors != System.Net.Security.SslPolicyErrors.None) {
            for (int i=0; i<chain.ChainStatus.Length; i++) {
                if (chain.ChainStatus [i].Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.RevocationStatusUnknown) {
                    chain.ChainPolicy.RevocationFlag = System.Security.Cryptography.X509Certificates.X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan (0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = System.Security.Cryptography.X509Certificates.X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build ((System.Security.Cryptography.X509Certificates.X509Certificate2)certificate);
                    if (!chainIsValid) {
                        isOk = false;
                    }
                }
            }
        }
        return isOk;
    }

    /// <summary>
    /// 发送Http请求
    /// </summary>
    /// <param name="url">URL.</param>
    /// <param name="param">Parameter.</param>
    /// <param name="callback">Callback.</param>
    /// <param name="errorCallback">Error callback.</param>
    public static void Post(string url, JObject param, System.Action<string> callback = null, System.Action errorCallback = null) {
        try
        {
            // var json = "{ 'receipt-data': '" + receiptData + "'}";

            var json = param.ToString();
            #if UNITY_EDITOR
            Debug.LogWarning("Http 上行: url = " + url + ", " + JsonManager.GetInstance().SerializeObject(json));
            #endif
            System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
            byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(json);
            System.Net.ServicePointManager.ServerCertificateValidationCallback = myRemoteCertificateValidationCallback;
            //  HttpWebRequest request;
            var request = System.Net.HttpWebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = postBytes.Length;

            //Stream postStream = request.GetRequestStream();
            //postStream.Write(postBytes, 0, postBytes.Length);
            //postStream.Close();

            using (var stream = request.GetRequestStream())
            {
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Flush();
            }

            //  var sendresponse = (HttpWebResponse)request.GetResponse();

            var sendresponse = request.GetResponse();

            string sendresponsetext = "";
            using (var streamReader = new System.IO.StreamReader(sendresponse.GetResponseStream()))
            {
                sendresponsetext = streamReader.ReadToEnd().Trim();
                streamReader.Close();
                streamReader.Dispose();
            }
            #if UNITY_EDITOR
            Debug.LogWarning("Http 下行: url = " + url + ", " + sendresponsetext);
            #endif
            if (callback != null) {
                callback(sendresponsetext);
            }

        }
        catch (System.Exception ex)
        {
            LoadingBlockCtrl.Hide();
            AlertCtrl.Show(ex.Message.ToString());
            if (errorCallback != null) {
                errorCallback();
            }
            SendEvent("HttpErro", ex.Message.ToString(), DbManager.Instance.HostData.Name);
        }
    }
}
