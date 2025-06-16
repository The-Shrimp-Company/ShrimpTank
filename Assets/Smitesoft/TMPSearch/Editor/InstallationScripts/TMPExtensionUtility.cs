using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using TMPExtension;
using System.Linq;


namespace TMPExtension
{
	public static class TMPExtensionUtility
	{
		//==== Check Unity Skin ====

		public static bool UnitySkinIsDark { get { return EditorPrefs.GetInt("UserSkin") == 1; } }


		//==== Check TMP Version >2019 ====

#if UNITY_2019_1_OR_NEWER
		public static string TMProVersionLaterThan2019()
        {
            var packageJsons = AssetDatabase.FindAssets("package")
            .Select(AssetDatabase.GUIDToAssetPath).Where(x => AssetDatabase.LoadAssetAtPath<TextAsset>(x) != null)
            .Select(UnityEditor.PackageManager.PackageInfo.FindForAssetPath).ToList();

            string TMPVersion = "Not Yet";

			foreach (var package in packageJsons)
			{

				if (package.name == "com.unity.textmeshpro")
				{
					TMPVersion = package.version;
					break;
				}
			}
			return TMPVersion;
		}

#endif

		//==== Check TMP Version <2019 ====

		public static string TMProVersionSub2019()
        {
			List<string> outLines = new List<string>();
			
			string[] dirs = Directory.GetFiles("Packages", "manifest.json", SearchOption.AllDirectories);
			string[] lines = File.ReadAllLines(dirs[0]);

			for (int i = 0; i < lines.Length; i++)             //Basically iterate through every line in the script
			{
				string line = lines[i];                                //Basically like highlighting and assining the current line to a string called line

				if (line.Contains("com.unity.textmeshpro"))
				{
					line = line.Replace("com.unity.textmeshpro", "");
					line = line.Replace(" ", "");     //here we are stating what that string should be replaced to
					line = line.Replace(":", "");
					line = line.Replace(",", "");
					line = line.Replace("\"", "");
					outLines.Add(line); 
				}					
			}
			return outLines[0];
        }


		//==== Smitesoft Banner=====

#region Layout
		public static void ShowBanner(Texture SmitesoftTexture)
        {
			var rect = GUILayoutUtility.GetRect(0f, 0f);
			rect.width = SmitesoftTexture.width;
			rect.height = SmitesoftTexture.height;
			//GUILayout.Space(rect.height);  //So it doesnt overlap with what comes after it "(I think)" //Remove if you want the text to overlap			
			GUI.DrawTexture(rect, SmitesoftTexture);


			//To make sure that clicking on the Banner doesnt do anything, (maybe we should make it that it takes you to smire soft website)
			var e = Event.current;
            if (e.type != EventType.MouseUp)
            {
				return;
            }
            if (!rect.Contains(e.mousePosition))
            {
				return;
            }
        }

#endregion

        //==== Making Tags ======

#region AddingBlockerTag

        public static bool CreateTag(string tag) //This is the tag we want to add
		{
			//I should add another check to see if they have reached maximum ammount of possible tags // nvm there seems to be no limit, its the layer that are limited

			var asset = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset");
			if (asset != null)
			{ // sanity checking
				var so = new SerializedObject(asset);
				var tags = so.FindProperty("tags");

				var numTags = tags.arraySize;
				// do not create duplicates
				for (int i = 0; i < numTags; i++)
				{
					var existingTag = tags.GetArrayElementAtIndex(i);
					Debug.Log(tag + " Tag Already Exists");
					if (existingTag.stringValue == tag) return true; //this is to make sure we dont add the same tag twice
				}

				tags.InsertArrayElementAtIndex(numTags);
				tags.GetArrayElementAtIndex(numTags).stringValue = tag;
				so.ApplyModifiedProperties();
				so.Update();
				Debug.Log(tag + " Tag Added Sucessfully");
				return true;
			}
			else
            {
				Debug.Log("Could not Find : TagManager.asset, please add the tag " + tag + " manually");
				return false;
			}			
		}

		public static bool CheckingTag(string tag)
        {
			var asset = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset");
			if (asset != null)
			{ // sanity checking
				var so = new SerializedObject(asset);
				var tags = so.FindProperty("tags");

				var numTags = tags.arraySize;
				// do not create duplicates
				for (int i = 0; i < numTags; i++)
				{
					var existingTag = tags.GetArrayElementAtIndex(i);					
					if (existingTag.stringValue == tag) return true; //this is to make sure we dont add the same tag twice
				}
				return false;
			}
			else
			{				
				return false;
			}

		}

#endregion


