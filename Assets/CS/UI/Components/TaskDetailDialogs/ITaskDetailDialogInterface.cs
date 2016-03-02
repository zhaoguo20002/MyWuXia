using UnityEngine;
using System.Collections;

namespace Game {
	public interface ITaskDetailDialogInterface {
		/// <summary>
		/// Updates the data.
		/// </summary>
		/// <param name="data">Data.</param>
		void UpdateData(TaskData data);
		/// <summary>
		/// Refreshs the view.
		/// </summary>
		void RefreshView();
	}
}
