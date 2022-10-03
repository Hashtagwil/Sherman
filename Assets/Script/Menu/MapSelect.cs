using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelect : MonoBehaviour
{
    [SerializeField] string Map;
    [SerializeField] GameObject SelectedImage;

    public void OnMouseUp()
    {
        DataPersistence.Instance.data.Map = Map;
        DataPersistence.Instance.SaveData();
        DataPersistence.Instance.SendGameSetting();
    }

    private void Update()
    {
        SelectedImage.SetActive(DataPersistence.Instance.data.Map == Map);
    }
}
