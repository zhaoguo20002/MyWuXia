////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class IOSInAppPurchaseManager : ISN_Singleton<IOSInAppPurchaseManager> {


	public const string APPLE_VERIFICATION_SERVER   = "https://buy.itunes.apple.com/verifyReceipt";
	public const string SANDBOX_VERIFICATION_SERVER = "https://sandbox.itunes.apple.com/verifyReceipt";
	

	//Actions
	public static event Action<IOSStoreKitResult> OnTransactionComplete = delegate{};
	public static event Action<IOSStoreKitRestoreResult> OnRestoreComplete = delegate{};
	public static event Action<ISN_Result> OnStoreKitInitComplete = delegate{};
	public static event Action<bool> OnPurchasesStateSettingsLoaded = delegate{};
	public static event Action<IOSStoreKitVerificationResponse> OnVerificationComplete = delegate{};

	
	private bool _IsStoreLoaded = false;
	private bool _IsWaitingLoadResult = false;
	private bool _IsInAppPurchasesEnabled = true;
	private static int _nextId = 1;
	
	private List<string> _productsIds =  new List<string>();
	private List<IOSProductTemplate> _products    =  new List<IOSProductTemplate>();
	private Dictionary<int, IOSStoreProductView> _productsView =  new Dictionary<int, IOSStoreProductView>(); 

	private static string lastPurchasedProduct;
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}


	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------



	[System.Obsolete("loadStore is deprecated, please use LoadStore instead.")]
	public void loadStore() {
		LoadStore();
	}

	public void LoadStore() {

		if(_IsStoreLoaded) {
			Invoke("FireSuccessInitEvent", 1f);
			return;
		}

		if(_IsWaitingLoadResult) {
			return;
		}

		_IsWaitingLoadResult = true;
		
		foreach(string pid in IOSNativeSettings.Instance.InAppProducts) {
			AddProductId(pid);
		}
		
		string ids = "";
		int len = _productsIds.Count;
		for(int i = 0; i < len; i++) {
			if(i != 0) {
				ids += ",";
			}
			
			ids += _productsIds[i];
		}


		#if !UNITY_EDITOR
		IOSNativeMarketBridge.loadStore(ids);
		#else
		if(IOSNativeSettings.Instance.SendFakeEventsInEditor) {
			Invoke("EditorFakeInitEvent", 1f);
		}
		#endif
		
	}

	public void RequestInAppSettingState() {
		IOSNativeMarketBridge.ISN_RequestInAppSettingState();
	}



	[System.Obsolete("buyProduct is deprecated, please use BuyProduct instead.")]
	public void buyProduct(string productId) {
		BuyProduct(productId);
	}

	public void BuyProduct(string productId) {

		#if !UNITY_EDITOR

		if(!_IsStoreLoaded) {

			if(!IOSNativeSettings.Instance.DisablePluginLogs) 
				Debug.LogWarning("buyProduct shouldn't be called before StoreKit is initialized"); 
			SendTransactionFailEvent(productId, "StoreKit not yet initialized", IOSTransactionErrorCode.SKErrorPaymentNotAllowed);

			return;
		} 

		IOSNativeMarketBridge.buyProduct(productId);

		#else
		if(IOSNativeSettings.Instance.SendFakeEventsInEditor) {
			FireProductBoughtEvent(productId, "", "", "", false);
		}
		#endif
	}


	[System.Obsolete("addProductId is deprecated, please use AddProductId instead.")]
	public void addProductId(string productId) {
		AddProductId(productId);
	}
	
	public void AddProductId(string productId) {
		if(_productsIds.Contains(productId)) {
			return;
		}

		_productsIds.Add(productId);
	}

	public IOSProductTemplate GetProductById(string id) {
		foreach(IOSProductTemplate tpl in Products) {
			if(tpl.id.Equals(id)) {
				return tpl;
			}
		}

		return null;
	}


	[System.Obsolete("restorePurchases is deprecated, please use RestorePurchases instead.")]
	public void restorePurchases() {
		RestorePurchases();
	}

	public void RestorePurchases() {

		if(!_IsStoreLoaded) {

			ISN_Error e = new ISN_Error((int) IOSTransactionErrorCode.SKErrorPaymentServiceNotInitialized, "Store Kit Initilizations required"); 

			IOSStoreKitRestoreResult r =  new IOSStoreKitRestoreResult(e);
			OnRestoreComplete(r);
			return;
		}

		#if !UNITY_EDITOR
		IOSNativeMarketBridge.restorePurchases();
		#else
		if(IOSNativeSettings.Instance.SendFakeEventsInEditor) {
			foreach(string productId in _productsIds) {
				Debug.Log("Restored: " + productId);
				FireProductBoughtEvent(productId, "", "", "", true);
			}
			FireRestoreCompleteEvent();
		}
		#endif
	}


	[System.Obsolete("verifyLastPurchase is deprecated, please use VerifyLastPurchase instead.")]
	public void verifyLastPurchase(string url) {
		VerifyLastPurchase(url);
	}

	public void VerifyLastPurchase(string url) {
		IOSNativeMarketBridge.verifyLastPurchase (url);
	}

	public void RegisterProductView(IOSStoreProductView view) {
		view.SetId(NextId);
		_productsView.Add(view.id, view);
	}
	
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------

	[System.Obsolete("products is deprecated, please use Products instead.")]
	public List<IOSProductTemplate> products {
		get {
			return Products;
		}
	}

	public List<IOSProductTemplate> Products {
		get {
			return _products;
		}
	}

	public bool IsStoreLoaded {
		get {
			return _IsStoreLoaded;
		}
	}

	public bool IsInAppPurchasesEnabled {
		get {
			return _IsInAppPurchasesEnabled;
		}
	}

	public bool IsWaitingLoadResult {
		get {
			return _IsWaitingLoadResult;
		}
	}

	private static int NextId {
		get {
			_nextId++;
			return _nextId;
		}
	}

	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	private void onStoreKitStart(string data) {
		int satus = System.Convert.ToInt32(data);
		if(satus == 1) {
			_IsInAppPurchasesEnabled = true;
		} else {
			_IsInAppPurchasesEnabled = false;
		}

		OnPurchasesStateSettingsLoaded(_IsInAppPurchasesEnabled);
	}

	private void OnStoreKitInitFailed(string data) {

		ISN_Error e =  new ISN_Error(data);

		_IsStoreLoaded = false;
		_IsWaitingLoadResult = false;


		ISN_Result res = new ISN_Result (false);
		res.SetError(e);
		OnStoreKitInitComplete (res);


		if(!IOSNativeSettings.Instance.DisablePluginLogs) 
			Debug.Log("STORE_KIT_INIT_FAILED Error: " + e.Description);
	}
	
	private void onStoreDataReceived(string data) {
		if(data.Equals(string.Empty)) {
			Debug.Log("InAppPurchaseManager, no products avaiable: " + _products.Count.ToString());
			ISN_Result res = new ISN_Result(true);
			OnStoreKitInitComplete(res);
			return;
		}


		string[] storeData;
		storeData = data.Split("|" [0]);
		
		for(int i = 0; i < storeData.Length; i+=7) {
			IOSProductTemplate tpl =  new IOSProductTemplate();
			tpl.id 				= storeData[i];
			tpl.title 			= storeData[i + 1];
			tpl.description 	= storeData[i + 2];
			tpl.localizedPrice 	= storeData[i + 3];
			tpl.price 			= storeData[i + 4];
			tpl.currencyCode 	= storeData[i + 5];
			tpl.currencySymbol 	= storeData[i + 6];
			_products.Add(tpl);
		}
		
		Debug.Log("InAppPurchaseManager, total products loaded: " + _products.Count.ToString());
		FireSuccessInitEvent();
	}
	
	private void onProductBought(string array) {

		string[] data;
		data = array.Split("|" [0]);

		bool IsRestored = false;
		if(data [1].Equals("0")) {
			IsRestored = true;
		}

		FireProductBoughtEvent(data [0], data [2], data [3], data [4], IsRestored);

	}

	private void onProductStateDeferred(string productIdentifier) {
		IOSStoreKitResult response = new IOSStoreKitResult (productIdentifier, InAppPurchaseState.Deferred);


		OnTransactionComplete (response);
	}

	
	private void onTransactionFailed(string array) {

		string[] data;
		data = array.Split("|" [0]);

		SendTransactionFailEvent(data [0], data [1], (IOSTransactionErrorCode) System.Convert.ToInt32( data [2]));
	}
	
	
	private void onVerificationResult(string array) {

		string[] data;
		data = array.Split("|" [0]);

		IOSStoreKitVerificationResponse response = new IOSStoreKitVerificationResponse ();
		response.status = System.Convert.ToInt32(data[0]);
		response.originalJSON = data [1];
		response.receipt = data [2];
		response.productIdentifier = lastPurchasedProduct;

		OnVerificationComplete (response);

	}

	public void onRestoreTransactionFailed(string array) {

		ISN_Error e = new ISN_Error(array);

		IOSStoreKitRestoreResult r =  new IOSStoreKitRestoreResult(e);


		OnRestoreComplete (r);
	}

	public void onRestoreTransactionComplete(string array) {
		FireRestoreCompleteEvent();
	}



	private void OnProductViewLoaded(string viewId) {
		int id = System.Convert.ToInt32(viewId);
		if(_productsView.ContainsKey(id)) {
			_productsView[id].OnContentLoaded();
		}
	}

	private void OnProductViewLoadedFailed(string viewId) {
		int id = System.Convert.ToInt32(viewId);
		if(_productsView.ContainsKey(id)) {
			_productsView[id].OnContentLoadFailed();
		}
	}

	private void OnProductViewDismissed(string viewId) {
		int id = System.Convert.ToInt32(viewId);
		if(_productsView.ContainsKey(id)) {
			_productsView[id].OnProductViewDismissed();
		}
	}

	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------

	private void FireSuccessInitEvent() {
		_IsStoreLoaded = true;
		_IsWaitingLoadResult = false;
		ISN_Result r = new ISN_Result(true);
		OnStoreKitInitComplete(r);
	}


	private void FireRestoreCompleteEvent() {

		IOSStoreKitRestoreResult r =  new IOSStoreKitRestoreResult(true);

		OnRestoreComplete (r);
	}

	private void FireProductBoughtEvent(string productIdentifier, string applicationUsername, string receipt, string transactionIdentifier, bool IsRestored) {

		InAppPurchaseState state;
		if(IsRestored) {
			state = InAppPurchaseState.Restored;
		} else {
			state = InAppPurchaseState.Purchased;
		}

		IOSStoreKitResult response = new IOSStoreKitResult (productIdentifier, state, applicationUsername, receipt, transactionIdentifier);

	
		
		lastPurchasedProduct = response.ProductIdentifier;
		OnTransactionComplete (response);
	}


	private void SendTransactionFailEvent(string productIdentifier, string errorDescribtion, IOSTransactionErrorCode errorCode) {
		IOSStoreKitResult response = new IOSStoreKitResult (productIdentifier, new ISN_Error((int) errorCode, errorDescribtion));

		OnTransactionComplete (response);
	}

	//--------------------------------------
	//  UNITY EDITOR FAKE SECTION
	//--------------------------------------

	private void EditorFakeInitEvent() {
		foreach(string id in _productsIds) {

			IOSProductTemplate tpl =  new IOSProductTemplate();
			tpl.id 				= id;
			tpl.title 			= "Title for " + id;
			tpl.description 	= "Description for " + id;
			tpl.localizedPrice 	= "1 $";
			tpl.price 			= "1";
			tpl.currencyCode 	= "USD";
			tpl.currencySymbol 	= "$";
			_products.Add(tpl);

		}

		FireSuccessInitEvent();
	}


	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
