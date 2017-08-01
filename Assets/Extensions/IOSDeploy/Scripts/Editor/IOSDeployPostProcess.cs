using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Diagnostics;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class IOSDeployPostProcess  {


	[PostProcessBuild(100)]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
		#if UNITY_IPHONE &&  UNITY_EDITOR_WIN
			UnityEngine.Debug.LogWarning("ISD Postprocess is not avaliable for Win");
		#endif


		#if UNITY_IPHONE && UNITY_EDITOR_OSX

		Process myCustomProcess = new Process();		
		myCustomProcess.StartInfo.FileName = "python";

		string frameworks 		= string.Join(" ", ISDSettings.Instance.frameworks.ToArray());
		string libraries 		= string.Join(" ", ISDSettings.Instance.libraries.ToArray());
		string linkFlags 		= string.Join(" ", ISDSettings.Instance.linkFlags.ToArray());
		string compileFlags 	= string.Join(" ", ISDSettings.Instance.compileFlags.ToArray());


		myCustomProcess.StartInfo.Arguments = string.Format("Assets/Extensions/IOSDeploy/Scripts/Editor/post_process.py \"{0}\" \"{1}\" \"{2}\" \"{3}\" \"{4}\"", new object[] { pathToBuiltProject, frameworks, libraries, compileFlags, linkFlags });
		myCustomProcess.StartInfo.UseShellExecute = false;
		myCustomProcess.StartInfo.RedirectStandardOutput = true;
		myCustomProcess.Start(); 
		myCustomProcess.WaitForExit();

		if(ISDSettings.Instance.plistkeys.Count != ISDSettings.Instance.plistvalues.Count || ISDSettings.Instance.plistkeys.Count != ISDSettings.Instance.plisttags.Count)
		{
			UnityEngine.Debug.LogError ("The number of keys is not equal to the number of values in Plist values.");
		}
		else
		{
			XmlDocument document = new XmlDocument();
			string filePath = Path.Combine (pathToBuiltProject, "Info.plist");
			document.Load (filePath);
			document.PreserveWhitespace = true;

			for(int i = 0; i < ISDSettings.Instance.plistkeys.Count; i++)
			{
				XmlNode temp = document.SelectSingleNode( "/plist/dict/key[text() = '" + ISDSettings.Instance.plistkeys[i] + "']" );
				if(temp == null)
				{
					XmlNode keyNode = document.CreateElement ("key");
					keyNode.InnerText = ISDSettings.Instance.plistkeys[i];
					document.DocumentElement.FirstChild.AppendChild (keyNode);

					XmlNode valNode = null;

					if(string.IsNullOrEmpty (ISDSettings.Instance.plisttags[i]))
					{
						valNode = document.CreateElement(ISDSettings.Instance.plistvalues[i]);
					}
					else
					{
						valNode = document.CreateElement(ISDSettings.Instance.plisttags[i]);
						valNode.InnerText = ISDSettings.Instance.plistvalues[i];
					}
					document.DocumentElement.FirstChild.AppendChild (valNode);
				}
			}

			XmlWriterSettings settings  = new XmlWriterSettings {
				Indent = true,
				IndentChars = "\t",
				NewLineHandling = NewLineHandling.None
			};
			XmlWriter xmlwriter = XmlWriter.Create (filePath, settings );
			document.Save (xmlwriter);
			xmlwriter.Close ();

			System.IO.StreamReader reader = new System.IO.StreamReader(filePath);
			string textPlist = reader.ReadToEnd();
			reader.Close ();
			
			//strip extra indentation (not really necessary)
			textPlist = (new Regex("^\\t",RegexOptions.Multiline)).Replace(textPlist,"");
			
			//strip whitespace from booleans (not really necessary)
			textPlist = (new Regex("<(true|false) />",RegexOptions.IgnoreCase)).Replace(textPlist,"<$1/>");
			
			int fixupStart = textPlist.IndexOf("<!DOCTYPE plist PUBLIC");
			if(fixupStart <= 0)
				return;
			int fixupEnd = textPlist.IndexOf('>', fixupStart);
			if(fixupEnd <= 0)
				return;
			
			string fixedPlist = textPlist.Substring(0, fixupStart);
			fixedPlist += "<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">";
			fixedPlist += textPlist.Substring(fixupEnd+1);
			
			System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, false);
			writer.Write(fixedPlist);
			writer.Close ();
		}

		UnityEngine.Debug.Log("ISD Executing post process done.");

		#endif
	}




}
