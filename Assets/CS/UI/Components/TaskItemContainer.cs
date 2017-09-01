using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System;

namespace Game {
	public class TaskItemContainer : MonoBehaviour {
		public Text title;
		public Text desc;
        public Button ScoutBtn;
		
		TaskData taskData;
        TaskDialogData currentDialog;
		SceneData scene;
		NpcData npc;
        JObject areaNames;
		string areaName;
		// Use this for initialization
		void Start () {
			if (title == null || desc == null) {
				enabled = false;
			}
		}
		
		public void UpdateData(TaskData data) {
            taskData = data;
            currentDialog = taskData.GetCurrentDialog();
			scene = JsonManager.GetInstance().GetMapping<SceneData>("Scenes", taskData.BelongToSceneId);
			npc = JsonManager.GetInstance().GetMapping<NpcData>("Npcs", taskData.BelongToNpcId);
			areaNames = JsonManager.GetInstance().GetJson("AreaNames");
			areaName = areaNames[scene.BelongToAreaName] != null ? areaNames[scene.BelongToAreaName]["Name"].ToString() : "";
            EventTriggerListener.Get(ScoutBtn.gameObject).onClick = onClick;
		}

        void onClick(GameObject e) {
            switch (e.name)
            {
                case "ScoutBtn":
                    ConfirmCtrl.Show("是否派出一个探子追踪目标？", () => {
                        if (DbManager.Instance.GetProp(PropType.Scout) == null) {
                            AlertCtrl.Show("没有探子可以派遣");
                            return;
                        }
                        EventData eventData = DbManager.Instance.GetActiveEvent(currentDialog.StringValue);
                        if (eventData != null) {
                            AlertCtrl.Show(string.Format("追踪结果: {0} X: {1} Y: {2}\n<color=\"#00FF00\">(请牢记该坐标)</color>", areaNames[eventData.SceneId] != null ? areaNames[eventData.SceneId]["Name"].ToString() : "", eventData.X, eventData.Y));
                        }
                        else {
                            AlertCtrl.Show("查询不到该事件信息");
                        }
                        DbManager.Instance.UseProp(PropType.Scout, 1);
                        Messenger.Broadcast(NotifyTypes.MakeUpdateProps);
                    });
                    break;
                default:
                    break;
            }
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
                    color = "#00FF00";
                    content = string.Format("{0}\n{1}", taskData.Desc, getDialogDesc(currentDialog));
                    break;
                case TaskStateType.CanAccept:
                    color = "#00FFFF";
                    content = string.Format("{0}\n{1}", taskData.Desc, getDialogDesc(currentDialog));
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
                    content = string.Format("{0}\n接取条件:<color=\"#FF0000\">{1}</color>\n目标:前往<color=\"#F57729\">{2}</color>的<color=\"#F57729\">{3}</color>找<color=\"#F57729\">{4}</color>交谈", taskData.Desc, condition, areaName, scene.Name, npc.Name);
                    break;
                case TaskStateType.Completed:
                    color = "#999999";
                    content = "已完成";
                    break;
                case TaskStateType.Ready:
                    color = "#00FF00";
                    content = string.Format("{0}\n{1}", taskData.Desc, getDialogDesc(currentDialog));
                    break;
                default:
                    color = "#FFFFFF";
                    break;
            }
			title.text = string.Format("<color=\"{0}\">{1}</color>", color, taskData.Name);
			desc.text = string.Format("{0}\n奖励:{1}", content, rewards != "" ? rewards : "无");
            if (currentDialog.Type == TaskDialogType.EventFightWined || currentDialog.Type == TaskDialogType.CreateTaskIsBindedWithEvent)
            {
                ScoutBtn.gameObject.SetActive(true);
            }
            else
            {
                ScoutBtn.gameObject.SetActive(false);
            }
		}
		
