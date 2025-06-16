using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.Compilation;
using System.IO;
using System;

namespace TMPExtension
{
    public class InstallationWindow : EditorWindow
    {
        [MenuItem("Tools/Smitesoft/TMP-Integration")]
        public static void ShowWindow()
        {
            GetWindow<InstallationWindow>("Smitesoft");
        }


#region Fields

        public static string globalRuntimeDirs; //weird that I can have this inside and outside a function
        public static string globalEditorDirs;  //weird that I can have this inside and outside a function

        private bool compatible;
        private bool unityCompatible;
        private bool tmpCompatible;

        private static bool _debugging;

        private static bool _setupEssentialsCompleted;
        private static bool _installPackagesCompleted;
        private static bool _recompilationCompleted;
        private string status = "Not - Installed";


        private bool haveWeAddedBlocker = false;
        private const string TagToAdd = "Blocker";

        private bool haveWeInstalledLocally;
        private bool haveWeInstalledGlobally;

        private static bool _haveWeUnPackedManagerPrefab;
        private static bool _haveWeUnpackedTextMeshProEssentials;
        private static bool _haveWeUnpackedTextMeshProExtras;

        private const string TextMeshProEssentialsDir =
            "Packages/com.unity.textmeshpro/Package Resources/TMP Essential Resources.unitypackage";

        private const string TextMeshProExamplesDir = "Packages/com.unity.textmeshpro/Package Resources/TMP Examples & Extras.unitypackage";

        private const string ManagerPrefabPlusDummyDontClickA =
            "Assets/Smitesoft/TMPSearch/Editor/SmitesoftInstallationPackages/IgnoreThisFile/DontClickA.unitypackage";

        private const string ManagerPrefabWithoutDummyDontClickB =
            "Assets/Smitesoft/TMPSearch/Editor/SmitesoftInstallationPackages/IgnoreThisFile/DontClickB.unitypackage";

        private readonly string[] tmpCompatibleVersionsArray =
            {"2.1.0", "2.1.1", "2.1.3", "2.1.4", "2.1.6", "3.0.1", "3.0.3", "3.0.4", "3.0.6"};
        //,"3.2.0" ,"2.2.0"  (These need testing)

        private Color newRedColorForDarkTheme;
        private Color newGreenColorForLightTheme;
        private Color newYellowColorForLightTheme;
        private Color newRedColorForLightTheme;


        private bool valveToMakeSureTheyDontInstallAgain = false;
        //TODO:...this makes no sense, its not like this values gets saved? its an Editor script...This is BS

        private int successInstallNumber = 0;
        private int installNumberFailed = 0;
        private bool installSuccess;

#endregion

        private void OnEnable()
        {
            ShowWindow();

            OnEnableButTriggerAble();

            newRedColorForDarkTheme = new Color(243f / 255f, 16f / 255f, 16f / 255f, 1.0f);
            newRedColorForLightTheme = new Color(255f / 255f, 0f / 255f, 0f / 255f, 0.3f);
            newGreenColorForLightTheme = new Color(0f / 255f, 255f / 255f, 0f / 255f, 0.3f);
            newYellowColorForLightTheme = new Color(255f / 255f, 213f / 255f, 5f / 255f, 0.3f);
        }


        private void OnInspectorUpdate() //Slow update
        {
            UpdateInstallerUI();
        }

        private void OnEnableButTriggerAble()
        {
            _haveWeUnpackedTextMeshProEssentials = CheckingTextoMeshProFile();

            _haveWeUnpackedTextMeshProExtras = CheckingTextMeshProExtras();

            _haveWeUnPackedManagerPrefab = CheckingManagerPrefab();

            //if we get false that means we do not have it locally, but we could still have it globally
            haveWeInstalledLocally = checkingDllFiles();

            if (searchAbleGlobal.Count > 0)
            {
                haveWeInstalledGlobally = true;
            }
            else
            {
                haveWeInstalledGlobally = false;
            }

            haveWeAddedBlocker = TMPExtensionUtility.CheckingTag(TagToAdd);

            HaveWeAlreadyInstalled(haveWeAddedBlocker, haveWeInstalledLocally);

            //This will allow rechecking after Unity reboot //TODO: this could have been source of a problem for us? (Keep an eye on this logic)
            haveWeCheckUnityVersion = false;
            //This will allow rechecking after Unity reboot //TODO: this could have been source of a problem for us? (Keep an eye on this logic)
            haveWeCheckTMPVersion = false;
        }

        public void OnFocus()
        {
            _haveWeUnpackedTextMeshProEssentials = CheckingTextoMeshProFile();

            _haveWeUnpackedTextMeshProExtras = CheckingTextMeshProExtras();

            _haveWeUnPackedManagerPrefab = CheckingManagerPrefab();

            VersionCheckTMP();
            CompatibilityCheckTMPro();

            UnityVersionCheck();
            CompatibilityCheckUnity();
        }

        private void HaveWeAlreadyInstalled(bool didWeAddBlocker, bool installed)
        {
            if (didWeAddBlocker)
            {
                _setupEssentialsCompleted = true;
                InstallationStatus();
            }

            if (installed)
            {
                _installPackagesCompleted = true;
                _recompilationCompleted = true;
                valveToMakeSureTheyDontInstallAgain = true;
                InstallationStatus();
            }
        }


