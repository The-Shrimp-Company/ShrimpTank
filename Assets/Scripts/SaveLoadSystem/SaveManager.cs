#pragma warning disable CS0162 // Unreachable code detected
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using System.Text.Json;

namespace SaveLoadSystem
{
    public static class SaveManager
    {
        public static SaveData CurrentSaveData = new SaveData();

        public const string directory = "/SaveGames/";
        public const string fileNameSuffix = ".save";
        public const string fileIntegrityChecker = "Pikselere";

        private const bool createBackupFile = true;  // Whether the game should create extra backup save files incase something goes wrong
        private const bool getLastPlayedTimeInUTC = false;  // Whether the last played time for a save file should be shown in universal standard time
        private const bool debugSaving = true;  // Whether the saving and loading should output extra messages
        private const bool copyPathToClipboard = false;  // Whether the path to the save file should be copied to your clipboard when the game saves

        public static bool startNewGame = false;  // Whether the game should start a new file on load
        public static string currentSaveFile = null;  // The name of the loaded file
        public static bool currentlySaving = false;  // If the game is saving right now
        public static bool loadingGameFromFile = false;  // Whether the game is loading from a file or starting a new one
        public static bool gameInitialized = false;  // If the game has finished loading, whether that is from a file or a new game

        public static UnityAction OnLoadGameStart;
        public static UnityAction OnLoadGameFinish;



        public static bool SaveGame(string _fileName)
        {
            string dir = Application.persistentDataPath + directory;
            string file = dir + _fileName + fileNameSuffix;
            string backupFile = dir + _fileName + "Backup" + fileNameSuffix;

            CurrentSaveData.fileIntegrityCheck = fileIntegrityChecker;


            if (!Directory.Exists(dir))  // If the directory does not exist
                Directory.CreateDirectory(dir);  // Create this directory

            string json = JsonSerializer.Serialize(CurrentSaveData, new JsonSerializerOptions() { IncludeFields = true, WriteIndented = true });  // Convert the save to json format


            if (createBackupFile && File.Exists(file))
            {
                if (File.Exists(backupFile))  // If save and backup both exist
                {
                    string jsonBackup = File.ReadAllText(backupFile);
                    if (json != jsonBackup)  // Check if the save is the same as the backup
                    {
                        File.Delete(backupFile);  // If they are different, delete backup and make a new one
                        File.Copy(file, backupFile);
                    }
                }
                else  // If save exists but backup does not
                {
                    File.Copy(file, backupFile);
                }
            }


            File.WriteAllText(file, json);  // Write the save to the file


            if (createBackupFile)
            {
                if (TryLoadGame(_fileName))
                {
                    File.Delete(backupFile);
                    File.Copy(file, backupFile);
                }
            }


            if (copyPathToClipboard)
                GUIUtility.systemCopyBuffer = file;  // Copies the path to your clipboard


            if (debugSaving && _fileName != "Autosave") Debug.Log("Game Saved to " + file);
            

            return true;  // Success
        }


        public static void NewGame()
        {
            OnLoadGameStart?.Invoke();
            loadingGameFromFile = false;
            OnLoadGameFinish?.Invoke();
            gameInitialized = true;

            if (debugSaving) Debug.Log("New Game Started");
        }


        public static void LoadGame(string _fileName)
        {
            OnLoadGameStart?.Invoke();
            string fullPath = Application.persistentDataPath + directory + _fileName + fileNameSuffix;
            string backupPath = Application.persistentDataPath + directory + _fileName + "Backup" + fileNameSuffix;
            SaveData tempData = new SaveData();
            bool tryBackup = false;

            if (File.Exists(fullPath))
            {
                string json = File.ReadAllText(fullPath);
                tempData = JsonSerializer.Deserialize<SaveData>(json, new JsonSerializerOptions() { IncludeFields = true });

                if (tempData.fileIntegrityCheck == fileIntegrityChecker)
                {
                    loadingGameFromFile = true;
                    if (debugSaving) Debug.Log("Game Loaded from " + fullPath);
                }
                else
                {
                    Debug.LogError("Save file at " + fullPath + " has been modified or corrupted");
                    tempData = new SaveData();
                    tryBackup = true;
                }
            }
            else
            {
                Debug.LogWarning("Save file at " + fullPath + " does not exist");
                tryBackup = true;
            }

            if (tryBackup)
            {
                if (File.Exists(backupPath))
                {
                    string json = File.ReadAllText(backupPath);
                    tempData = JsonSerializer.Deserialize<SaveData>(json, new JsonSerializerOptions() { IncludeFields = true });

                    if (tempData.fileIntegrityCheck == fileIntegrityChecker)
                    {
                        loadingGameFromFile = true;
                        if (debugSaving) Debug.Log("Game Backup Loaded from " + backupPath);
                    }
                    else
                    {
                        Debug.LogError("Save file backup at " + backupPath + " has been modified or corrupted");
                        tempData = new SaveData();
                    }
                }

                else
                {
                    Debug.LogWarning("Save file backup at " + backupPath + " does not exist");
                    loadingGameFromFile = false;
                }
            }

            tempData.playerStats.timesGameLoaded++;
            CurrentSaveData = tempData;
        }


        public static bool TryLoadGame(string _fileName)
        {
            string fullPath = Application.persistentDataPath + directory + _fileName + fileNameSuffix;
            SaveData tempData = new SaveData();

            if (File.Exists(fullPath))
            {
                string json = File.ReadAllText(fullPath);
                tempData = JsonSerializer.Deserialize<SaveData>(json, new JsonSerializerOptions() { IncludeFields = true });

                if (tempData.fileIntegrityCheck == fileIntegrityChecker)
                {
                    //if (debugSaving) Debug.Log("Game can be loaded from " + fullPath);

                    return true;
                }
                else
                {
                    Debug.Log("Game file at " + fullPath + " has been modified or corrupted");

                    return false;
                }
            }

            else
            {
                Debug.LogError("Save file at " + fullPath + " does not exist");

                return false;
            }
        }


        public static void DeleteSave(string _fileName)
        {
            string fullPath = Application.persistentDataPath + directory + _fileName + fileNameSuffix;
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);

                if (debugSaving) Debug.Log("File Deleted at " + fullPath);
            }
        }


        public static void OpenSaveFolder()
        {
            string dir = Application.persistentDataPath + directory;
            System.Diagnostics.Process.Start(dir);
        }


        public static DateTime GetSaveLastPlayedDate(string _fileName)
        {
            DateTime dt;

            if (!getLastPlayedTimeInUTC)
                dt = File.GetLastWriteTime(Application.persistentDataPath + directory + _fileName + fileNameSuffix);
            else
                dt = File.GetLastWriteTimeUtc(Application.persistentDataPath + directory + _fileName + fileNameSuffix);

            return dt;
        }
    }
}

#pragma warning restore CS0162 // Unreachable code detected