using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public static Action SpacebarPressed;
    public static Action<UnitScript> UnitClicked;

    public static GameManager instance;

    public Camera mainCamera;
    public UnitScript selectedUnit;

    public List<UnitScript> units = new List<UnitScript>();

    // Text panels for each interactive object
    public GameObject chestTextPanel;
    public GameObject barrelTextPanel;
    public GameObject notebookTextPanel;
    public GameObject bookshelfTextPanel;

    // Slider for handle movement
    public Slider handleSlider;

    // HashSet to track clicked objects
    private HashSet<string> clickedObjects;

    // Total number of interactive objects
    private int totalInteractableObjects;

    // Proximity range
    public float interactionRange = 5f;

    // Door input panel and winner panel
    public GameObject doorInputPanel;
    public GameObject winnerPanel;

    // Door input field
    public TMP_InputField doorInputField;

    // Distance message
    public TMP_Text distanceMessage;

    LayerMask layerMask;

    void OnEnable() 
    {
        if (GameManager.instance == null) {
            GameManager.instance = this;
        } else {
            Destroy(this);
        }
    }

    void Start()
    {
        layerMask = LayerMask.GetMask("ground", "unit", "Interactable");

        // Ensure all panels are initially inactive
        chestTextPanel.SetActive(false);
        barrelTextPanel.SetActive(false);
        notebookTextPanel.SetActive(false);
        bookshelfTextPanel.SetActive(false);
        doorInputPanel.SetActive(false);
        winnerPanel.SetActive(false);

        // Initialize the HashSet to track clicked objects
        clickedObjects = new HashSet<string>();

        // Set the slider's maximum value to the total number of interactive objects
        totalInteractableObjects = 4; // Example: chest, barrel, notebook, bookshelf
        handleSlider.maxValue = totalInteractableObjects;
        handleSlider.value = 0;

        // Hide the distance message at start
        if (distanceMessage != null)
        {
            distanceMessage.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            SpacebarPressed?.Invoke();
        }

        if (Input.GetMouseButtonDown(0)) {
            Ray mousePositionRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(mousePositionRay, out hitInfo, Mathf.Infinity, layerMask)) 
            {
                if (hitInfo.collider.CompareTag("ground")) {
                    if (selectedUnit != null) {
                        selectedUnit.nma.SetDestination(hitInfo.point);
                    }
                } else if (hitInfo.collider.CompareTag("unit")) {
                    SelectUnit(hitInfo.collider.gameObject.GetComponent<UnitScript>());
                } else {
                    HandleObjectClick(hitInfo.collider);
                }
            }
        }
    }

    void HandleObjectClick(Collider clickedObject)
    {
        // Check if the unit is within interaction range
        float distanceToUnit = Vector3.Distance(selectedUnit.transform.position, clickedObject.transform.position);
        if (distanceToUnit > interactionRange)
        {
            // Show the distance message if not close enough
            if (distanceMessage != null)
            {
                StartCoroutine(DisplayDistanceMessage("You are not close enough to the object!"));
            }
            return;
        }

        // Hide all panels before showing the correct one
        chestTextPanel.SetActive(false);
        barrelTextPanel.SetActive(false);
        notebookTextPanel.SetActive(false);
        bookshelfTextPanel.SetActive(false);

        // Show the relevant panel based on the clicked object's tag
        if (clickedObject.CompareTag("Chest")) {
            chestTextPanel.SetActive(true);
        } else if (clickedObject.CompareTag("Barrel")) {
            barrelTextPanel.SetActive(true);
        } else if (clickedObject.CompareTag("Notebook")) {
            notebookTextPanel.SetActive(true);
        } else if (clickedObject.CompareTag("Bookshelf")) {
            bookshelfTextPanel.SetActive(true);
        } else if (clickedObject.CompareTag("Door")) {
            doorInputPanel.SetActive(true); // Show the door input panel when the player clicks the door
        }

        // Get the object name
        string objectName = clickedObject.gameObject.name;

        // Increment progress only if the object hasn't been clicked before
        if (!clickedObjects.Contains(objectName))
        {
            clickedObjects.Add(objectName);
            handleSlider.value = clickedObjects.Count; // Update slider value
        }
    }

    IEnumerator DisplayDistanceMessage(string message)
    {
        // Display the message
        distanceMessage.text = message;
        distanceMessage.gameObject.SetActive(true);

        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);

        // Hide the message
        distanceMessage.gameObject.SetActive(false);
    }

    public void SubmitDoorCode()
    {
        // Check if the entered code is "777"
        if (doorInputField.text == "710")
        {
            doorInputPanel.SetActive(false); // Close the door input panel
            winnerPanel.SetActive(true); // Show the winner panel
        }
    }

    public void SelectUnit(UnitScript unit)
    {
        UnitClicked?.Invoke(unit);
        selectedUnit = unit;
    }

    public void CloseAllTextPanels()
    {
        // Method to close all text panels, can be used by UI buttons
        chestTextPanel.SetActive(false);
        barrelTextPanel.SetActive(false);
        notebookTextPanel.SetActive(false);
        bookshelfTextPanel.SetActive(false);
    }

    public void RestartGame()
    {
        // Reload the currently active scene to restart the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
