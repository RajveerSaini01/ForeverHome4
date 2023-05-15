using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public GameObject instructionsObject;
   public GameObject mainMenuObject;

   private void Awake()
   {
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
   }

   public void PlayGame()
   {
      PlayerPrefs.SetInt("Waves", 0);
      PlayerPrefs.SetInt("Kills", 0);
      SceneManager.LoadScene("HomeWorld");
   }
   
   public void QuitGame()
   {
      Application.Quit();
      Debug.Log("Quit");
   }

   public void Credits()
   {
      SceneManager.LoadScene("Credits");
      Debug.Log("credits");
   }
   
   public void Options()
   {
      SceneManager.LoadScene("Options");
      Debug.Log("options");
   }
   
   public void Instructions()
   {
      // Disable the main menu object and all its children
      mainMenuObject.SetActive(false);
      
      // Enable the instructions object and all its children
      instructionsObject.SetActive(true);
   }
   
   public void InstructionsBack()
   {
      // Disable the instructions object and all its children
      instructionsObject.SetActive(false);
      
      // Enable the main menu object and all its children
      mainMenuObject.SetActive(true);
   }
   
   public void mainmenu()
   {
      SceneManager.LoadScene("MainMenu");
      Debug.Log("Mainmenu");
   } 
}
