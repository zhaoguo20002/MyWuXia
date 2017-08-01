////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IOSNative {


	//--------------------------------------
	// Constants
	//--------------------------------------

	public const char DATA_SPLITTER = '|';
	public const string DATA_SPLITTER2 = "|%|";
	public const string DATA_EOF = "endofline";


	public static string SerializeArray(string[] array) {

		if(array == null) {
			return string.Empty;
		} else {
			if(array.Length == 0) {
				return string.Empty;
			} else {

				string serializedArray = "";
				int len = array.Length;
				for(int i = 0; i < len; i++) {
					if(i != 0) {
						serializedArray += DATA_SPLITTER;
					}
					
					serializedArray += array[i];
				}

				return serializedArray;
			}
		}
	}

	public static string[] ParseArray(string arrayData) {

		List<string> ParsedArray =  new List<string>();
		string[] DataArray = arrayData.Split(IOSNative.DATA_SPLITTER); 

		for(int i = 0; i < DataArray.Length; i ++ ) {
			if(DataArray[i] == IOSNative.DATA_EOF) {
				break;
			}
			ParsedArray.Add(DataArray[i]);
		}

		return ParsedArray.ToArray();
	}
}
