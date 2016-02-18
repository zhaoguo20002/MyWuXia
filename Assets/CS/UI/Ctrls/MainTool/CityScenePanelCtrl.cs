using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using DG;
using DG.Tweening;
using System.Collections.Generic;

namespace Game {
	public class CityScenePanelCtrl : WindowCore<CityScenePanelCtrl, JArray> {
		CanvasGroup bg;
		Text sceneNameText;
		Button enterAreaBtn;
		Button enterWorkshopBtn;
		Button enterStoreBtn;
		Button enterHospitalBtn;
		Button enterInnBtn;
		Button enterYamenBtn;
		Button enterWinshopBtn;
		Button enterForbiddenAreaBtn;
		GridLayoutGroup npcsGrid;
		List<NpcContainer> npcContainers;

		SceneData sceneData;

		protected override void Init () {
			bg = GetComponent<CanvasGroup>();
			bg.DOFade(0, 0);
			sceneNameText = GetChildText("sceneNameText");
			enterAreaBtn = GetChildButton("enterAreaBtn");
			EventTriggerListener.Get(enterAreaBtn.gameObject).onClick += onClick;
			enterWorkshopBtn = GetChildButton("enterWorkshopBtn");
			enterStoreBtn = GetChildButton("enterStoreBtn");
			enterHospitalBtn = GetChildButton("enterHospitalBtn");
			enterInnBtn = GetChildButton("enterInnBtn");
			enterYamenBtn = GetChildButton("enterYamenBtn");
			enterWinshopBtn = GetChildButton("enterWinshopBtn");
			enterForbiddenAreaBtn = GetChildButton("enterForbiddenAreaBtn");
			npcsGrid = GetChildGridLayoutGroup("npcsGrid");
			npcContainers = new List<NpcContainer>();
		}

		void onClick(GameObject e) {
			if (!e.GetComponent<Button>().enabled) {
				return;
			}
			switch (e.name) {
			case "enterAreaBtn":
				Hide();
				break;
			default:
				break;
			}
		}

		public void UpdateData(SceneData data) {
			sceneData = data;
		}

		public override void RefreshView () {
			sceneNameText.text = sceneData.Name;
			for (int i = npcContainers.Count - 1; i >= 0; i--) {
				Destroy(npcContainers[i].gameObject);
			}
			npcContainers.Clear();
			GameObject itemPrefab;
			NpcContainer container;
			for (int i = 0; i < sceneData.Npcs.Count; i++) {
				itemPrefab = Statics.GetPrefabClone("Prefabs/UI/GridItems/NpcItemContainer");
				itemPrefab.name = "NpcItemContainer" + i;
				MakeToParent(npcsGrid.transform, itemPrefab.transform);
				container = itemPrefab.GetComponent<NpcContainer>();
				container.SetNpcData(sceneData.Npcs[i]);
				npcContainers.Add(container);
			}
			FadeIn();
		}

		public void FadeIn() {
			bg.DOFade(1, 0.5f).SetDelay(0.3f);
		}

		public void FadeOut() {
			bg.DOFade(0, 0.5f).OnComplete(() => {
				Close();
			});
		}

		public static void Show(SceneData data) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/CityScenePanelView", "CityScenePanelCtrl");
			}
			Ctrl.UpdateData(data);
			Ctrl.RefreshView();
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.FadeOut();
			}
		}
	}
}
