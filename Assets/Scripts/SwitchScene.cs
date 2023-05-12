using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    public string sceneName;
    public float activationDistance = 2f;
    public Camera playerCamera;

    void Awake()
    {
        InteractHandler.OnInteract += OnInteract;
    }

    void OnInteract(string caller)
    {
        if (caller == gameObject.name)
        {
            SceneManager.LoadScene("PsychoWorld");
        }
    }
    /*
    void Update()
    {
        // Check if the player is within the activation distance
        if (Vector3.Distance(transform.position, GameObject.Find("FirstPersonController(Clone)").transform.position) <= activationDistance)
        {
            // Check if the player is looking at the object
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
              
                if (Input.GetKeyDown(KeyCode.E))
                {
                    
                    SceneManager.LoadScene(sceneName);
                }
            }
        }
    }
    */
}