using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MenuUI : MonoBehaviour
{
    [Header("Referências")]
    public GameObject painelMenu;

    private bool aberto = false;

    void Start()
    {
        FecharMenu(); // começa fechado
    }

    void Update()
    {
        // Tecla ESC abre/fecha (opcional, mas útil)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (aberto)
                FecharMenu();
            else
                AbrirMenu();
        }
    }

    public void AbrirMenu()
    {
        painelMenu.SetActive(true);
        Time.timeScale = 0f;
        aberto = true;
    }

    public void FecharMenu()
    {
        painelMenu.SetActive(false);
        Time.timeScale = 1f;
        aberto = false;
    }

    // BOTÃO VOLTAR USA ESSE
    public void BotaoVoltar()
    {
        FecharMenu();
    }
}