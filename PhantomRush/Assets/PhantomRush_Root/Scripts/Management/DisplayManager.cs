using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatioManager : MonoBehaviour
{
    [SerializeField] private int targetDisplay = 0; // Índice de la pantalla donde se abrirá el juego (0 = Pantalla principal)

    private float targetAspect = 18f / 9f; // Relación de aspecto objetivo 18:9

    void Start()
    {
        // Detectar la resolución de la pantalla seleccionada y ajustar la resolución del juego
        SetResolutionForDisplay(targetDisplay);
    }

    void SetResolutionForDisplay(int displayIndex)
    {
        bool isFullscreen = Screen.fullScreen; // Detecta si el juego está en pantalla completa o no

        if (displayIndex < Display.displays.Length)
        {
            if (!Display.displays[displayIndex].active)
            {
                Display.displays[displayIndex].Activate();
            }

            // Obtener el ancho y alto nativos de la pantalla seleccionada
            int screenWidth = Display.displays[displayIndex].systemWidth;
            int screenHeight = Display.displays[displayIndex].systemHeight;

            // Calcular la nueva altura basada en la relación de aspecto 18:9
            int newHeight = Mathf.RoundToInt(screenWidth / targetAspect);

            // Si la nueva altura calculada excede la altura de la pantalla, ajustar el ancho en su lugar
            if (newHeight > screenHeight)
            {
                newHeight = screenHeight;
                screenWidth = Mathf.RoundToInt(newHeight * targetAspect);
            }

            // Establecer la resolución del juego ajustada, usando isFullscreen
            Screen.SetResolution(screenWidth, newHeight, isFullscreen);
            Debug.Log($"Resolución ajustada a {screenWidth}x{newHeight} en la pantalla {displayIndex} | Pantalla Completa: {isFullscreen}");
        }
        else
        {
            Debug.LogWarning($"La pantalla {displayIndex} no está disponible. Se usará la pantalla principal.");
        }
    }
}





