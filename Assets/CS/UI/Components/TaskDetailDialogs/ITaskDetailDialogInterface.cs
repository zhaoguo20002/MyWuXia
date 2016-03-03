using UnityEngine;
using System.Collections;

namespace Game {
	public interface ITaskDetailDialogInterface {
		/// <summary>
		/// Updates the data.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="data">Data.</param>
		/// <param name="willDuring">If set to <c>true</c> will during.</param>
		void UpdateData(string id, TaskDialogData data, bool willDuring);
		/// <summary>
		/// Refreshs the view.
		/// </summary>
		void RefreshView();
	}
}
