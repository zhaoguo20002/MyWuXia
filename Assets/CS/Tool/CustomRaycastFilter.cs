using UnityEngine;

public class CustomRaycastFilter : MonoBehaviour, ICanvasRaycastFilter {
	public RaycastMode mode;

	public enum RaycastMode {
		ReceiveNone,        // You can't interact with us or any of our children
		ReceiveAll,         // You can interact with us and all of our children
		OnlyChildsReceive   // You can't interact with us, but you can with any of our children
	}

	public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera) {
		switch (mode)
		{
		case RaycastMode.ReceiveNone:
			return false;
		case RaycastMode.ReceiveAll:

			RectTransform rt = transform as RectTransform;
			Vector2 localPos = rt.worldToLocalMatrix.MultiplyPoint(sp);
			return rt.rect.Contains(localPos);
		case RaycastMode.OnlyChildsReceive:
			for (int i = 0; i < transform.childCount; i++)
			{
				RectTransform childRect = transform.GetChild(i) as RectTransform;
				Vector2 childPos = childRect.worldToLocalMatrix.MultiplyPoint(sp);
				if (childRect.rect.Contains(childPos))
				{
					return true;
				}
			}
			return false;
		default:
			throw new System.NotImplementedException("Mode not implemented");
		}
	}
}