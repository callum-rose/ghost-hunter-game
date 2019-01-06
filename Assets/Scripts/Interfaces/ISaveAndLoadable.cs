using UnityEngine;
public interface ISaveAndLoadable {

    void SaveData();

    void LoadData();

    void OnLoadDataFailed();
}