        private string InstallationStatus()
        {
            if (_installPackagesCompleted && _recompilationCompleted && _setupEssentialsCompleted &&
                EditorScriptSO.ReturnState("InstallationState") == 2)
            {
                status = "Completed!";
            }
            else if (_setupEssentialsCompleted)
            {
                status = "Pending Installation";
            }

            return status;
        }

        private void OnGUI()
        {
            //if (GUILayout.Button("Testing"))
            //{
            //    //EditorScriptSO.Start();
            //    //GetWindow<EditorWindow>().Close();
            //    OpenFullReference();
            //}

            BannerAndHeaderMethods();

            VideosAndDocumentation();


            if (
                _haveWeUnPackedManagerPrefab) // This should be, if we have a Manager folder or even a Note thats comes with it.. stupid way to check, but w/e
            {
                UnityVersionDisplay();

                TmpVersionDisplay();


                GUILayout.Space(15);

                CompatabilityCheckDisplay();

                if (compatible)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    SetupEssentials();


                    if (_setupEssentialsCompleted && !valveToMakeSureTheyDontInstallAgain)
                    {
                        if (GUILayout.Button("Install Packages"))
                        {
                            Debug.Log("Installing TMP Extension");

                            EditorScriptSO.RunInstallStage(); //Increase +1


                            InstallAll();

                            _installPackagesCompleted = true;
                            InstallationStatus();

                            AssetDatabase.Refresh();

                            _recompilationCompleted = true;
                            InstallationStatus();

                            Debug.Log("TMP-Integration:  Complete");
                            EditorScriptSO.SaveInstallationsStatus();   //Increase +1 to make it +2 on the databse
                            valveToMakeSureTheyDontInstallAgain = true; //Just so they dont install it again
                        }
                    }
                    else
                    {
                        GUI.enabled = false;
                        if (GUILayout.Button("Install Packages"))
                        {
                        }

                        GUI.enabled = true;
                    }

                    EditorGUILayout.EndVertical();
                }
                else //Show but Greyed out
                {
                    GUI.enabled = false;
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    if (GUILayout.Button("Setup-Essential"))
                    {
                    }

                    if (GUILayout.Button("Install Packages"))
                    {
                    }

                    if (GUILayout.Button("Apply Changes"))
                    {
                    }

                    EditorGUILayout.EndVertical();
                    GUI.enabled = true;
                }


                EditorGUILayout.Space();
                InstallationStatusDisplay();
                EditorGUILayout.Space();


                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                if (InstallationStatus() == "Completed!")
                {
                    if (GUILayout.Button("User Manual"))
                    {
                        OpenFullReference();
                    }

                    if (GUILayout.Button("User Video Manual "))
                    {
                        Application.OpenURL("https://youtu.be/hp3cSSxaYqc");
                    }
                }
                else
                {
                    GUI.enabled = false;
                    if (GUILayout.Button("User Manual"))
                    {
                        OpenFullReference();
                    }

                    if (GUILayout.Button("User Video Manual "))
                    {
                        Application.OpenURL("https://youtu.be/hp3cSSxaYqc");
                    }

                    GUI.enabled = true;
                }


                EditorGUILayout.EndVertical();


                EditorGUILayout.Space();
                GUILayout.Label("Contact us", EditorStyles.label);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                if (GUILayout.Button("Discord"))
                {
                    Application.OpenURL("https://discord.gg/d39KwkkWn3");
                }

                // if (GUILayout.Button("Forums"))
                // {
                //     Application.OpenURL("https://smitesoft.com/community");
                // }

                EditorGUILayout.EndVertical();


                GUILayout.Space(15);


                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Special Thanks:", EditorStyles.boldLabel);
                GUILayout.Label("Developer Page:", EditorStyles.boldLabel, GUILayout.MaxWidth(200));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();


                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("SteveSmith", EditorStyles.label);
                
                if (GUILayout.Button("Asset Store", GUILayout.MaxWidth(200)))
                {
                    Application.OpenURL("https://assetstore.unity.com/publishers/35776");
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Invertex", EditorStyles.label);
                if (GUILayout.Button("Portfolio", GUILayout.MaxWidth(200)))
                {
                    Application.OpenURL("https://forum.unity.com/members/invertex.458918/");
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Exanite", EditorStyles.label);
                if (GUILayout.Button("Portfolio", GUILayout.MaxWidth(200)))
                {
                    Application.OpenURL("https://github.com/Exanite");
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Austin Rife", EditorStyles.label);
                if (GUILayout.Button("Portfolio", GUILayout.MaxWidth(200)))
                {
                    Application.OpenURL("https://techgeek1.github.io/about");
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                _debugging = GUILayout.Toggle(_debugging, " Debugging Mode");

                if (_debugging)
                {
                    EditorGUILayout.Space();
                    GUI.enabled = false;
                    GUILayout.TextArea(
                        "Note: Debugging mode will activate all Logs, It is advised to Turn this mode off, important Logs will remain active regardless",
                        300, GUILayout.ExpandHeight(true), GUILayout.MaxHeight(50));
                    GUI.enabled = true;
                    EditorGUILayout.Space();


                    GUI.enabled = false;
                    GUI.contentColor = Color.yellow;
                    GUILayout.TextArea(
                        "Warrning!!, Only Delete Cache in a fresh project that does not have this asset fully Installed," +
                        " This asset cannot function without its Cache and it will attempt to reinstall all the cache with " +
                        "a fresh installation of this asset. ", 300, GUILayout.ExpandHeight(true), GUILayout.MaxHeight(70));
                    GUI.contentColor = Color.white;
                    GUI.enabled = true;

                    EditorGUILayout.Space();
                    GUI.contentColor = Color.yellow;
                    if (InstallationStatus() == "Completed!")
                    {
                        GUI.enabled = false;
                        if (GUILayout.Button("Delete Cache"))
                        {
                            TMPExtensionUtility.DeleteGlobalDllFiles();
                            TMPExtensionUtility.DeleteLocalDllFiles();
                            OnEnableButTriggerAble();
                        }

                        GUI.enabled = true;
                    }
                    else
                    {
                        if (GUILayout.Button("Delete Cache"))
                        {
                            TMPExtensionUtility.DeleteGlobalDllFiles();
                            TMPExtensionUtility.DeleteLocalDllFiles();
                            OnEnableButTriggerAble();
                        }
                    }

                    GUI.contentColor = Color.white;
                }
            }
            else //This applies if they have the files globbaly but not locally, so we need to purge them
            {
                EditorGUILayout.Space();
                GUILayout.Label("Unity Version", EditorStyles.label);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(UnityVersion, EditorStyles.textArea);
                if (UnityCompatability == "Compatible")
                {
                    if (TMPExtensionUtility.UnitySkinIsDark)
                    {
                        GUI.contentColor = Color.green;
                        GUILayout.Label(UnityCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                        GUI.contentColor = Color.white;
                    }
                    else
                    {
                        GUI.backgroundColor = newGreenColorForLightTheme;
                        GUILayout.Label(UnityCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                        GUI.backgroundColor = Color.white;
                    }
                }
                else
                {
                    if (TMPExtensionUtility.UnitySkinIsDark)
                    {
                        GUI.contentColor = newRedColorForDarkTheme;
                        GUILayout.Label(UnityCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                        GUI.contentColor = Color.white;
                    }
                    else
                    {
                        GUI.backgroundColor = newRedColorForLightTheme;
                        GUILayout.Label(UnityCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                        GUI.backgroundColor = Color.white;
                    }
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
                GUILayout.Label("TMPro Version", EditorStyles.label);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(TMProVersion, EditorStyles.textArea);

                if (TMProCompatability == "Compatible")
                {
                    if (TMPExtensionUtility.UnitySkinIsDark)
                    {
                        GUI.contentColor = Color.green;
                        GUILayout.Label(TMProCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                        GUI.contentColor = Color.white;
                    }
                    else
                    {
                        GUI.backgroundColor = newGreenColorForLightTheme;
                        GUILayout.Label(TMProCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                        GUI.backgroundColor = Color.white;
                    }
                }
                else
                {
                    if (TMPExtensionUtility.UnitySkinIsDark)
                    {
                        GUI.contentColor = newRedColorForDarkTheme;
                        GUILayout.Label(TMProCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                        GUI.contentColor = Color.white;
                    }
                    else
                    {
                        GUI.backgroundColor = newRedColorForLightTheme;
                        GUILayout.Label(TMProCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                        GUI.backgroundColor = Color.white;
                    }
                }


                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                if (tmpCompatible && unityCompatible) // Unity version check and TMP version Check // or if Installation completed
                {
                    compatible = true;
                }
                else
                {
                    compatible = false;
                }


                if (compatible)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    if (_haveWeUnpackedTextMeshProEssentials && _haveWeUnpackedTextMeshProExtras)
                    {
                        if (!haveWeInstalledLocally &&
                            !haveWeInstalledGlobally) //&& TMP_Pro Essnetials Exists (we really only need to check for TMP folder)... I menan who downloads extras without the essensials..
                        {
                            GUI.enabled = false;
                            if (GUILayout.Button("Unpack TMPro Essentials"))
                            {
                            }

                            GUI.enabled = true;

                            GUI.enabled = false;
                            if (GUILayout.Button("Unpack TMPro Extras"))
                            {
                            }

                            GUI.enabled = true;

                            if (GUILayout.Button("Unpack Smitesoft Essentials"))
                            {
                                EditorScriptSO.RunInstallStage();
                                GetWindow<EditorWindow>().Close();
                                EditorUtility.OpenWithDefaultApp(ManagerPrefabPlusDummyDontClickA); //THIS IS DIFFERENT
                            }
                        }
                        else
                        {
                            GUI.enabled = false;
                            if (GUILayout.Button("Unpack TMPro Essentials"))
                            {
                            }

                            GUI.enabled = true;

                            GUI.enabled = false;
                            if (GUILayout.Button("Unpack TMPro Extras"))
                            {
                            }

                            GUI.enabled = true;

                            if (GUILayout.Button("Unpack Smitesoft Essentials"))
                            {
                                if (!_setupEssentialsCompleted)
                                {                                    
                                        haveWeAddedBlocker = TMPExtensionUtility.CreateTag(TagToAdd);

                                        _setupEssentialsCompleted = true;
                                        InstallationStatus();                                    
                                }


                                EditorScriptSO.RunInstallStage();
                                GetWindow<EditorWindow>().Close();
                                EditorUtility.OpenWithDefaultApp(ManagerPrefabWithoutDummyDontClickB); //THIS IS DIFFERENT
                                EditorScriptSO.SaveInstallationsStatus(); //3.2022 
                            }
                        }
                    }
                    else
                    {
                        if (_haveWeUnpackedTextMeshProEssentials)
                        {
                            GUI.enabled = false;
                            if (GUILayout.Button("Unpack Smitesoft Essentials"))
                            {
                            }

                            GUI.enabled = true;
                            
                            if (GUILayout.Button("Unpack TMPro Extras"))
                            {
                                //this is checking the local dir dll (which you can only see from inside unity) not from windows, so weird
                                if (File.Exists(TextMeshProExamplesDir))
                                {
                                    EditorScriptSO.RunInstallStage();
                                    GetWindow<EditorWindow>().Close();

                                    //This Opens it from the Global dir dll, coz we cant even open it from the local one.. coz its not there! even though we can see it in unity and check for its exisistance    
                                    UnPackTextMeshProExtras();
                                }
                                else
                                {
                                    Debug.Log("Could Not find TMP_Extras to install it, Please install it manually to continue." +
                                              " You can find it under Windows tab => TextMeshPro => Import TMP Extras");
                                }
                            }

                            GUI.enabled = false;
                            if (GUILayout.Button("Unpack Smitesoft Essentials"))
                            {
                            }

                            GUI.enabled = true;
                            
                        }
                        else
                        {
                            if (GUILayout.Button("Unpack TMPro Essentials"))
                            {
                                //this is checking the local dir dll (which you can only see from inside unity) not from windows, so weird
                                if (File.Exists(TextMeshProEssentialsDir))
                                {
                                    EditorScriptSO.RunInstallStage();
                                    GetWindow<EditorWindow>().Close();

                                    //This Opens it from the Global dir dll, coz we cant even open it from the local one.. coz its not there! even though we can see it in unity and check for its exisistance  
                                    UnPackTextMeshProEssentials();
                                }
                                else
                                {
                                    Debug.Log("Could Not find TMP_Essentials to install it, Please install it manually to continue." +
                                              " You can find it under Windows tab => TextMeshPro => Import TMP Essential Resources");
                                }
                            }

                            GUI.enabled = false;
                            if (GUILayout.Button("Unpack TMPro Extras"))
                            {
                            }

                            GUI.enabled = true;

                            GUI.enabled = false;
                            if (GUILayout.Button("Unpack Smitesoft Essentials"))
                            {
                            }

                            GUI.enabled = true;
                        }
                    }


                    if (GUILayout.Button("Apply Changes"))
                    {
                        AssetDatabase.Refresh();
                    }

                    EditorGUILayout.EndVertical();
                }
                else
                {
                    GUI.contentColor = Color.yellow;
                    GUILayout.TextArea(
                        "Incompatible builds: Make sure your using Unity versions 2019 onwards. Make sure your TMP version is compatible," +
                        " you can update you TMP version in Package-Manager. Compatible TMP versions Include:  (2.1.0, 2.1.1, 2.1.3, 3.0.1, 3.0.3). Back-Up project before updating TMP!",
                        300, GUILayout.ExpandHeight(true), GUILayout.MaxHeight(70));
                    GUI.contentColor = Color.white;
                }


                EditorGUILayout.Space();
                GUILayout.Label("Status", EditorStyles.label);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Installation", EditorStyles.textArea);
                if (status == "Completed!")
                {
                    if (TMPExtensionUtility.UnitySkinIsDark)
                    {
                        GUI.contentColor = Color.green;
                        GUILayout.Label(status, EditorStyles.textArea, GUILayout.MaxWidth(200)); //green
                        GUI.contentColor = Color.white;
                    }
                    else
                    {
                        GUI.backgroundColor = newGreenColorForLightTheme;
                        GUILayout.Label(status, EditorStyles.textArea, GUILayout.MaxWidth(200)); //green
                        GUI.backgroundColor = Color.white;
                    }
                }
                else if (status == "Not - Installed")
                {
                    if (TMPExtensionUtility.UnitySkinIsDark)
                    {
                        GUI.contentColor = newRedColorForDarkTheme;
                        GUILayout.Label(status, EditorStyles.textArea, GUILayout.MaxWidth(200)); //red
                        GUI.contentColor = Color.white;
                    }
                    else
                    {
                        GUI.backgroundColor = newRedColorForLightTheme;
                        GUILayout.Label(status, EditorStyles.textArea, GUILayout.MaxWidth(200)); //red
                        GUI.backgroundColor = Color.white;
                    }
                }
                else
                {
                    if (TMPExtensionUtility.UnitySkinIsDark)
                    {
                        GUI.contentColor = Color.yellow;
                        GUILayout.Label(status, EditorStyles.textArea, GUILayout.MaxWidth(200)); //yellow
                        GUI.contentColor = Color.white;
                    }
                    else
                    {
                        GUI.backgroundColor = newYellowColorForLightTheme;
                        GUILayout.Label(status, EditorStyles.textArea, GUILayout.MaxWidth(200)); //yellow
                        GUI.backgroundColor = Color.white;
                    }
                }

                EditorGUILayout.EndHorizontal();


                GUILayout.Space(20);


                _debugging = GUILayout.Toggle(_debugging, " Debugging Mode");

                if (_debugging)
                {
                    EditorGUILayout.Space();
                    GUI.enabled = false;
                    GUILayout.TextArea(
                        "Note: Debugging mode will activate all Logs, It is advised to Turn this mode off, Impotants Logs will remain activ regardless",
                        300, GUILayout.ExpandHeight(true), GUILayout.MaxHeight(50));
                    GUI.enabled = true;
                    EditorGUILayout.Space();


                    GUI.enabled = false;
                    GUI.contentColor = Color.yellow;
                    GUILayout.TextArea(
                        "Warrning!!, Only Delete Cache in a fresh project that does not have this asset fully Installed," +
                        " This asset cannot function without its Cache and it will attempt to reinstall all the cache with " +
                        "a fresh installation of this asset", 300, GUILayout.ExpandHeight(true), GUILayout.MaxHeight(70));
                    GUI.contentColor = Color.white;
                    GUI.enabled = true;

                    EditorGUILayout.Space();

                    GUI.contentColor = Color.yellow;
                    if (InstallationStatus() == "Completed!")
                    {
                        GUI.enabled = false;
                        if (GUILayout.Button("Delete Cache"))
                        {
                            TMPExtensionUtility.DeleteGlobalDllFiles();
                            TMPExtensionUtility.DeleteLocalDllFiles();
                            OnEnableButTriggerAble();
                        }

                        GUI.enabled = true;
                    }
                    else
                    {
                        if (GUILayout.Button("Delete Cache"))
                        {
                            TMPExtensionUtility.DeleteGlobalDllFiles();
                            TMPExtensionUtility.DeleteLocalDllFiles();
                            OnEnableButTriggerAble();
                        }
                    }

                    GUI.contentColor = Color.white;
                }
            }
        }

        private void InstallationStatusDisplay()
        {
            GUILayout.Label("Status", EditorStyles.label);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Installation", EditorStyles.textArea);
            if (status == "Completed!")
            {
                if (TMPExtensionUtility.UnitySkinIsDark)
                {
                    GUI.contentColor = Color.green;
                    GUILayout.Label(status, EditorStyles.textArea, GUILayout.MaxWidth(200)); //green
                    GUI.contentColor = Color.white;
                }
                else
                {
                    GUI.backgroundColor = newGreenColorForLightTheme;
                    GUILayout.Label(status, EditorStyles.textArea, GUILayout.MaxWidth(200)); //green
                    GUI.backgroundColor = Color.white;
                }
            }
            else if (status == "Not - Installed")
            {
                if (TMPExtensionUtility.UnitySkinIsDark)
                {
                    GUI.contentColor = newRedColorForDarkTheme;
                    GUILayout.Label(status, EditorStyles.textArea, GUILayout.MaxWidth(200)); //red
                    GUI.contentColor = Color.white;
                }
                else
                {
                    GUI.backgroundColor = newRedColorForLightTheme;
                    GUILayout.Label(status, EditorStyles.textArea, GUILayout.MaxWidth(200)); //red
                    GUI.backgroundColor = Color.white;
                }
            }
            else
            {
                if (TMPExtensionUtility.UnitySkinIsDark)
                {
                    GUI.contentColor = Color.yellow;
                    GUILayout.Label(status, EditorStyles.textArea, GUILayout.MaxWidth(200)); //yellow
                    GUI.contentColor = Color.white;
                }
                else
                {
                    GUI.backgroundColor = newYellowColorForLightTheme;
                    GUILayout.Label(status, EditorStyles.textArea, GUILayout.MaxWidth(200)); //yellow
                    GUI.backgroundColor = Color.white;
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void SetupEssentials()
        {
            if (!_setupEssentialsCompleted)
            {
                if (GUILayout.Button("Setup-Essential"))
                {
                    haveWeAddedBlocker = TMPExtensionUtility.CreateTag(TagToAdd);

                    _setupEssentialsCompleted = true;
                    GetWindow<EditorWindow>().Close();
                    ShowWindow();
                    InstallationStatus();
                }
            }
            else
            {
                GUI.enabled = false;
                if (GUILayout.Button("Setup-Essential"))
                {
                }

                GUI.enabled = true;
            }
        }

        private void CompatabilityCheckDisplay()
        {
            if (tmpCompatible && unityCompatible) // Unity version check and TMP version Check // or if Installation completed
            {
                compatible = true;
            }
            else
            {
                compatible = false;
            }
        }

        private void TmpVersionDisplay()
        {
            EditorGUILayout.Space();
            GUILayout.Label("TMPro Version", EditorStyles.label);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(TMProVersion, EditorStyles.textArea);
            if (TMProCompatability == "Compatible")
            {
                if (TMPExtensionUtility.UnitySkinIsDark)
                {
                    GUI.contentColor = Color.green;
                    GUILayout.Label(TMProCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                    GUI.contentColor = Color.white;
                }
                else
                {
                    GUI.backgroundColor = newGreenColorForLightTheme;
                    GUILayout.Label(TMProCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                    GUI.backgroundColor = Color.white;
                }
            }
            else
            {
                if (TMPExtensionUtility.UnitySkinIsDark)
                {
                    GUI.contentColor = newRedColorForDarkTheme;
                    GUILayout.Label(TMProCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                    GUI.contentColor = Color.white;
                }
                else
                {
                    GUI.backgroundColor = newRedColorForLightTheme;
                    GUILayout.Label(TMProCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                    GUI.backgroundColor = Color.white;
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void UnityVersionDisplay()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Unity Version", EditorStyles.label);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(UnityVersion, EditorStyles.textArea);
            if (UnityCompatability == "Compatible")
            {
                if (TMPExtensionUtility.UnitySkinIsDark)
                {
                    GUI.contentColor = Color.green;
                    GUILayout.Label(UnityCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                    GUI.contentColor = Color.white;
                }
                else
                {
                    GUI.backgroundColor = newGreenColorForLightTheme;
                    GUILayout.Label(UnityCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                    GUI.backgroundColor = Color.white;
                }
            }
            else
            {
                if (TMPExtensionUtility.UnitySkinIsDark)
                {
                    GUI.contentColor = newRedColorForDarkTheme;
                    GUILayout.Label(UnityCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                    GUI.contentColor = Color.white;
                }
                else
                {
                    GUI.backgroundColor = newRedColorForLightTheme;
                    GUILayout.Label(UnityCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                    GUI.backgroundColor = Color.white;
                }
            }

            EditorGUILayout.EndHorizontal();
        }


#region VersionCheck

        private bool haveWeCheckUnityVersion;
        private string UnityVersion = "Checking";
        private string UnityCompatability = "Checking";

        private void UnityVersionCheck()
        {
            if (haveWeCheckUnityVersion)
            {
                return;
            }

            UnityVersion = Application.unityVersion;
            haveWeCheckUnityVersion = true;
        }


#if UNITY_2019_1_OR_NEWER
        private void CompatibilityCheckUnity()
        {
            unityCompatible = true;
            UnityCompatability = "Compatible";
        }
#else
    private void CompatibilityCheckUnity()
    {
        unityCompatible = false;
        UnityCompatability = "Not-Compatible";
    }
#endif

#region TMProVersionCheck  && Compatibility Check

        private bool haveWeCheckTMPVersion;
        private string TMProVersion = "Checking";
        private string TMProCompatability = "Checking";


        private void VersionCheckTMP()
        {
            if (haveWeCheckTMPVersion)
            {
                return;
            }

#if UNITY_2019_1_OR_NEWER //TODO: I think I should probably drop support for 2019

            TMProVersion = TMPExtensionUtility.TMProVersionLaterThan2019();
            haveWeCheckTMPVersion = true;
#else
            TMProVersion = TMPExtensionUtility.TMProVersionSub2019();            
            haveWeCheckTMPVersion = true;
#endif
        }

        private void CompatibilityCheckTMPro()
        {
            foreach (var version in tmpCompatibleVersionsArray)
            {
                if (version == TMProVersion)
                {
                    tmpCompatible = true;
                    break;
                }
                else
                {
                    tmpCompatible = false;
                }
            }

            if (tmpCompatible)
            {
                TMProCompatability = "Compatible";
            }
            else
            {
                TMProCompatability = "Not-Compatible";
            }
        }

#endregion

#endregion


#region fluff

        private void VideosAndDocumentation()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (
                InstallationStatus() ==
                "Completed!") //TODO: Either Update all this.. or just... Get rid of it.. but since this is installation window, I guess I can do it after the difficult part anyway
            {
                if (GUILayout.Button("Updating Video-Guide"))
                {
                    Application.OpenURL("https://www.youtube.com/watch?v=KR7Ntee0qQM");
                }

                GUI.enabled = false;
                if (GUILayout.Button("Setup Video-Guide"))
                {
                    Application.OpenURL("https://www.youtube.com/watch?v=Sn4z-Knxdu4");
                }

                if (GUILayout.Button("Setup Documentation"))
                {
                    OpenFullReference();
                }

                GUI.enabled = true;
            }
            else
            {
                if (GUILayout.Button("Setup Video-Guide"))
                {
                    Application.OpenURL("https://www.youtube.com/watch?v=Sn4z-Knxdu4");
                }

                if (GUILayout.Button("Setup Documentation"))
                {
                    OpenFullReference();
                }
            }


            EditorGUILayout.EndVertical();
        }


        private static void BannerAndHeaderMethods()
        {
            if (_haveWeUnPackedManagerPrefab)
            {
                EditorGUILayout.Space();
                BannerMethod();

                EditorGUILayout.Space();

                InstallerHeaderA();
            }
            else
            {
                EditorGUILayout.Space();
                BannerMethod();

                EditorGUILayout.Space();
                InstallerHeaderB();
            }
        }

        private static void InstallerHeaderB()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout
                .Space(); //WARNING: Dont use EditorGUILayout.Space(60) or similar spacing here, use multiple spaces not to effect lines verical spacing. this only applies here due to overlap
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (TMPExtensionUtility.UnitySkinIsDark)
            {
                GUILayout.Label("TMP-Searchable : Preparing Dependencies", EditorStyles.boldLabel);
            }
            else
            {
                GUILayout.Label("TMP-Searchable : Preparing Dependencies", EditorStyles.boldLabel);
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(20); //Not sure why I had to add this, this was working perfeclty, and all of a sudden I had to addt his... weird needs further investigation
        }

        private static void InstallerHeaderA()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout
                .Space(); //Dont use EditorGUILayout.Space(60) or similar spacing here, use multiple spaces not to effect lines verical spacing. this only applies here due to overlap
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (TMPExtensionUtility.UnitySkinIsDark) //TODO: well this needs cleaning up
            {
                GUILayout.Label("TMP-Searchable : Integration Installer", EditorStyles.boldLabel);
            }
            else
            {
                GUILayout.Label("TMP-Searchable : Integration Installer", EditorStyles.boldLabel);
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(20); //Not sure why I had to add this, this was working perfeclty, and all of a sudden I had to addt his... weird needs further investigation
        }

        private static void BannerMethod()
        {
            if (
                TMPExtensionUtility
                    .UnitySkinIsDark) //odd way to get skin... EGU.isProSkin is another way.. seems easier too (Maybe its a newer API?) 
            {
                TMPExtensionUtility.ShowBanner(
                    EditorGUIUtility.LoadRequired(
                            "Assets/Smitesoft/Editor Default Resources/Editor banner Dark Theme.png") as
                        Texture); //TODO: Clean this up...can this be cached??..not sure coz its a window
            }
            else
            {
                TMPExtensionUtility.ShowBanner(
                    EditorGUIUtility.LoadRequired(
                        "Assets/Smitesoft/Editor Default Resources/Editor banner Light Theme.png") as Texture); //TODO: Clean this up
            }
        }

        private static void UpdateInstallerUI()
        {
            if (!_haveWeUnPackedManagerPrefab) //small window
            {
                if (!_debugging)
                {
                    GetWindow<InstallationWindow>().minSize = new Vector2(450, 400);
                    GetWindow<InstallationWindow>().maxSize = new Vector2(650, 400);
                }
                else
                {
                    GetWindow<InstallationWindow>().minSize = new Vector2(450, 550);
                    GetWindow<InstallationWindow>().maxSize = new Vector2(650, 550);
                }
            }
            else //Big Window
            {
                if (!_debugging)
                {
                    GetWindow<InstallationWindow>().minSize = new Vector2(450, 600);
                    GetWindow<InstallationWindow>().maxSize = new Vector2(650, 600);
                }
                else
                {
                    GetWindow<InstallationWindow>().minSize = new Vector2(450, 750);
                    GetWindow<InstallationWindow>().maxSize = new Vector2(650, 750);
                }
            }
        }

#endregion


#region TechnicalCode

        private void InstallAll()
        {
            //if we click, we will run the function from the Utility script and close the MenuItem at the same time due to the next function!
            installSuccess = TMPExtensionUtility.InstallDropdownMain();

            SuccessSum(installSuccess);

            installSuccess = TMPExtensionUtility.InstallDropdownMeta();

            SuccessSum(installSuccess);

            installSuccess = TMPExtensionUtility.InstallInputFieldMain();

            SuccessSum(installSuccess);

            installSuccess = TMPExtensionUtility.InstallInputFieldMeta();

            SuccessSum(installSuccess);

            //if we click, we will run the function from the Utility script and close the MenuItem at the same time due to the next function!
            installSuccess = TMPExtensionUtility.InstallDropdownEditorMain();

            SuccessSum(installSuccess);

            installSuccess = TMPExtensionUtility.InstallDropdownEditorMeta();

            SuccessSum(installSuccess);

            installSuccess = TMPExtensionUtility.InstallInputFieldEditorMain();

            SuccessSum(installSuccess);

            installSuccess = TMPExtensionUtility.InstallInputFieldEditorMeta();

            SuccessSum(installSuccess);
        }

        private void SuccessSum(bool Successful)
        {
            installNumberFailed += 1;
            if (Successful)
            {
                successInstallNumber += 1;
            }
            else
            {
                Debug.Log("install: " + installNumberFailed + ",has Failed contact Smitesoft for help on discord");
            }

            Debug.Log("Total Successful Installs: " + successInstallNumber.ToString() + "/8");
        }


        private void UnPackTextMeshProEssentials()
        {
            string globalDirsFirstFraction = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Unity/cache/packages/packages.unity.com"); // This uses using System (not the IO)
            //Debug.Log(globalDirsFirstFraction);//

            string globalDirsCombinedFractions = Path.Combine(globalDirsFirstFraction, "com.unity.textmeshpro" + "@" + TMProVersion);

            string packageresources = Path.Combine(globalDirsCombinedFractions, "Package Resources");

            string[] globalTmpEssentialResourcesFile =
                Directory.GetFiles(packageresources, "TMP Essential Resources.unitypackage", SearchOption.AllDirectories);

            EditorUtility.OpenWithDefaultApp(globalTmpEssentialResourcesFile[0]);
        }


        private void UnPackTextMeshProExtras()
        {
            string globalDirsFirstFraction = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Unity/cache/packages/packages.unity.com"); // This uses using System (not the IO)
            //Debug.Log(globalDirsFirstFraction);//

            string globalDirsCombinedFractions = Path.Combine(globalDirsFirstFraction, "com.unity.textmeshpro" + "@" + TMProVersion);

            string packageresources = Path.Combine(globalDirsCombinedFractions, "Package Resources");

            string[] globalTmpEssentialResourcesFile =
                Directory.GetFiles(packageresources, "TMP Examples & Extras.unitypackage", SearchOption.AllDirectories);

            EditorUtility.OpenWithDefaultApp(globalTmpEssentialResourcesFile[0]);
        }


        private void OpenFullReference() //this is the way to do it, was still able to find documentation even after I moved the file
        {
            string[] dirDocumentationFolder = Directory.GetDirectories("Assets/Smitesoft", "Documentation", SearchOption.AllDirectories);
            string[] dirDocumentationFiles =
                Directory.GetFiles(dirDocumentationFolder[0], "Full Documentation*", SearchOption.AllDirectories);

            if (dirDocumentationFiles.Length > 0)
            {
                EditorUtility.OpenWithDefaultApp(dirDocumentationFiles[0]);
            }
        }

        private bool CheckingManagerPrefab()
        {
            string[] dirManagerFolder = Directory.GetDirectories("Assets/Smitesoft/TMPSearch", "Prefabs", SearchOption.AllDirectories);
            string[] dirManagerFiles = Directory.GetFiles(dirManagerFolder[0], "TMP_Searchable_Manager*", SearchOption.AllDirectories);

            if (dirManagerFiles.Length > 0)
            {
                return true;
            }

            return false;
        }


        private bool CheckingTextoMeshProFile()
        {
            string[] dirTextMeshPro = Directory.GetDirectories("Assets", "TextMesh Pro");

            if (_debugging)
            {
                if (dirTextMeshPro.Length > 0)
                {
                    Debug.Log(dirTextMeshPro[0] + "is found");
                }
                else
                {
                    Debug.Log("TMPro Essencials Not found");
                }
            }

            if (dirTextMeshPro.Length > 0)
            {
                return true;
            }

            return false;
        }

        private bool CheckingTextMeshProExtras()
        {
            string[] dirTextMeshPro = Directory.GetDirectories("Assets", "Examples & Extras", SearchOption.AllDirectories);

            if (_debugging)
            {
                if (dirTextMeshPro.Length > 0)
                {
                    Debug.Log(dirTextMeshPro[0] + "is found");
                }
                else
                {
                    Debug.Log("TMPro Extras Not found");
                }
            }

            if (dirTextMeshPro.Length > 0)
            {
                return true;
            }

            return false;
        }


        public static List<string> searchAbleLocal = new List<string>();
        public static List<string> searchAbleGlobal = new List<string>();

        private bool checkingDllFiles()
        {
            searchAbleLocal.Clear();
            searchAbleGlobal.Clear();


            //Finding TMP Locally==Start
            string[] localDirs = Directory.GetDirectories("Library/PackageCache", "com.unity.textmeshpro*", SearchOption.AllDirectories);
            //Finding TMP Locally==End

            //Local Runtime Directory
            string localRuntimeDirs = Path.Combine(localDirs[0], "Scripts/Runtime");

            string[] localRuntimeFiles = Directory.GetFiles(localRuntimeDirs, "TMP_*", SearchOption.AllDirectories);

            foreach (var file in localRuntimeFiles)
            {
                if (file.Contains("Searchable"))
                {
                    searchAbleLocal.Add(file);
                }
            }

            //Local Editor Directory
            string localEditorDirs = Path.Combine(localDirs[0], "Scripts/Editor");

            string[] localEditorFiles = Directory.GetFiles(localEditorDirs, "TMP_*", SearchOption.AllDirectories);


            foreach (var file in localEditorFiles)
            {
                if (file.Contains("Searchable"))
                {
                    searchAbleLocal.Add(file);
                }
            }


            //Finding TMP Globally==Start
            string globalDirsFirstFraction = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Unity/cache/packages/packages.unity.com"); // This uses using System (not the IO)

            string globalDirsCombinedFractions = Path.Combine(globalDirsFirstFraction, "com.unity.textmeshpro" + "@" + TMProVersion);
            //Finding TMP Globally==End


            //Global Runtime Directory
            globalRuntimeDirs = Path.Combine(globalDirsCombinedFractions, "Scripts/Runtime");

            string[] globalRuntimeFiles = Directory.GetFiles(globalRuntimeDirs, "TMP_*", SearchOption.AllDirectories);

            foreach (var file in globalRuntimeFiles)
            {
                if (file.Contains("Searchable"))
                {
                    searchAbleGlobal.Add(file);
                }
            }

            //Global Editor Directory
            globalEditorDirs = Path.Combine(globalDirsCombinedFractions, "Scripts/Editor");

            string[] globalEditorFiles = Directory.GetFiles(globalEditorDirs, "TMP_*", SearchOption.AllDirectories);

            foreach (var file in globalEditorFiles)
            {
                if (file.Contains("Searchable"))
                {
                    searchAbleGlobal.Add(file);
                }
            }


            if (_debugging)
            {
                Debug.Log("TMP_Searchable Local Cashe file count = " + searchAbleLocal.Count);
            }

            foreach (var item in searchAbleLocal) //This is where I check if they exist and delete them, and then force reload/refresh
            {
                if (_debugging)
                {
                    Debug.Log(item);
                }
            }


            if (_debugging)
            {
                Debug.Log("TMP_Searchable Global Cashe file count = " + searchAbleGlobal.Count);
            }

            foreach (var item in searchAbleGlobal) //This is where I check if they exist and delete them, and then force reload/refresh
            {
                if (_debugging)
                {
                    Debug.Log(item);
                }
            }

            if (searchAbleLocal.Count > 0)
            {
                return true;
            }

            return false;
        }

#endregion


        //==========NOTES============

        //private void OnValidate()
        //{
        //    showWindow();
        //    Debug.Log("OnValidate");
        //}

        //private void OnProjectChange()
        //{
        //    showWindow();
        //    Debug.Log("OnProjectChange");
        //}


        //static void Update()  //too many updates
        //{
        //}
    }
}