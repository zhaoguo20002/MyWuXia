using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReplayKitVideoStopResult : ISN_Result {

	private string[] _SavedSources =  new string[0];


	public ReplayKitVideoStopResult(string[] sourcesArray):base(true) {
		_SavedSources = sourcesArray;
	}

	public ReplayKitVideoStopResult(string errorData):base(errorData) {

	}


	public string[] SavedSources {
		get {
			return _SavedSources;
		}
	}
}