        //====   Writing : DropDown && InputField : **RUNTIME** :.CS + .Meta =========


#region InstallingDropdownSearchable.CS

        public static bool InstallDropdownMain()					// I am not sure why this is a bool return type at all
		{
			string[] dirs = Directory.GetDirectories("Library/PackageCache", "com.unity.textmeshpro*", SearchOption.AllDirectories); // I think this is getting the path containing the version number

			if (dirs.Length == 0) return false;
			
													    //I dont need an else here, only one return will run coz we would have exited the method anyway

			ProcessDropdown(dirs[0]);                   // run the function below with the first value(reference), probably the only reference in the array
			return true;
		}		

		public static void ProcessDropdown(string dir)
		{
			List<string> outLines = new List<string>();		//This is where we populate our script and add what we want to write!



			string inFile = Path.Combine(dir, "Scripts/Runtime/TMP_Dropdown.cs"); //I think this is combining the file path with version number and the rest of the path
			string[] lines = File.ReadAllLines(inFile);                           //I think this is an array of every line of code in that script
			int skipUnlessOne = 1;


			for (int i=0;i<lines.Length;i++)             //Basically iterate through every line in the script
			{

				skipUnlessOne = Mathf.Max((skipUnlessOne - 1), 1);
				//bool done = false;
				string line = lines[i];                  //Basically like highlighting and assining the current line to a string called line


				if (line.Contains("private void OnSelectItem(Toggle toggle)")) // 08-10-2021
				{
					outLines.Add("public UnityEvent OnSelected; //I Added This");
					outLines.Add("private void OnSelectItem(Toggle toggle)");

					skipUnlessOne = 2;
				}
				
				if (line.Contains("Hide();") && lines[i-1].Contains("value = selectedIndex;")) // 08-10-2021
				{
					outLines.Add("value = selectedIndex;");
					outLines.Add("Hide();");
					outLines.Add("OnSelected.Invoke(); // I Added this");

					skipUnlessOne = 2;
				}
				
				if (line.Contains("private void ImmediateDestroyDropdownList()"))
				{
					line = line.Replace("private", "public");     //here we are stating what that string should be replaced to
					outLines.Add(line);                                                //and here we actually add the line!

					skipUnlessOne = 2;					
				}

				if (line.Contains("protected virtual GameObject CreateBlocker(Canvas rootCanvas)"))
				{
					outLines.Add("public static List<GameObject> blockerList = new List<GameObject>();");    //15-03-2021
					outLines.Add("public bool activateBlockerTagging = false;");
					outLines.Add("protected virtual GameObject CreateBlocker(Canvas rootCanvas)");

					skipUnlessOne = 2;
				}

				if (line.Contains("blockerCanvas.sortingOrder = dropdownCanvas.sortingOrder - 1;"))
				{
					outLines.Add("blockerCanvas.sortingOrder = dropdownCanvas.sortingOrder - 1;");
					outLines.Add("if (activateBlockerTagging){blocker.tag = \"Blocker\";}");
					outLines.Add("blockerList.Add(blocker);");                                              //15-03-2021


					skipUnlessOne = 2;
				}

				if (line.Contains("UI/Dropdown - TextMeshPro"))
				{
					line = line.Replace("Dropdown", "DropdownSearchable");							 //here we are stating what that string should be replaced to
					line = line.Replace("35", "80");
					outLines.Add(line);															     //and here we actually add the line!

					skipUnlessOne = 2;
					//done = true;
				}

				if (line.Contains("TMP_Dropdown"))
				{
					line = line.Replace("TMP_Dropdown", "TMP_DropdownSearchable");     //here we are stating what that string should be replaced to
					outLines.Add(line);                                                //and here we actually add the line!

					skipUnlessOne = 2;
					//done = true;
				}

                if (line.Contains("void SetValue(int value, bool sendCallback = true)"))
                {
					outLines.Add("public delegate void OnNoValueChange();");
					outLines.Add("public event OnNoValueChange onNoValueChange;");
					outLines.Add(line);

					skipUnlessOne = 2;
				}

				if (line.Contains("if (Application.isPlaying && (value == m_Value || options.Count == 0))"))
				{
					outLines.Add(line);
					outLines.Add("{if (onNoValueChange != null){onNoValueChange();}  return;} //I added this");				

					skipUnlessOne = 3; //to skip the return
				}


				if (skipUnlessOne == 1) outLines.Add(line);
				//if (!done) outLines.Add(line);           // This means, otherwise, just add the line that was suppose to be there!
			}

			string outFile = Path.Combine(dir, "Scripts/Runtime/TMP_DropdownSearchable.cs"); //The File path for the new File			
            File.WriteAllLines(outFile, outLines.ToArray());   //this writes all the lines? though I am not sure what .ToArray() means... almost like we are converting our list to an array
															   //Ahh because WriteAllLines, can only work with arrays and not lists


			//For global Files
			string outFileGlobal = Path.Combine(InstallationWindow.globalRuntimeDirs, "TMP_DropdownSearchable.cs");
			File.WriteAllLines(outFileGlobal, outLines.ToArray());
		}

#endregion


#region InstallingDropdownSearchable.META

