using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IOSNativeFeaturesPreview : BaseIOSFeaturePreview {


	public static IOSNativePreviewBackButton back = null;



	void Awake() {
		if(back == null) {
			back = IOSNativePreviewBackButton.Create();
		}
	}


	void OnGUI() {
		
		UpdateToStartPos();
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Game Center Examples", style);
		
		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Basic Features")) {
			Application.LoadLevel("GameCenterGeneral");
		}
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Friends Load Example")) {
			Application.LoadLevel("FriendsLoadExample");
		}


		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Custom Leaderboard GUI")) {
			Application.LoadLevel("CustomLeaderboardGUIExample");
		}


		StartX = XStartPos;
		StartY += YLableStep;
		StartY += YLableStep;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Main Features", style);


		StartX = XStartPos;
		StartY += YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Billing")) {
			Application.LoadLevel("BillingExample");
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "iAd App Network")) {
			Application.LoadLevel("iAdExample");
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "iAd No Coding Example")) {
			Application.LoadLevel("iAdNoCodingExample");
		}

		StartX = XStartPos;
		StartY += YButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "iCloud")) {
			Application.LoadLevel("iCloudExampleScene");
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Social Posting")) {
			Application.LoadLevel("SocialPostingExample");
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Local And Push Notifications")) {
			Application.LoadLevel("NotificationExample");

		}

		StartX = XStartPos;
		StartY += YButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "ReplayKit")) {
			Application.LoadLevel("ReplayKitExampleScene");
		}


		StartX = XStartPos;
		StartY += YLableStep;
		StartY += YLableStep;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Networking", style);
		
		
		StartX = XStartPos;
		StartY += YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "TBM Multiplayer Example")) {
			Application.LoadLevel("TMB_Multiplayer_Example");
		}


		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "RTM Multiplayer Example")) {
			Application.LoadLevel("RTM_Multiplayer_Example");
		}

		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "P2P Game Example")) {
			Application.LoadLevel("Peer-To-PeerGameExample");
		}


		StartX = XStartPos;
		StartY += YLableStep;
		StartY += YLableStep;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Additional Features Features", style);

		StartX = XStartPos;
		StartY += YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Native Popups and Events")) {
			Application.LoadLevel("PopUpsAndAppEventsHandler");
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Media Player API")) {
			Application.LoadLevel("MediaExample");
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "IOS Native Actions")) {
			Application.LoadLevel("NativeIOSActionsExample");
		}
	}

}
