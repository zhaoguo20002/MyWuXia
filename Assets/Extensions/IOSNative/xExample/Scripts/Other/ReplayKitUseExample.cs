////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReplayKitUseExample : BaseIOSFeaturePreview {

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {


		ISN_ReplayKit.ActionRecordStarted += HandleActionRecordStarted;
		ISN_ReplayKit.ActionRecordStoped += HandleActionRecordStoped;
		ISN_ReplayKit.ActionRecordInterrupted += HandleActionRecordInterrupted;

		IOSNativePopUpManager.showMessage ("Welcome", "Hey there, welcome to the ReplayKit testing scene!");
	}

	void OnDestroy() {
		ISN_ReplayKit.ActionRecordStarted -= HandleActionRecordStarted;
		ISN_ReplayKit.ActionRecordStoped -= HandleActionRecordStoped;
		ISN_ReplayKit.ActionRecordInterrupted -= HandleActionRecordInterrupted;
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	void OnGUI() {
		
		UpdateToStartPos();
		
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Replay Kit", style);
		
		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Start Recording")) {
			ISN_ReplayKit.Instance.StartRecording();
		}
		
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Stop Recording")) {
			ISN_ReplayKit.Instance.StopRecording();
		}

		
	}

	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	void HandleActionRecordInterrupted (ISN_Error error) {
		IOSNativePopUpManager.showMessage ("Video was interrupted with error: "," " + error.Description);
	}
	
	void HandleActionRecordStoped (ReplayKitVideoStopResult res) {

		if(res.IsFailed) {
			IOSNativePopUpManager.showMessage ("Fail", "Error: " + res.Error.Description);
			return;
		}

		if(res.SavedSources.Length > 0) {
			foreach(string source in res.SavedSources) {
				IOSNativePopUpManager.showMessage ("Success", "User has shared the video to" + source);
			}
		} else {
			IOSNativePopUpManager.showMessage ("Fail", "User declined video sharing!");
		}

	}
	

	
	void HandleActionRecordStarted (ISN_Result res) {
		if(res.IsSucceeded) {
			IOSNativePopUpManager.showMessage ("Success", "Record was successfully started!");

		} else {
			Debug.Log("Record start failed: " + res.Error.Description);
			IOSNativePopUpManager.showMessage ("Fail","Error: " + res.Error.Description);
		}
		ISN_ReplayKit.ActionRecordStarted -= HandleActionRecordStarted;
	}

	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

	

}