		public static bool InstallDropdownMeta()                  
		{
			string[] dirsMeta = Directory.GetDirectories("Assets/Smitesoft", "TempSearchableInstallationScripts", SearchOption.AllDirectories);		//this looks for the SmiteSoft Script folder

			if (dirsMeta.Length == 0) return false;

			//I dont need an else here, only one return will run coz we would have exited the method anyway

			ProcessDropdownMeta(dirsMeta[0]);                         // the only reference in the array (scripts folder)

			ProcessDropdownMetaDelete(dirsMeta[0]);                   // to delete the files.
			return true;
		}
		public static void ProcessDropdownMeta(string dir)
		{
			List<string> outLines = new List<string>();                       //This is where we populate our script and add what we want to write!

																																														
			string inFile = Path.Combine(dir, "TMP_DropdownSearchable.cs.meta");  //Note: .Meta didnt work, I beleive its always been .cs.meta
			string[] lines = File.ReadAllLines(inFile);                           //I think this is an array of every line of code in that script

			for (int i = 0; i < lines.Length; i++)             //Basically iterate through every line in the script
			{			
				string line = lines[i];					       //Basically like highlighting and assining the current line to a string called line				

				outLines.Add(line);							   //We are only reWriting this script
			}


			string[] dirs = Directory.GetDirectories("Library/PackageCache", "com.unity.textmeshpro*", SearchOption.AllDirectories);  //We are looking gor the Cache Directory to save the file there with the rest of TMP files

			dir = dirs[0];

			string outFile = Path.Combine(dir, "Scripts/Runtime/TMP_DropdownSearchable.cs.meta");    
			File.WriteAllLines(outFile, outLines.ToArray());

			//For global Files
			string outFileGlobal = Path.Combine(InstallationWindow.globalRuntimeDirs, "TMP_DropdownSearchable.cs.meta");
			File.WriteAllLines(outFileGlobal, outLines.ToArray());
		}

#endregion


#region InstallingInputFieldSearchable.CS

		public static bool InstallInputFieldMain()                    // I am not sure why this is a bool return type at all
		{
			string[] dirs = Directory.GetDirectories("Library/PackageCache", "com.unity.textmeshpro*", SearchOption.AllDirectories); // I think this is getting the path containing the version number

			if (dirs.Length == 0) return false;

			//I dont need an else here, only one return will run coz we would have exited the method anyway

			ProcessInpuField(dirs[0]);                   // run the function below with the first value(reference), probably the only reference in the array
			return true;
		}