		string getDialogDesc(TaskDialogData dialog) {
			string result = "";
			if (dialog != null) {
				string noticeColor = "#F57729";
				switch (dialog.Type) {
				case TaskDialogType.Choice:
					return string.Format("目标:前往<color=\"" + noticeColor + "\">{0}</color>，<color=\"" + noticeColor + "\">{1}</color>的<color=\"" + noticeColor + "\">{2}</color>有困难需要你的帮助", areaName, scene.Name, npc.Name);
				case TaskDialogType.JustTalk:
				case TaskDialogType.Notice:
					return string.Format("目标:前往<color=\"" + noticeColor + "\">{0}</color>的<color=\"" + noticeColor + "\">{1}</color>找<color=\"" + noticeColor + "\">{2}</color>交谈", areaName, scene.Name, npc.Name);
				default:
					return "";
				case TaskDialogType.ConvoyNpc:
					string[] fen = dialog.StringValue.Split(new char[] { '_' });
					return string.Format("目标:护送<color=\"" + noticeColor + "\">{0}</color>到达<color=\"" + noticeColor + "\">{1}</color>({2})", JsonManager.GetInstance().GetMapping<NpcData>("Npcs", fen[0]).Name, fen[1], dialog.Completed ? "<color=\"#00FF00\">已到达</color>" : "<color=\"#FF0000\">未到达</color>") + (dialog.Completed ? string.Format(" (完成!{0}在{1}等你回复)", npc.Name, scene.Name) : "");
				case TaskDialogType.FightWined:
                    return string.Format("目标:在战斗[<color=\"" + noticeColor + "\">{0}</color>]中获胜({1})", JsonManager.GetInstance().GetMapping<FightData>("Fights", dialog.StringValue).Name, dialog.Completed ? "<color=\"#00FF00\">已获胜</color>" : "<color=\"#FF0000\">未获胜</color>") + (dialog.Completed ? string.Format(" (<color=\"#00FF00\">完成!</color>{0}在{1}等你回复)", npc.Name, scene.Name) : "");
				case TaskDialogType.EventFightWined:
					return string.Format("目标:在野外战斗[<color=\"" + noticeColor + "\">{0}</color>]中获胜({1})", JsonManager.GetInstance().GetMapping<FightData>("Fights", dialog.StringValue).Name, dialog.Completed ? "<color=\"#00FF00\">已获胜</color>" : "<color=\"#FF0000\">未获胜</color>") + (dialog.Completed ? string.Format(" (<color=\"#00FF00\">完成!</color>{0}在{1}等你回复)", npc.Name, scene.Name) : "");
				case TaskDialogType.RecruitedThePartner:
					RoleData role = JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", dialog.StringValue);
					return string.Format("结识对象:<color=\"{0}\">{1}</color>\n目标: 在<color=\"" + noticeColor + "\">{2}</color>的酒馆里与<color=\"" + noticeColor + "\">{1}</color>结识", dialog.Completed ? "#00FF00" : "#FF0000", role.Name, JsonManager.GetInstance().GetMapping<SceneData>("Scenes", role.HometownCityId).Name) + (dialog.Completed ? string.Format(" (完成!{0}在{1}等你回复)", npc.Name, scene.Name) : "");
				case TaskDialogType.SendItem:
					return string.Format("需要物品:<color=\"{0}\">{1}</color>\n目标: 收集到{2}个<color=\"" + noticeColor + "\">{1}</color>", dialog.Completed ? "#00FF00" : "#FF0000", JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", dialog.StringValue).Name, dialog.IntValue) + (dialog.Completed ? string.Format(" (<color=\"#00FF00\">完成!</color>{0}在{1}等你回复)", npc.Name, scene.Name) : "");
				case TaskDialogType.SendResource:
					return string.Format("需要资源:<color=\"{0}\">{1}</color>\n目标: 收集到{2}个<color=\"" + noticeColor + "\">{1}</color>", dialog.Completed ? "#00FF00" : "#FF0000", Statics.GetResourceName((ResourceType)Enum.Parse(typeof(ResourceType), dialog.StringValue)), dialog.IntValue) + (dialog.Completed ? string.Format(" (<color=\"#00FF00\">完成!</color>{0}在{1}等你回复)", npc.Name, scene.Name) : "");
				case TaskDialogType.UsedTheBook:
					return string.Format("装备秘籍:<color=\"{0}\">{1}</color>\n目标: 将秘籍<color=\"" + noticeColor + "\">{1}</color>装备上", dialog.Completed ? "#00FF00" : "#FF0000", JsonManager.GetInstance().GetMapping<BookData>("Books", dialog.StringValue).Name) + (dialog.Completed ? string.Format(" (<color=\"#00FF00\">完成!</color>{0}在{1}等你回复)", npc.Name, scene.Name) : "");
				case TaskDialogType.UsedTheSkillOneTime:
					return string.Format("施展招式:<color=\"{0}\">{1}</color>\n目标: 将招式<color=\"" + noticeColor + "\">{1}</color>施展一次", dialog.Completed ? "#00FF00" : "#FF0000", JsonManager.GetInstance().GetMapping<SkillData>("Skills", dialog.StringValue).Name) + (dialog.Completed ? string.Format(" (<color=\"#00FF00\">完成!</color>{0}在{1}等你回复)", npc.Name, scene.Name) : "");
				case TaskDialogType.UsedTheWeapon:
					return string.Format("装备兵器:<color=\"{0}\">{1}</color>\n目标: 将兵器<color=\"" + noticeColor + "\">{1}</color>装备上", dialog.Completed ? "#00FF00" : "#FF0000", JsonManager.GetInstance().GetMapping<WeaponData>("Weapons", dialog.StringValue).Name) + (dialog.Completed ? string.Format(" (<color=\"#00FF00\">完成!</color>{0}在{1}等你回复)", npc.Name, scene.Name) : "");
				case TaskDialogType.WeaponPowerPlusSuccessed:
					return string.Format("目标:招式施展时爆发<color=\"{0}\">{1}倍伤害</color>", dialog.Completed ? "#00FF00" : "#FF0000", dialog.IntValue == 1 ? "1.25" : dialog.IntValue == 2 ? "1.5" : "2") + (dialog.Completed ? string.Format(" (<color=\"#00FF00\">完成!</color>{0}在{1}等你回复)", npc.Name, scene.Name) : "");
				case TaskDialogType.TheHour:
					return string.Format("目标:在<color=\"" + noticeColor + "\">{0}</color>时前往<color=\"" + noticeColor + "\">{1}</color>的<color=\"" + noticeColor + "\">{2}</color>找<color=\"" + noticeColor + "\">{3}</color>交谈({4})", Statics.GetTimeName(dialog.IntValue), areaName, scene.Name, npc.Name, dialog.Completed ? "<color=\"#00FF00\">时辰已到</color>" : "<color=\"#FF0000\">时辰未到</color>");
				case TaskDialogType.PushRoleToWinshop:
					return string.Format("目标:前往<color=\"" + noticeColor + "\">{0}</color>的<color=\"" + noticeColor + "\">{1}</color>找<color=\"" + noticeColor + "\">{2}</color>交谈后可以结识新的侠客", areaName, scene.Name, npc.Name);
				case TaskDialogType.CreateTaskIsBindedWithEvent:
					return string.Format("目标:前往<color=\"" + noticeColor + "\">{0}</color>野外区域寻找线索", areaName);
				}

			}
			return result;
		}
		
	}
}
