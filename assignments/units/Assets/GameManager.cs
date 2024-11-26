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

    public GameObject chestTextPanel;
    public GameObject barrelTextPanel;
    public GameObject notebookTextPanel;
    public GameObject bookshelfTextPanel;

    public Slider handleSlider;

    private HashSet<string> clickedObjects;

    private int totalInteractableObjects;

    public float interactionRange = 5f;

    public GameObject doorInputPanel;
    public GameObject winnerPanel;
    public GameObject gameOverPanel;

    public TMP_InputField doorInputField;

    public TMP_Text distanceMessage;
    public TMP_Text livesText;

    private int lives = 3;
    LayerMask layerMask;

    void OnEnable()
    {
        if (GameManager.instance == null)
        {
            GameManager.instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        layerMask = LayerMask.GetMask("ground", "unit", "Interactable");

        chestTextPanel.SetActive(false);
        barrelTextPanel.SetActive(false);
        notebookTextPanel.SetActive(false);
        bookshelfTextPanel.SetActive(false);
        doorInputPanel.SetActive(false);
        winnerPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        clickedObjects = new HashSet<string>();

        totalInteractableObjects = 4;
        handleSlider.maxValue = totalInteractableObjects * 40;
        handleSlider.value = 0;

        livesText.text = $"Lives: {lives}";

        if (distanceMessage != null)
        {
            distanceMessage.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpacebarPressed?.Invoke();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray mousePositionRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(mousePositionRay, out hitInfo, Mathf.Infinity, layerMask))
            {
                if (hitInfo.collider.CompareTag("ground"))
                {
                    if (selectedUnit != null)
                    {
                        selectedUnit.nma.SetDestination(hitInfo.point);
                    }
                }
                else if (hitInfo.collider.CompareTag("unit"))
                {
                    SelectUnit(hitInfo.collider.gameObject.GetComponent<UnitScript>());
                }
                else
                {
                    HandleObjectClick(hitInfo.collider);
                }
            }
        }
    }

    void HandleObjectClick(Collider clickedObject)
    {
        float distanceToUnit = Vector3.Distance(selectedUnit.transform.position, clickedObject.transform.position);
        if (distanceToUnit > interactionRange)
        {
            if (distanceMessage != null)
            {
                StartCoroutine(DisplayDistanceMessage("You are not close enough to the object!"));
            }
            return;
        }

        chestTextPanel.SetActive(false);
        barrelTextPanel.SetActive(false);
        notebookTextPanel.SetActive(false);
        bookshelfTextPanel.SetActive(false);

        if (clickedObject.CompareTag("Chest"))
        {
            chestTextPanel.SetActive(true);
        }
        else if (clickedObject.CompareTag("Barrel"))
        {
            barrelTextPanel.SetActive(true);
        }
        else if (clickedObject.CompareTag("Notebook"))
        {
            notebookTextPanel.SetActive(true);
        }
        else if (clickedObject.CompareTag("Bookshelf"))
        {
            bookshelfTextPanel.SetActive(true);
        }
        else if (clickedObject.CompareTag("Door"))
        {
            doorInputPanel.SetActive(true);
            handleSlider.value = handleSlider.maxValue;
            return;
        }

        string objectName = clickedObject.gameObject.name;

        if (clickedObjects.Contains(objectName))
        {
            handleSlider.value -= 40;
            clickedObjects.Remove(objectName);
        }
        else
        {
            clickedObjects.Add(objectName);
            handleSlider.value += 40;
        }
    }

    IEnumerator DisplayDistanceMessage(string message)
    {
        distanceMessage.text = message;
        distanceMessage.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        distanceMessage.gameObject.SetActive(false);
    }

    public void SubmitDoorCode()
    {
        if (doorInputField.text == "710")
        {
            doorInputPanel.SetActive(false);
            winnerPanel.SetActive(true);
        }
        else
        {
            lives -= 1;
            livesText.text = $"Lives: {lives}";

            if (lives <= 0)
            {
                gameOverPanel.SetActive(true);
                gameOverPanel.GetComponentInChildren<TMP_Text>().text = "YOU DIED";
                doorInputPanel.SetActive(false);
            }
        }
    }

    public void SelectUnit(UnitScript unit)
    {
        UnitClicked?.Invoke(unit);
        selectedUnit = unit;
    }

    public void CloseAllTextPanels()
    {
        chestTextPanel.SetActive(false);
        barrelTextPanel.SetActive(false);
        notebookTextPanel.SetActive(false);
        bookshelfTextPanel.SetActive(false);
        doorInputPanel.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
