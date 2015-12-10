using UnityEngine;
using System;
using System.Collections;
using System.Reflection;

namespace Game {
	public class NotifyBase {
		/// <summary>
		/// Init this instance.
		/// </summary>
		public static void Init() {
			Type notifyTypes = typeof(NotifyTypes);
			FieldInfo[] fields = notifyTypes.GetFields(BindingFlags.Public | BindingFlags.Static);
			foreach (var field in fields)
			{
				field.SetValue(null, field.Name);
			}
			Type notifyRegister = typeof(NotifyRegister);
			MethodInfo[] methods = notifyRegister.GetMethods(BindingFlags.Public | BindingFlags.Static);
			foreach (var method in methods)
			{
				method.Invoke(null, null);
			}
		}
	}
}