		public static void ProcessInpuField(string dir)
		{
			List<string> outLines = new List<string>();     //This is where we populate our script and add what we want to write!



			string inFile = Path.Combine(dir, "Scripts/Runtime/TMP_InputField.cs"); //I think this is combining the file path with version number and the rest of the path
			string[] lines = File.ReadAllLines(inFile);                           //I think this is an array of every line of code in that script

			int skipUnlessOne = 1;

			for (int i = 0; i < lines.Length; i++)             //Basically iterate through every line in the script
			{
				
				string line = lines[i];                  //Basically like highlighting and assining the current line to a string called line

				if (line.Contains("static class SetPropertyUtility"))
				{
					break;
				}

				//bool done = false;
				skipUnlessOne = Mathf.Max((skipUnlessOne-1),1);

				if (line.Contains("public void DeactivateInputField(bool clearSelection = false)"))
				{
					outLines.Add("public bool SkipDeactivation = false;");
					outLines.Add("public void SkipDeactivationFunction(bool state) {SkipDeactivation = state;}");
					outLines.Add("public void DeactivateInputField(bool clearSelection = false){");
					outLines.Add("if (SkipDeactivation){return;}");

					skipUnlessOne = 3;  //this skips this line and the next line
				}


				if (line.Contains("TextMeshPro - Input Field"))
				{
					line = line.Replace("Input Field", "InputFieldSearchable");                           //here we are stating what that string should be replaced to
					line = line.Replace("11", "81");
					outLines.Add(line);                                                              //and here we actually add the line!
					
					skipUnlessOne = 2;
					//done = true;
				}



				if (line.Contains("TMP_InputField"))
				{
					line = line.Replace("TMP_InputField", "TMP_InputFieldSearchable");     //here we are stating what that string should be replaced to
					outLines.Add(line);                                                //and here we actually add the line!
					
					//done = true;	
					skipUnlessOne = 2;  //this skips this line
				}

				


				//if (!done) outLines.Add(line);           // This means, otherwise, just add the line that was suppose to be there!
			    if(skipUnlessOne==1) outLines.Add(line);
			}

			outLines.Add("}");						      //we are breaking out in the last function that we removed coz It was static, but we need to close the main class

			string outFile = Path.Combine(dir, "Scripts/Runtime/TMP_InputFieldSearchable.cs"); //The File path for the new File
			File.WriteAllLines(outFile, outLines.ToArray());   //this writes all the lines? though I am not sure what .ToArray() means... almost like we are converting our list to an array
															   //Ahh because WriteAllLines, can only work with arrays and not lists

			//For global Files
			string outFileGlobal = Path.Combine(InstallationWindow.globalRuntimeDirs, "TMP_InputFieldSearchable.cs");
			File.WriteAllLines(outFileGlobal, outLines.ToArray());
		}


#endregion


#region InstallingInputFieldSearchable.META

		public static bool InstallInputFieldMeta()
		{
			string[] dirsMeta = Directory.GetDirectories("Assets/Smitesoft", "TempSearchableInstallationScripts", SearchOption.AllDirectories);       //this looks for the SmiteSoft Script folder

			if (dirsMeta.Length == 0) return false;

			//I dont need an else here, only one return will run coz we would have exited the method anyway

			ProcessInputFieldMeta(dirsMeta[0]);                         // the only reference in the array (scripts folder)

			ProcessInputFieldMetaDelete(dirsMeta[0]);                   // to delete the files.
			return true;
		}

		public static void ProcessInputFieldMeta(string dir)
		{
			List<string> outLines = new List<string>();                       //This is where we populate our script and add what we want to write!


			string inFile = Path.Combine(dir, "TMP_InputFieldSearchable.cs.meta");  //Note: .Meta didnt work, I beleive its always been .cs.meta
			string[] lines = File.ReadAllLines(inFile);                           //I think this is an array of every line of code in that script

			for (int i = 0; i < lines.Length; i++)             //Basically iterate through every line in the script
			{
				string line = lines[i];                        //Basically like highlighting and assining the current line to a string called line				

				outLines.Add(line);                            //We are only reWriting this script
			}


			string[] dirs = Directory.GetDirectories("Library/PackageCache", "com.unity.textmeshpro*", SearchOption.AllDirectories);  //We are looking gor the Cache Directory to save the file there with the rest of TMP files

			dir = dirs[0];

			string outFile = Path.Combine(dir, "Scripts/Runtime/TMP_InputFieldSearchable.cs.meta");
			File.WriteAllLines(outFile, outLines.ToArray());

			//For global Files
			string outFileGlobal = Path.Combine(InstallationWindow.globalRuntimeDirs, "TMP_InputFieldSearchable.cs.meta");
			File.WriteAllLines(outFileGlobal, outLines.ToArray());
		}
#endregion






        //====   Writing : DropDown && InputField: ** EDITOR** :.CS + .Meta =========

#region InstallingDrownEditor.CS

