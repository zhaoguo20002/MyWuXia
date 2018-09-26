using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json.Linq;
using DG.Tweening;


namespace Game{
	public class TestChildScrollViewCtrl : ComponentCore {

		
		private GameObject item;
		//private GridLayoutGroup glg;
		public Transform GridContentParent;
		RectTransform GCrt;
		float GCh;
		float addh = 60f;
		
		private int childCount;
		
		private Scrollbar _thescrollbar;
		
		void Awake(){
			item = GameObject.Find ("selectItem");
			//glg = transform.FindChild ("scrollshow").FindChild ("scrollcontent").GetComponent<GridLayoutGroup> ();
			GCrt = GridContentParent.GetComponent<RectTransform>();
			GCh = GCrt.rect.height;
			_thescrollbar = transform.Find("selectScrollview").Find("MyScrollbar").GetComponent<Scrollbar>();
		}
		
		void Update () {
			childCount = GridContentParent.childCount;
		}
		
		public void AddItem(){
			GameObject go = Instantiate (Resources.Load("selectItem")) as GameObject;
			//go.transform.DOScale (2 ,2);
			go.transform.SetParent (GameObject.FindGameObjectWithTag("content").transform);
			RectTransform pareContent = GridContentParent.transform.GetComponent<RectTransform> ();
			GCrt.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, GCrt.rect.height + addh);
			GCrt.anchoredPosition = new Vector2 (0f, (GCrt.rect.height - GCh) / 2f);

			Debug.Log (childCount);
					
		}
		public void MinusItem(){
			for(int i = 0; i < childCount; i ++){
				GameObject go = GridContentParent.GetChild(i).gameObject;
				Destroy(go);
				break;
			}
			RectTransform pareContent = GridContentParent.transform.GetComponent<RectTransform> ();
			GCrt.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, GCrt.rect.height - addh);
			GCrt.anchoredPosition = new Vector2 (0f, (GCrt.rect.height - GCh) / 2f);
			
		}

	}

}
