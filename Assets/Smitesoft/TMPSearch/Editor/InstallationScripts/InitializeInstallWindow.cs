using UnityEditor;
using UnityEngine;
using System.IO;

namespace TMPExtension
{
    [InitializeOnLoad]
    public class InitializeInstallWindow
    {
        static InitializeInstallWindow()
        {
            EditorApplication.projectChanged += OnProjectChanged;
        }

        
        
        static void OnProjectChanged()
        {
            string[] dirs = Directory.GetDirectories("Assets/Smitesoft/TMPSearch", "Editor/Resources", SearchOption.AllDirectories);
            string SOPath = Path.Combine(dirs[0], "EditorDatabase.asset");


            if (File.Exists(SOPath))
            {
                //this returns true once we finish unpacking something or finish installing
                if (EditorScriptSO.ReturnState("InstallStage") == EditorScriptSO.ReturnState("InstallValveState")) 
                {
                    if (EditorScriptSO.ReturnState("InstallationState") != 2) //2 Means completed
                    {
                        InstallationWindow.ShowWindow();
                        EditorScriptSO.RunInstallValve();
                    }
                }
                else
                {
                    if (EditorScriptSO.ReturnState("InstallationState") == 1) //This MSG is generally shown if your installation is not completed..what value is completed???
					{
						Debug.Log("Note: TMPro_Searchable Installations/Status Window can be found in Tools -> Smitesoft -> TMP-Integration"); 
					}					
                    //Debug.Log("Install/Valve Missmatch");                    
                }
            }
            else //First Time we Unpack the Asset
            {
                Debug.Log("DataBaseCreated");
                EditorScriptSO.Start();
                EditorScriptSO.RunInstallValve();  //Adding the + 1 right of the get go
				EditorScriptSO.SaveInstallationsStatus(); //+1 means not Installed, +2 means its installed
				InstallationWindow.ShowWindow();
            }
        }
    }    

}