        public static bool InstallDropdownEditorMain()                    // I am not sure why this is a bool return type at all
        {
            string[] dirs = Directory.GetDirectories("Library/PackageCache", "com.unity.textmeshpro*", SearchOption.AllDirectories); // I think this is getting the path containing the version number



															  //I dont need an else here, only one return will run coz we would have exited the method anyway
			if (dirs.Length == 0) return false;
			ProcessDropdownEditor(dirs[0]);                   // run the function below with the first value(reference), probably the only reference in the array
            return true;
        }

        public static void ProcessDropdownEditor(string dir)
        {
            List<string> outLines = new List<string>();                               //This is where we populate our script and add what we want to write!

            string inFile = Path.Combine(dir, "Scripts/Editor/TMP_DropdownEditor.cs"); //I think this is combining the file path with version number and the rest of the path
            string[] lines = File.ReadAllLines(inFile);                                //I think this is an array of every line of code in that script
            int skipUnlessOne = 1;


            for (int i = 0; i < lines.Length; i++)                                     //Basically iterate through every line in the script
            {

                skipUnlessOne = Mathf.Max((skipUnlessOne - 1), 1);

                string line = lines[i];                                                //Basically like highlighting and assining the current line to a string called line



                if (line.Contains("Dropdown"))
                {
                    line = line.Replace("Dropdown", "DropdownSearchable");     //here we are stating what that string should be replaced to
                    outLines.Add(line);                                                //and here we actually add the line!

                    skipUnlessOne = 2;
                }

                if (line.Contains("SerializedProperty m_Options;")) // 8-10-2021
                {
	                outLines.Add("SerializedProperty OnSelected; //I added this"); 
	                outLines.Add("SerializedProperty m_Options;"); 

	                skipUnlessOne = 2;
                }
                
                
                string m_Options = "m_Options = serializedObject.FindProperty(\"m_Options\");";
                string OnSelected = "OnSelected = serializedObject.FindProperty(\"OnSelected\"); //I Added This";
                if (line.Contains(m_Options)) // 8-10-2021
                {
	                
	                outLines.Add(OnSelected); 
	                outLines.Add(m_Options); 

	                skipUnlessOne = 2;
                }
                
                if (line.Contains("EditorGUILayout.PropertyField(m_OnSelectionChanged);")) // 8-10-2021
                {
	                outLines.Add("EditorGUILayout.PropertyField(OnSelected);  //I added this"); 
	                outLines.Add("EditorGUILayout.PropertyField(m_OnSelectionChanged);"); 

	                skipUnlessOne = 2;
                }

                

                if (skipUnlessOne == 1) outLines.Add(line);

            }

            string outFile = Path.Combine(dir, "Scripts/Editor/TMP_DropdownSearchableEditor.cs"); //The File path for the new File
            File.WriteAllLines(outFile, outLines.ToArray());                                 //this writes all the lines? though I am not sure what .ToArray() means... almost like we are converting our list to an array
																							 //Ahh because WriteAllLines, can only work with arrays and not lists

			string outFileGlobal = Path.Combine(InstallationWindow.globalEditorDirs, "TMP_DropdownSearchableEditor.cs");
			File.WriteAllLines(outFileGlobal, outLines.ToArray());
		}

#endregion


#region InstallingDrownEditor.META

		public static bool InstallDropdownEditorMeta()
		{
			string[] dirsMeta = Directory.GetDirectories("Assets/Smitesoft", "TempSearchableInstallationEditorScripts", SearchOption.AllDirectories);       //this looks for the SmiteSoft Script folder

			if (dirsMeta.Length == 0) return false;

			//I dont need an else here, only one return will run coz we would have exited the method anyway

			ProcessDropdownEditorMeta(dirsMeta[0]);                         // the only reference in the array (scripts folder)

			ProcessDropdownEditorMetaDelete(dirsMeta[0]);                   // to delete the files.
			return true;
		}
		public static void ProcessDropdownEditorMeta(string dir)
		{
			List<string> outLines = new List<string>();                       //This is where we populate our script and add what we want to write!


			string inFile = Path.Combine(dir, "TMP_DropdownSearchableEditor.cs.meta");  
			string[] lines = File.ReadAllLines(inFile);                          

			for (int i = 0; i < lines.Length; i++)             
			{
				string line = lines[i];                        			

				outLines.Add(line);                            
			}


			string[] dirs = Directory.GetDirectories("Library/PackageCache", "com.unity.textmeshpro*", SearchOption.AllDirectories);  //We are looking gor the Cache Directory to save the file there with the rest of TMP files

			dir = dirs[0];

			string outFile = Path.Combine(dir, "Scripts/Editor/TMP_DropdownSearchableEditor.cs.meta");
			File.WriteAllLines(outFile, outLines.ToArray());

			string outFileGlobal = Path.Combine(InstallationWindow.globalEditorDirs, "TMP_DropdownSearchableEditor.cs.meta");
			File.WriteAllLines(outFileGlobal, outLines.ToArray());
		}

#endregion


#region InstallingInputFieldEditor.CS
		public static bool InstallInputFieldEditorMain()                    // I am not sure why this is a bool return type at all
		{
			string[] dirs = Directory.GetDirectories("Library/PackageCache", "com.unity.textmeshpro*", SearchOption.AllDirectories); // I think this is getting the path containing the version number

			if (dirs.Length == 0) return false;

			//I dont need an else here, only one return will run coz we would have exited the method anyway

			ProcessInpuFieldEditor(dirs[0]);                   // run the function below with the first value(reference), probably the only reference in the array
			return true;
		}

