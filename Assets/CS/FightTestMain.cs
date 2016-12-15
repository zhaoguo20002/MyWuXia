using UnityEngine;
using System.Collections;
using Game;
using System.Collections.Generic;

public class FightTestMain : MonoBehaviour {
	// Use this for initialization
	void Start () {
		List<RoleData> roleDatas = new List<RoleData>();
		roleDatas.Add(JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", PlayerPrefs.GetString("FightEditorTestRoleId0")));
		roleDatas.Add(JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", PlayerPrefs.GetString("FightEditorTestRoleId1")));
		roleDatas.Add(JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", PlayerPrefs.GetString("FightEditorTestRoleId2")));
		roleDatas.Add(JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", PlayerPrefs.GetString("FightEditorTestRoleId3")));
		for (int i= 0; i< roleDatas.Count; i++) {
			roleDatas[i].MakeJsonToModel();
        }
        RoleInfoPanelCtrl.Show(roleDatas);
		Messenger.Broadcast<RoleData, string>(NotifyTypes.CreateTestBattle, roleDatas[0], PlayerPrefs.GetString("FightEditorCurrentId"));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
