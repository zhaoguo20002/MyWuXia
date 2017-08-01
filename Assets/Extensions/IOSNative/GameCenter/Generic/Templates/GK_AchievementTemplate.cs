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

public class GK_AchievementTemplate  {



	public string Id;
	public string Description;
	public float _progress;



	public float Progress {
		get {
			if(IOSNativeSettings.Instance.UsePPForAchievements) {
				return GameCenterManager.GetAchievementProgress(Id);
			} else {
				return _progress;
			}

		}

		set {
			_progress = value;
		}
	}
}
