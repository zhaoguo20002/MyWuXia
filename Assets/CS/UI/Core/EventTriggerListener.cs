using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game {
//	public class EventTriggerListener : UnityEngine.EventSystems.EventTrigger {
	public class EventTriggerListener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, ISelectHandler, IUpdateSelectedHandler {
		public delegate void VoidDelegate (GameObject go);
		public VoidDelegate onClick;
		public VoidDelegate onDown;
		public VoidDelegate onEnter;
		public VoidDelegate onExit;
		public VoidDelegate onUp;
		public VoidDelegate onSelect;
		public VoidDelegate onUpdateSelect;
		
		static public EventTriggerListener Get (GameObject go) {
			EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
			if (listener == null) {
				listener = go.AddComponent<EventTriggerListener>();
			}
			return listener;
		}
		public void OnPointerClick(PointerEventData eventData) {
			if(onClick != null) {
				onClick(gameObject);
			}
		}
		public void OnPointerDown (PointerEventData eventData) {
			if(onDown != null) {
				onDown(gameObject);
			}
		}
		public void OnPointerEnter (PointerEventData eventData) {
			if(onEnter != null) {
				onEnter(gameObject);
			}
		}
		public void OnPointerExit (PointerEventData eventData) {
			if(onExit != null) {
				onExit(gameObject);
			}
		}
		public void OnPointerUp (PointerEventData eventData) {
			if(onUp != null) {
				onUp(gameObject);
			}
		}
		public void OnSelect (BaseEventData eventData) {
			if(onSelect != null) {
				onSelect(gameObject);
			}
		}
		public void OnUpdateSelected (BaseEventData eventData) {
			if(onUpdateSelect != null) {
				onUpdateSelect(gameObject);
			}
		}
	}
}