		public static void ProcessInpuFieldEditor(string dir)
		{
			List<string> outLines = new List<string>();     //This is where we populate our script and add what we want to write!



			string inFile = Path.Combine(dir, "Scripts/Editor/TMP_InputFieldEditor.cs"); //I think this is combining the file path with version number and the rest of the path
			string[] lines = File.ReadAllLines(inFile);                           //I think this is an array of every line of code in that script

			int skipUnlessOne = 1;

			for (int i = 0; i < lines.Length; i++)             //Basically iterate through every line in the script
			{

				string line = lines[i];                  //Basically like highlighting and assining the current line to a string called line

				
				skipUnlessOne = Mathf.Max((skipUnlessOne - 1), 1);


				if (line.Contains("InputField"))
				{
					line = line.Replace("InputField", "InputFieldSearchable");     //here we are stating what that string should be replaced to
					outLines.Add(line);                                                //and here we actually add the line!

					//done = true;	
					skipUnlessOne = 2;  //this skips this line
				}

				if (skipUnlessOne == 1) outLines.Add(line);
			}

			

			string outFile = Path.Combine(dir, "Scripts/Editor/TMP_InputFieldSearchableEditor.cs"); //The File path for the new File
			File.WriteAllLines(outFile, outLines.ToArray());   //this writes all the lines? though I am not sure what .ToArray() means... almost like we are converting our list to an array
															   //Ahh because WriteAllLines, can only work with arrays and not lists

			string outFileGlobal = Path.Combine(InstallationWindow.globalEditorDirs, "TMP_InputFieldSearchableEditor.cs");
			File.WriteAllLines(outFileGlobal, outLines.ToArray());
		}

#endregion


#region InstallingInputFieldEditor.META
		public static bool InstallInputFieldEditorMeta()
		{
			string[] dirsMeta = Directory.GetDirectories("Assets/Smitesoft", "TempSearchableInstallationEditorScripts", SearchOption.AllDirectories);       //this looks for the SmiteSoft Script folder

			if (dirsMeta.Length == 0) return false;

			//I dont need an else here, only one return will run coz we would have exited the method anyway

			ProcessInputFieldEditorMeta(dirsMeta[0]);                         // the only reference in the array (scripts folder)

			ProcessInputFieldMetaEditorDelete(dirsMeta[0]);                   // to delete the files.
			return true;
		}

		public static void ProcessInputFieldEditorMeta(string dir)
		{
			List<string> outLines = new List<string>();                       //This is where we populate our script and add what we want to write!


			string inFile = Path.Combine(dir, "TMP_InputFieldSearchableEditor.cs.meta");  //Note: .Meta didnt work, I beleive its always been .cs.meta
			string[] lines = File.ReadAllLines(inFile);                           //I think this is an array of every line of code in that script

			for (int i = 0; i < lines.Length; i++)             //Basically iterate through every line in the script
			{
				string line = lines[i];                        //Basically like highlighting and assining the current line to a string called line				

				outLines.Add(line);                            //We are only reWriting this script
			}


			string[] dirs = Directory.GetDirectories("Library/PackageCache", "com.unity.textmeshpro*", SearchOption.AllDirectories);  //We are looking gor the Cache Directory to save the file there with the rest of TMP files

			dir = dirs[0];

			string outFile = Path.Combine(dir, "Scripts/Editor/TMP_InputFieldSearchableEditor.cs.meta");
			File.WriteAllLines(outFile, outLines.ToArray());

			string outFileGlobal = Path.Combine(InstallationWindow.globalEditorDirs, "TMP_InputFieldSearchableEditor.cs.meta");
			File.WriteAllLines(outFileGlobal, outLines.ToArray());
		}

#endregion



