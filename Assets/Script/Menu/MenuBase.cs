using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuBase : MonoBehaviour
{
    [SerializeField] protected GameObject main;
    [SerializeField] protected GameObject options;
    [SerializeField] protected GameObject tutorial;

    protected GameObject currentMenu;

    private void Awake()
    {
        Time.timeScale = 1;
    }

    private void Start()
    {
        DataPersistence.Instance.LoadData();
        AudioListener.volume = DataPersistence.Instance.data.Volume;
        DataPersistence.Instance.CurrentMenu = DataPersistence.Menus.MAIN;
        currentMenu = main;
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null && Input.GetAxis("Vertical") != 0)
        {                
            FocusFirstElementInCurrentMenu();
        }
    }

    public void Options()
    {
        DataPersistence.Instance.CurrentMenu = DataPersistence.Menus.OPTIONS;
        main.SetActive(false);
        options.SetActive(true);
        currentMenu = options;
        FocusFirstElementInCurrentMenu();
    }

    public void Tutorial()
    {
        DataPersistence.Instance.CurrentMenu = DataPersistence.Menus.TUTORIAL;
        main.SetActive(false);
        tutorial.SetActive(true);
        currentMenu = tutorial;
        FocusFirstElementInCurrentMenu();
    }

    public void Back()
    {
        DataPersistence.Instance.CurrentMenu = DataPersistence.Menus.MAIN;
        options.SetActive(false);
        tutorial.SetActive(false);
        main.SetActive(true);
        currentMenu = main;
        FocusFirstElementInCurrentMenu();
    }

    protected void FocusFirstElementInCurrentMenu()
    {
        EventSystem.current.SetSelectedGameObject(currentMenu.transform.GetChild(0).gameObject);
    }

    internal void RestoreCurrentMenu()
    {
        if (DataPersistence.Instance.CurrentMenu == DataPersistence.Menus.OPTIONS)
        {
            Options();
        }
    }
}
