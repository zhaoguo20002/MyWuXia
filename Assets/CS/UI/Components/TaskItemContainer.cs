using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Game;

public class TaskItemContainer : MonoBehaviour {
	public Text title;
	public Text desc;

	TaskData taskData;
	SceneData scene;
	NpcData npc;
	// Use this for initialization
	void Start () {
		if (title == null || desc == null) {
			enabled = false;
		}
	}

	public void UpdateData(TaskData data) {
		taskData = data;
		taskData.MakeJsonToModel();
		scene = JsonManager.GetInstance().GetMapping<SceneData>("Scenes", taskData.BelongToSceneId);
		npc = JsonManager.GetInstance().GetMapping<NpcData>("Npcs", taskData.BelongToNpcId);
	}

	public void RefreshView() {
		string color = "";
		string content = "";
		string rewards = "";
		DropData drop;
		for (int i = 0; i < taskData.Rewards.Count; i++) {
			drop = taskData.Rewards[i];
			rewards += drop.Item.Name + (drop.Num > 1 ? "*" + drop.Num : "");
			if (i < taskData.Rewards.Count - 1) {
				rewards += ", ";
			}
		}
		switch (taskData.State) {
		case TaskStateType.Accepted:
			color = "#FF0000";
			content = string.Format("{0}\n{1}", taskData.Desc, getDialogDesc(taskData.GetCurrentDialog()));
			break;
		case TaskStateType.CanAccept:
			color = "#00FFFF";
			content = string.Format("{0}\n{1}", taskData.Desc, getDialogDesc(taskData.GetCurrentDialog()));
			break;
		case TaskStateType.CanNotAccept:
			color = "#CCCCCC";
			string condition = "";
			switch (taskData.Type) {
			case TaskType.Gender:
				condition = (GenderType)taskData.IntValue == GenderType.Female ? "需要主角为 女性" : "需要主角为 男性";
				break;
			case TaskType.ItemInHand:
				condition = "需要拥有 " + JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", taskData.StringValue).Name;
				break;
			case TaskType.MoralRange:
				condition = "需要道德点区间为 " + taskData.MinIntValue + "-" + taskData.MaxIntValue;
				break;
			case TaskType.None:
				condition = "无限制";
				break;
			case TaskType.Occupation:
				condition = "主角所属门派必须为 " + Statics.GetOccupationName((OccupationType)taskData.IntValue);
				break;
			case TaskType.TheHour:
				condition = "需要到 " + Statics.GetTimeName(taskData.IntValue) + " 才能接取";
				break;
			default:
				break;
			}
			content = string.Format("{0}\n接取条件:<color=\"#FF0000\">{1}</color>", taskData.Desc, condition);
			break;
		case TaskStateType.Completed:
			color = "#333333";
			content = "已完成";
			break;
		case TaskStateType.Ready:
			color = "#00FF00";
			content = string.Format("{0}\n{1}", taskData.Desc, getDialogDesc(taskData.GetCurrentDialog()));
			break;
		default:
			color = "#FFFFFF";
			break;
		}
		title.text = string.Format("<color=\"{0}\">{1}</color>", color, taskData.Name);
		desc.text = string.Format("{0}\n奖励:{1}", content, rewards);
	}

	string getDialogDesc(TaskDialogData dialog) {
		string result = "";
		string noticeColor = "#F57729";
		switch (dialog.Type) {
		case TaskDialogType.Choice:
		case TaskDialogType.JustTalk:
			return string.Format("目标:前往<color=\"" + noticeColor + "\">{0}</color>的<color=\"" + noticeColor + "\">{1}</color>找<color=\"" + noticeColor + "\">{2}</color>交谈", scene.BelongToAreaName, scene.Name, npc.Name);
		default:
			return "";
		case TaskDialogType.ConvoyNpc:
			string[] fen = dialog.StringValue.Split(new char[] { '_' });
			return string.Format("目标:护送<color=\"" + noticeColor + "\">{0}</color>到达<color=\"" + noticeColor + "\">{1}</color>", JsonManager.GetInstance().GetMapping<NpcData>("Npcs", fen[0]).Name, fen[1]);
		case TaskDialogType.FightWined:
			return string.Format("目标:在<color=\"" + noticeColor + "\">{0}</color>中获胜", JsonManager.GetInstance().GetMapping<FightData>("Fights", dialog.StringValue).Name);
		case TaskDialogType.RecruitedThePartner:
			return string.Format("归附对象:<color=\"{0}\">{1}</color>\n目标: <color=\"" + noticeColor + "\">{1}</color>与你结伴同行", taskData.State == TaskStateType.Completed ? "#00FF00" : "#FF0000", JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", dialog.StringValue).Name);
		case TaskDialogType.SendItem:
			return string.Format("需要物品:<color=\"{0}\">{1}</color>{{2}/{3}}\n目标: 收集到足够数量的<color=\"" + noticeColor + "\">{1}</color>", taskData.State == TaskStateType.Completed ? "#00FF00" : "#FF0000", JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", dialog.StringValue).Name, dialog.CurrentNum, dialog.IntValue);
		case TaskDialogType.UsedTheBook:
			return string.Format("装备秘籍:<color=\"{0}\">{1}</color>\n目标: 将秘籍<color=\"" + noticeColor + "\">{1}</color>装备上", taskData.State == TaskStateType.Completed ? "#00FF00" : "#FF0000", JsonManager.GetInstance().GetMapping<BookData>("Books", dialog.StringValue).Name);
		case TaskDialogType.UsedTheSkillOneTime:
			return string.Format("施展招式:<color=\"{0}\">{1}</color>\n目标: 将招式<color=\"" + noticeColor + "\">{1}</color>施展一次", taskData.State == TaskStateType.Completed ? "#00FF00" : "#FF0000", JsonManager.GetInstance().GetMapping<SkillData>("Skills", dialog.StringValue).Name);
		case TaskDialogType.UsedTheWeapon:
			return string.Format("装备兵器:<color=\"{0}\">{1}</color>\n目标: 将兵器<color=\"" + noticeColor + "\">{1}</color>装备上", taskData.State == TaskStateType.Completed ? "#00FF00" : "#FF0000", JsonManager.GetInstance().GetMapping<WeaponData>("Weapons", dialog.StringValue).Name);
		case TaskDialogType.WeaponPowerPlusSuccessed:
			return string.Format("目标:招式施展时爆发<color=\"{0}\">{1}倍伤害</color>", taskData.State == TaskStateType.Completed ? "#00FF00" : "#FF0000", dialog.IntValue == 1 ? "1.25" : dialog.IntValue == 2 ? "1.5" : "2");
		}
		return result;
	}

}
