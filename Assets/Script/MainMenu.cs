using System;
using TriInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private AdvancedButton firstSelected;

    [Scene, SerializeField]
    private string loadedScene;

    [SerializeField]
    private ManagerRefs managerRefs;

    private void Start()
    {
        if (managerRefs.InputManager.IsGamepad)
            EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);

        managerRefs.InputManager.OnInputDeviceChanged += OnInputDeviceChanged;
    }

    private void OnDestroy()
    {
        managerRefs.InputManager.OnInputDeviceChanged -= OnInputDeviceChanged;
    }

    private void OnInputDeviceChanged()
    {
        if (managerRefs.InputManager.IsGamepad)
            EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(loadedScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
