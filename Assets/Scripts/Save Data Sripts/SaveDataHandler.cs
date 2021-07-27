using System.IO;
using UnityEngine;

/// <summary>
/// Handles saving and loading of game data.
/// </summary>
public static class SaveDataHandler
{
    /// <summary>
    /// Loads existing save data.
    /// </summary>
    public static void Load()
    {
        string _path = Application.persistentDataPath + Path.DirectorySeparatorChar + "SaveData" + Path.DirectorySeparatorChar;

        string _saveDataJSON = File.ReadAllText(_path + "savedata.txt");

        SaveDataScriptable _saveData = ScriptableObject.CreateInstance<SaveDataScriptable>();

        JsonUtility.FromJsonOverwrite(_saveDataJSON, _saveData);

        SaveData.SavedDateTime = _saveData.SavedDateTime;
        SaveData.SavedInvasionTimeRemaining = _saveData.SavedInvasionTimeRemaining;
        SaveData.TotalMoney = _saveData.TotalMoney;
        SaveData.PlacedBuildings = _saveData.PlacedBuildings;
        SaveData.PlacedRoads = _saveData.PlacedRoads;
        SaveData.CurrentInvasionState = _saveData.CurrentInvasionState;
        SaveData.TotalSoldiers = _saveData.TotalSoldiers;
        SaveData.SoldiersNeeded = _saveData.SoldiersNeeded;
        SaveData.SoundOn = _saveData.SoundOn;

        Debug.Log("Game Loaded!");
    }

    /// <summary>
    /// Saves all data in it's current form.
    /// </summary>
    public static void Save()
    {
        SaveDataScriptable _saveData = ScriptableObject.CreateInstance<SaveDataScriptable>();

        _saveData.SavedDateTime = SaveData.SavedDateTime;
        _saveData.SavedInvasionTimeRemaining = SaveData.SavedInvasionTimeRemaining;
        _saveData.TotalMoney = SaveData.TotalMoney;
        _saveData.PlacedBuildings = SaveData.PlacedBuildings;
        _saveData.PlacedRoads = SaveData.PlacedRoads;
        _saveData.CurrentInvasionState = SaveData.CurrentInvasionState;
        _saveData.TotalSoldiers = SaveData.TotalSoldiers;
        _saveData.SoldiersNeeded = SaveData.SoldiersNeeded;
        _saveData.SoundOn = SaveData.SoundOn;

        string _saveJSON = JsonUtility.ToJson(_saveData);
        string _path = Application.persistentDataPath + Path.DirectorySeparatorChar + "SaveData" + Path.DirectorySeparatorChar;

        File.WriteAllText(_path + "savedata.txt", _saveJSON);

        Debug.Log("Game Saved!");
    }
}