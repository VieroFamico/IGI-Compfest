using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManger : MonoBehaviour
{
    public Animator panelAnimator; // Reference to the Animator component on the panel
    public CinemachineVirtualCamera virtualCamera; // Reference to the Cinemachine Virtual Camera
    public Transform playerTransform;
    public Button startButton;
    public float duration = 1.0f; // Duration for the offset change

    private GameObject currentCamTarget;
    private Vector3 initialOffset;
    private Vector3 targetOffset;

    private void Start()
    {
        currentCamTarget = new GameObject();
        currentCamTarget.transform.position = new Vector3 (0, 0, -40f);

        virtualCamera.m_Follow = currentCamTarget.transform;
        // Ensure the button has an onClick event listener
        startButton.onClick.AddListener(OnButtonPressed);
    }

    private void OnButtonPressed()
    {
        // Trigger the animation on the panel
        if (panelAnimator != null)
        {
            panelAnimator.SetTrigger("Close");
        }
        else
        {
            Debug.LogWarning("Panel Animator is not assigned!");
        }

        virtualCamera.m_Follow = playerTransform;
    }

}
