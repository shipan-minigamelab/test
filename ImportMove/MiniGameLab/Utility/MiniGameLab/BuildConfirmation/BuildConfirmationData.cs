using System;
using NaughtyAttributes;
using UnityEngine;

// [CreateAssetMenu(menuName = "Data/BuildConfirmationData", fileName = "BuildConfirmationData")]
public class BuildConfirmationData : ScriptableObject
{
	[SerializeField] private AdvancedData _advancedData;
	private const string _buildConfirmationDataKey = "BuildConfirmationData";

	public bool CheckIfShouldDeleteUserData()
	{
		// LOAD ADVANCED DATA TO TEMP
		if (BinarySerializer.HasSaved(_buildConfirmationDataKey))
		{
			AdvancedData tempData = BinarySerializer.Load<AdvancedData>(_buildConfirmationDataKey);
			_advancedData.SavedDataVersionUser = tempData.SavedDataVersionUser;
			if (_advancedData.SavedDataVersionFromBuild > _advancedData.SavedDataVersionUser)
			{
				// SHOULD DELETE USER'S DATA
				_advancedData.SavedDataVersionUser = _advancedData.SavedDataVersionFromBuild;
				BinarySerializer.Save(_advancedData,_buildConfirmationDataKey);
			}
			else
			{
				return false;
			}
		}

		_advancedData.SavedDataVersionUser = _advancedData.SavedDataVersionFromBuild;
		BinarySerializer.Save(_advancedData,_buildConfirmationDataKey);
		return true;
	}

	public void ChangeSaveDataUserVersion()
	{
		_advancedData.SavedDataVersionUser = _advancedData.SavedDataVersionFromBuild;
		BinarySerializer.Save(_advancedData,_buildConfirmationDataKey);
	}

	public void IncrementSaveBuildVersion() => _advancedData.SavedDataVersionFromBuild += 1;

	public void DecrementSaveBuildVersion() => _advancedData.SavedDataVersionFromBuild -= 1;

	[Button("\n[USE WITH CAUTION !!]\n\n" +
	        "[Delete Saved User Data Version]\n")]
	private void DeleteSaveVersionKey()
	{
		BinarySerializer.DeleteDataFile(_buildConfirmationDataKey);
		_advancedData.SavedDataVersionUser = 0;
		_advancedData.SavedDataVersionFromBuild = 0;
	}


	// public static void DeleteAllData()
	// {
	// 	CommonStuffs.OnResetEvent?.Invoke();
	// 	PlayerPrefs.DeleteAll();
	// 	Debug.Log("DATA CLEARED");
	// }

	public int GetSavedDataVersionUser() => _advancedData.SavedDataVersionUser;
	public int GetSavedDataVersionFromBuild() => _advancedData.SavedDataVersionFromBuild;
}

[Serializable]
public struct AdvancedData
{
	[AllowNesting] [ReadOnly] public int SavedDataVersionUser;
	[AllowNesting] [ReadOnly] public int SavedDataVersionFromBuild;
}