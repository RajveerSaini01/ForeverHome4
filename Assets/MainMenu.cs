using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public void PlayGame()
   {
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
   public void mainmenu()
   {
      SceneManager.LoadScene("MainMenu");
      Debug.Log("Mainmenu");
   }
   
  
}