        //====   Cleaning temp files =========

#region DeletingTempFiles

        public static void ProcessDropdownMetaDelete(string dir)
		{
			// Not sure if I should add some sort of delay here since it is writing files and stuff-- (going to try without first)
			string tempFileCs = Path.Combine(dir, "TMP_DropdownSearchable.cs");
			string tempFileMeta = Path.Combine(dir, "TMP_DropdownSearchable.cs.meta");

			if (File.Exists (tempFileCs))
            {
				File.Delete(tempFileCs);
            }
            else
            {
				Debug.Log("tempTMP_DropdownSearchable.cs Could not be found to be removed");
            }

			if (File.Exists(tempFileMeta))
			{
				File.Delete(tempFileMeta);
			}
			else
			{
				Debug.Log("tempTMP_DropdownSearchable.cs.meta Could not be found to be removed");
			}
		}


		public static void ProcessInputFieldMetaDelete(string dir)
		{
			// Not sure if I should add some sort of delay here since it is writing files and stuff-- (going to try without first)
			string tempFileCs = Path.Combine(dir, "TMP_InputFieldSearchable.cs");
			string tempFileMeta = Path.Combine(dir, "TMP_InputFieldSearchable.cs.meta");

			if (File.Exists(tempFileCs))
			{
				File.Delete(tempFileCs);
			}
			else
			{
				Debug.Log("tempTMP_InputFieldSearchable.cs Could not be found to be removed");
			}

			if (File.Exists(tempFileMeta))
			{
				File.Delete(tempFileMeta);
			}
			else
			{
				Debug.Log("tempTMP_InputFieldSearchable.cs.meta Could not be found to be removed");
			}
		}


		public static void ProcessDropdownEditorMetaDelete(string dir)
		{
			// Not sure if I should add some sort of delay here since it is writing files and stuff-- (going to try without first)
			string tempFileCs = Path.Combine(dir, "TMP_DropdownSearchableEditor.cs");
			string tempFileMeta = Path.Combine(dir, "TMP_DropdownSearchableEditor.cs.meta");

			if (File.Exists(tempFileCs))
			{
				File.Delete(tempFileCs);
			}
			else
			{
				Debug.Log("TMP_DropdownSearchableEditor.cs Could not be found to be removed");
			}

			if (File.Exists(tempFileMeta))
			{
				File.Delete(tempFileMeta);
			}
			else
			{
				Debug.Log("TMP_DropdownSearchableEditor.cs.meta Could not be found to be removed");
			}
		}


		public static void ProcessInputFieldMetaEditorDelete(string dir)
		{
			// Not sure if I should add some sort of delay here since it is writing files and stuff-- (going to try without first)
			string tempFileCs = Path.Combine(dir, "TMP_InputFieldSearchableEditor.cs");
			string tempFileMeta = Path.Combine(dir, "TMP_InputFieldSearchableEditor.cs.meta");

			if (File.Exists(tempFileCs))
			{
				File.Delete(tempFileCs);
			}
			else
			{
				Debug.Log("TMP_InputFieldSearchableEditor.cs Could not be found to be removed");
			}

			if (File.Exists(tempFileMeta))
			{
				File.Delete(tempFileMeta);
			}
			else
			{
				Debug.Log("TMP_InputFieldSearchableEditor.cs.meta Could not be found to be removed");
			}
		}


#endregion

		//====   Removing All Added Files =========

#region DeletingAddedCache
		public static void DeleteLocalDllFiles()
        {
            foreach (var file in InstallationWindow.searchAbleLocal)
            {
                if (File.Exists(file))
                {
					File.Delete(file);
				}
				else
                {
					Debug.Log(file + "Was not found to be removed");
                }
            }
        }

		public static void DeleteGlobalDllFiles()
		{
			foreach (var file in InstallationWindow.searchAbleGlobal)
			{
				if (File.Exists(file))
				{
					File.Delete(file);
				}
				else
				{
					Debug.Log(file + "Was not found to be removed");
				}
			}
		}

#endregion

        








    }
}

