using UnityEditor;
using UnityEngine;
using System.IO;

namespace TMPExtension
{
	public class ScriptableObjectEditorDatabase : ScriptableObject
	{
		public int installationStage;
		public int InstallationStage
		{
			get { return installationStage; }
			set
			{
				installationStage = value;
				EditorUtility.SetDirty(this);
			}
		}

		public int installationValve;
		public int InstallationValve
		{
			get { return installationValve; }
			set
			{
				installationValve = value;
				EditorUtility.SetDirty(this);
			}
		}

		public int installationsStatusSave;
		public int InstallationsStatusSave
		{
			get { return installationsStatusSave; }
			set
			{
				installationsStatusSave = value;
				EditorUtility.SetDirty(this);
			}
		}

		public static ScriptableObjectEditorDatabase CreateSO()                                 //why is the type of function the same as the name of the class?
		{
			try                                                                                 //incase directory is does not exist, it will catch an error. this is new to me
			{
				ScriptableObjectEditorDatabase so = ScriptableObject.CreateInstance<ScriptableObjectEditorDatabase>(); //This is creating an instance of the SO, can make multiples if we dont override
																													   //so is of type <ScriptableObjectExample>, which is why the SO says it belongs to the script "ScriptableObjectExample"
				AssetDatabase.CreateAsset(so, GetInstallPath("Editor/Resources/EditorDatabase.asset"));
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();

				EditorUtility.FocusProjectWindow();                                             //not sure what effect this had
				return so;                                                                      //so this is getting saved as SOE?.. not sure how long this will get saved for since its an editor script
																								//I mean, isnt this why where are making a SO in the first place?
			}
			catch
			{
				Debug.Log("Error");
			}
			return null;
		}

		public static ScriptableObjectEditorDatabase Open()
		{
			try
			{
				string path = GetInstallPath("Editor/Resources/EditorDatabase.asset");
				ScriptableObjectEditorDatabase so = AssetDatabase.LoadAssetAtPath(path, typeof(ScriptableObjectEditorDatabase)) as ScriptableObjectEditorDatabase;
				if (so)
					return so;
			}
			catch
			{
				Debug.Log("Error");
			}
			return null;
		}

		public static string GetInstallPath(string path)
		{
			string[] dirs = Directory.GetDirectories("Assets", "TMPSearch", SearchOption.AllDirectories);           //I dont need to say "Smitesoft", just TMPSearch and it will find it
																													//THis is so if they move the smite soft folder, it will still find it
			string dir = string.Empty;
			if (dirs.Length > 0)
			{
				dir = dirs[0];
			}
			else
			{
				dir = "Assets/Smitesoft/TMPSearch";                                                                 //Does this mean if we cant find Smitesoft, we will create the directory? needs testing
			}
			dir = Path.Combine(dir, path);
			return dir;
		}
	}

	public class EditorScriptSO : Editor
	{

		public static void Start()
		{
			// Create the SO. Only do this once //Overrides if you do it again
			ScriptableObjectEditorDatabase soe = ScriptableObjectEditorDatabase.CreateSO();
			//Debug.Log("Install Status @ Creation = " + soe.InstallationStage);
			//Debug.Log("Valve Status @ Creation = " + soe.InstallationValve);
		}

		public static void RunInstallStage()
		{
			// When you want to check the So or Update it's contents
			ScriptableObjectEditorDatabase soe = ScriptableObjectEditorDatabase.Open();
			soe.InstallationStage = soe.InstallationStage + 1;
			//Debug.Log("Install Status = " + soe.InstallationStage);
		}

		public static void RunInstallValve()
		{
			// When you want to check the So or Update it's contents
			ScriptableObjectEditorDatabase soe = ScriptableObjectEditorDatabase.Open();
			soe.InstallationValve = soe.InstallationValve + 1;
			//Debug.Log("Valve Status = " + soe.InstallationValve);
		}

		public static void SaveInstallationsStatus() // +1 = not completed, +2 = Completed
		{
			ScriptableObjectEditorDatabase soe = ScriptableObjectEditorDatabase.Open();
			soe.InstallationsStatusSave = soe.InstallationsStatusSave + 1;
		}

		public static int ReturnState(string dataRequest)
		{
			ScriptableObjectEditorDatabase soe = ScriptableObjectEditorDatabase.Open();
			if (dataRequest == "InstallStage")
			{
				return soe.InstallationStage;
			}
			
			if (dataRequest == "InstallValveState")
			{
				return soe.InstallationValve;
			}
			
			if (dataRequest == "InstallationState")
			{
				return soe.InstallationsStatusSave;
			}

			return -1;  // I can through an exception if it returns a -1 // or log an error or somthing
		}

		public static void Reset()
		{
			// Reset Soe Status
			ScriptableObjectEditorDatabase soe = ScriptableObjectEditorDatabase.Open();
			soe.InstallationStage = 0;
			//Debug.Log("Status = " + soe.InstallationStage);
		}
	}
}

