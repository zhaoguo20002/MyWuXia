using UnityEngine;
using System.Collections.Generic;
using System;
using Game;
using Newtonsoft.Json.Linq;

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
    const string price6 = "com.nookjoy.haiyao_yuan6";
    static string _mai_ProductId = "";
    static string _mai_OrderId = "";
    static string _mai_Receipt = "";

    void Start() {
        init();
    }

    void init() {
        if (!isInitialized)
        {
            isInitialized = true;
            IOSInAppPurchaseManager.Instance.AddProductId(price6);

            //Event Use Examples
            IOSInAppPurchaseManager.OnTransactionComplete += OnTransactionComplete;

            IOSInAppPurchaseManager.Instance.LoadStore();
            StartSession();
        }
    }
    /// <summary>
    /// 设置账户
    /// </summary>
    public static void SetAccount(RoleData role) {
        TDGAAccount account = TDGAAccount.SetAccount(role.Id);
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
    /// 检测是否有没有提交的充值成功请求
    /// </summary>
    public static void UnlockProducts() {
        if (!string.IsNullOrEmpty(_mai_ProductId))
        {
            switch(_mai_ProductId) {
                case price6:
                    
                    break;
                default:
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

        switch(result.State) {
            case InAppPurchaseState.Purchased:
            case InAppPurchaseState.Restored:
                //Our product been succsesly purchased or restored
                //So we need to provide content to our user depends on productIdentifier
                _mai_ProductId = result.ProductIdentifier;
                _mai_Receipt = result.Receipt;
                LoadingBlockCtrl.Hide();
                UnlockProducts();

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
    public static void PayForProduct(int money) {
        string ipaValue = string.Format("com.nookjoy.haiyao_yuan{0}", money);
        PaymentManagerExample.buyItem(ipaValue);
        LoadingBlockCtrl.Show();
    }
}
