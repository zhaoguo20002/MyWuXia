using UnityEngine;
using System.Collections;

public class ISN_LocalReceiptResult  {
	
	private byte[] _Receipt = null;

	public ISN_LocalReceiptResult(string data) {
		if(data.Length > 0) {
			_Receipt = System.Convert.FromBase64String(data);
		}
	}



	public byte[] Receipt {
		get {
			return _Receipt;
		}
	}
}
