using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractHandler : MonoBehaviour
{
    public static event Action<string> OnInteract;
    private Camera cam;

    [SerializeField] private GameObject target;
    [SerializeField] private GameObject interactableComponent;
    [SerializeField] private GameObject hoverCanvas;
    [SerializeField] private TextMeshProUGUI hoverText;
    
    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {

        RaycastHit hit;
        
        
        // Physics.SphereCast(p1, 0.2f, ray.direction, out hit, 2f)
        Ray ray = cam.ScreenPointToRay(new (Screen.width/2f, Screen.height/2f, 0f));
        if (Physics.Raycast(ray, out hit,3f))
        {
            target = hit.collider.gameObject;
            interactableComponent = GetInteractableObjectOrParent(target);
            
            Debug.DrawLine(ray.origin, hit.point);
            //hoverCanvas.transform.SetParent(target.transform, true);
            hoverCanvas.transform.position = hit.point;
            hoverCanvas.transform.LookAt(cam.transform.position);
            hoverCanvas.transform.Translate(0.01f * Vector3.forward);
            hoverCanvas.transform.Rotate(0, 180, 0, Space.Self);
           
            

        }
        else
        {
            hoverText.text = "";
            hoverCanvas.SetActive(false);
            target = null;
            interactableComponent = null;
        }

        if (interactableComponent != null)
        {
            hoverCanvas.SetActive(true);
            hoverText.text = $"{interactableComponent.name} [E]";
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Callingf: " + interactableComponent.name);
                OnInteract?.Invoke(interactableComponent.name);
            }
        }
        else
        {
            hoverText.text = "";
            hoverCanvas.SetActive(false);
        }
    }

    GameObject GetInteractableObjectOrParent(GameObject target)
    {
        if (target.CompareTag("Interactable"))
        {
            return target;
        }

        Transform myself = target.transform;
        while (myself.parent != null)
        {
            myself = myself.parent;
            if (myself.CompareTag("Interactable"))
            {
                return myself.gameObject;
            }
        }
        return null;
    }
}
