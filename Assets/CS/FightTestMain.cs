using UnityEngine;
using System.Collections;
using Game;
using System.Collections.Generic;

public class FightTestMain : MonoBehaviour {
	// Use this for initialization
	void Start () {
		List<RoleData> roleDatas = new List<RoleData>();
        if(!string.IsNullOrEmpty(PlayerPrefs.GetString("FightEditorTestRoleId0"))) {
            RoleData hostData = JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", PlayerPrefs.GetString("FightEditorTestRoleId0"));
            hostData.CurrentWeaponLV = PlayerPrefs.GetInt("TestHostWeaponLv");
            roleDatas.Add(hostData);
        }
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("FightEditorTestRoleId1"))) {
            roleDatas.Add(JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", PlayerPrefs.GetString("FightEditorTestRoleId1")));
        }
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("FightEditorTestRoleId2"))) {
            roleDatas.Add(JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", PlayerPrefs.GetString("FightEditorTestRoleId2")));
        }
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("FightEditorTestRoleId3"))) {
            roleDatas.Add(JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", PlayerPrefs.GetString("FightEditorTestRoleId3")));
        }
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("FightEditorTestRoleId4"))) {
            roleDatas.Add(JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", PlayerPrefs.GetString("FightEditorTestRoleId4")));
        }
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("FightEditorTestRoleId5"))) {
            roleDatas.Add(JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", PlayerPrefs.GetString("FightEditorTestRoleId5")));
        }
		for (int i= 0; i< roleDatas.Count; i++) {
			roleDatas[i].MakeJsonToModel();
        }
//        RoleInfoPanelCtrl.Show(roleDatas);
		Messenger.Broadcast<List<RoleData>, string>(NotifyTypes.CreateTestBattle, roleDatas, PlayerPrefs.GetString("FightEditorCurrentId"));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
