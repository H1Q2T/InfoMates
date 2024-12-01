using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FuncionalidadBotonesInicio : MonoBehaviour
{

    public void BotonJugar()
    {
        SceneManager.LoadScene("SeleccionJuego");
    }
    public void BotonPersonalizar()
    {
        SceneManager.LoadScene("Personalizar");
    }
    public void BotonOpciones()
    {
       SceneManager.LoadScene("Opciones");  
    }
    public void BotonSalir()
    {
        Debug.Log("El boton de salir funciona");
        Application.Quit();
    }
    public void BotonHistoria()
    {
        SceneManager.LoadScene("Historia"); 
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
