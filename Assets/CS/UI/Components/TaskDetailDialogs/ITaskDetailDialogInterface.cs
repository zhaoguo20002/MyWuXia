using UnityEngine;
using System.Collections;

namespace Game {
	public interface ITaskDetailDialogInterface {
		/// <summary>
		/// TaskDialogStatusType status
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="data">Data.</param>
		/// <param name="willDuring">If set to <c>true</c> will during.</param>
		/// <param name="status">Status.</param>
		void UpdateData(string id, TaskDialogData data, bool willDuring, TaskDialogStatusType status);
		/// <summary>
		/// Refreshs the view.
		/// </summary>
		void RefreshView();
	}
}
