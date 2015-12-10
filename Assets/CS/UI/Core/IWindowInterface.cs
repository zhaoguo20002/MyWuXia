using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace Game {
	public interface IWindowInterface {
		/// <summary>
		/// Sets the identifier.
		/// </summary>
		/// <param name="id">Identifier.</param>
		void SetId(string id);
		/// <summary>
		/// Gets the identifier.
		/// </summary>
		/// <returns>The identifier.</returns>
		string GetId();
		/// <summary>
		/// Updates the data.
		/// </summary>
		/// <param name="obj">Object.</param>
		void UpdateData(object obj);
		/// <summary>
		/// Refreshs the view.
		/// </summary>
		void RefreshView();
		/// <summary>
		/// Sets the child path.
		/// </summary>
		/// <param name="path">Path.</param>
		void SetChildPath(string path);
	}
}
