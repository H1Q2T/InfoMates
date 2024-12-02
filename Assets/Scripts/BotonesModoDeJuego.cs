using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FuncionalidadBotonesModo : MonoBehaviour
{
    public void BotonVidas()
    {
        SceneManager.LoadScene("JuegoVIDAS");
    }
    public void BotonTiempo()
    {
        SceneManager.LoadScene("JuegoRELOJ");
    }
    public void BotonTutorial()
    {
        SceneManager.LoadScene("JuegoTutorial");

    }
    public void BotonVolver()
    {
        SceneManager.LoadScene("Menu");
    }
    public void BotonRanking()
    {
        SceneManager.LoadScene("Rankings");
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
