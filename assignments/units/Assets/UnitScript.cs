using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitScript : MonoBehaviour
{

    public string unitName;
    public string bio;
    public string stats;

    public NavMeshAgent nma;

    public GameObject wallSeeingSphere;

    public Renderer bodyRenderer;
    public Color normalColor;
    public Color selectedColor;

    public Vector3 destination;

    public bool selected = false;

    float rotateSpeed;

    LayerMask layerMask;

    void OnEnable()
    {
        GameManager.SpacebarPressed += ChangeToRandomColor;
        GameManager.UnitClicked += GameManagerSaysUnitWasClicked;
    }

    void OnDisable()
    {
        GameManager.SpacebarPressed -= ChangeToRandomColor;
        GameManager.UnitClicked -= GameManagerSaysUnitWasClicked;
    }



    void ChangeToRandomColor()
    {
        bodyRenderer.material.color = new Color(Random.value, Random.value, Random.value);
    }

    void GameManagerSaysUnitWasClicked(UnitScript unit) {
        if (unit == this) {
            selected = true;
            bodyRenderer.material.color = selectedColor;

        } else {
            selected = false;
            bodyRenderer.material.color = normalColor;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        layerMask = LayerMask.GetMask("wall");

        GameManager.instance.units.Add(this);

        rotateSpeed = Random.Range(20, 60);

        transform.Rotate(0, Random.Range(0, 360), 0);
    }

    void OnDestroy()
    {
        GameManager.instance.units.Remove(this);
    }



    // Update is called once per frame
    void Update()
    {
        // if (destination != null) {
        //     Vector3 direction = destination - transform.position;
        //     direction.Normalize();
        //     transform.position += direction * 5 * Time.deltaTime;
        // }

        // transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);

        // Vector3 rayStart = transform.position + Vector3.up * 1.75f;

        // Color rayColor = Color.red;
        // RaycastHit hit;
        // if (Physics.Raycast(rayStart, transform.forward, out hit, Mathf.Infinity, layerMask))
        // {
            // We hit something!
            // if (hit.collider.CompareTag("wall")) {
        //         wallSeeingSphere.SetActive(true);
        //         wallSeeingSphere.transform.position = hit.point;
        // //     } else if (hit.collider.CompareTag("unit")) {
        //         wallSeeingSphere.SetActive(false);
        //         if (hit.collider.gameObject.GetComponent<UnitScript>().unitName == "Mew Mew") {
        //             rayColor = Color.blue;
        //         } else {
        //             rayColor = Color.white;
        //         }
        //     } else {
        //         wallSeeingSphere.SetActive(false);
        //     }
        // } else {
        //     wallSeeingSphere.SetActive(false);
        // }
        // Debug.DrawRay(rayStart, transform.forward * 4, rayColor);
    }

    // void OnMouseDown() {
    //     // GameObject gmObj = GameObject.Find("GameManagerObject");
    //     // GameManager gm = gmObj.GetComponent<GameManager>();

    //     GameManager.instance.SelectUnit(this);
    // }
}
