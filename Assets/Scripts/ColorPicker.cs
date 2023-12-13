using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class ColorPicker : MonoBehaviour
{

    [SerializeField]
    RectTransform _texture;

    [SerializeField]
    Texture2D _RefSprite;

    [SerializeField]
    CustomColor customColor1;

    public delegate void ColorSwitchHandler();
    public event ColorSwitchHandler ColorChanged;

    public Color laserColor;
    Color orange = new Color(1f, 0.5f, 0f, 1f);

    bool pickingCustomColor1 = false;

    public PredictionMotor ship;

    public void OnClickPickColor()
    {
        ColorChanged.Invoke();
        SetColor();
    }

    private void SetColor()
    {
        Vector3 imagePos = _texture.position;
        float globalPosX = Input.mousePosition.x - imagePos.x;
        float globalPosY = Input.mousePosition.y - imagePos.y;

        

        int localPosX = (int)(globalPosX * (_RefSprite.width / _texture.rect.width));
        int localPosY = (int)(globalPosY * (_RefSprite.height / _texture.rect.height));

        laserColor = _RefSprite.GetPixel(localPosX, localPosY);

        if (pickingCustomColor1)
        {
            pickingCustomColor1 = false;
            customColor1.selectingText.gameObject.SetActive(false);

            /*PlayerPrefs.SetFloat("Custom1R", laserColor.r);
            PlayerPrefs.SetFloat("Custom1G", laserColor.g);
            PlayerPrefs.SetFloat("Custom1B", laserColor.b);
            PlayerPrefs.SetFloat("Custom1A", laserColor.a);*/

            customColor1.ChangeColor();
        }
        if (ship != null)
            ship.ChangeColor(ship, laserColor);
        ColorChanged.Invoke();


    }

    public void OnClickPickRed()
    {
        laserColor = Color.red;
        if (ship != null)
            ship.ChangeColor(ship, laserColor);
        ColorChanged.Invoke();


    }
    public void OnClickPickOrange()
    {
        laserColor = orange;
        if (ship != null)
            ship.ChangeColor(ship, laserColor);
        ColorChanged.Invoke();

    }
    public void OnClickPickYellow()
    {
        laserColor = Color.yellow;
        if (ship != null)
            ship.ChangeColor(ship, laserColor);
        ColorChanged.Invoke();

    }
    public void OnClickPickGreen()
    {
        laserColor = Color.green;
        if (ship != null)
            ship.ChangeColor(ship, laserColor);
        ColorChanged.Invoke();

    }
    public void OnClickPickBlue()
    {
        laserColor = Color.blue;
        if (ship != null)
            ship.ChangeColor(ship, laserColor);
        ColorChanged.Invoke();

    }
    public void OnClickPickCyan()
    {
        laserColor = Color.cyan;
        if (ship != null)
            ship.ChangeColor(ship, laserColor);
        ColorChanged.Invoke();

    }
    public void OnClickPickMagenta()
    {
        laserColor = Color.magenta;
        if (ship != null)
            ship.ChangeColor(ship, laserColor);
        ColorChanged.Invoke();

    }
    private void Awake()
    {
        Color savedColor = new Color(PlayerPrefs.GetFloat("LaserColorR"), PlayerPrefs.GetFloat("LaserColorG"), PlayerPrefs.GetFloat("LaserColorB"), PlayerPrefs.GetFloat("LaserColorA"));
        laserColor = savedColor;
    }
    private void OnDisable()
    {
        PlayerPrefs.SetFloat("LaserColorR", laserColor.r);
        PlayerPrefs.SetFloat("LaserColorG", laserColor.g);
        PlayerPrefs.SetFloat("LaserColorB", laserColor.b);
        PlayerPrefs.SetFloat("LaserColorA", laserColor.a);
    }

    public void Custom1()
    {
        if (pickingCustomColor1 == true)
        {
            pickingCustomColor1 = false;
            customColor1.selectingText.gameObject.SetActive(false);

        }
        else if (!customColor1.hasColor)
        {
            pickingCustomColor1 = true;
            customColor1.selectingText.gameObject.SetActive(true);
            laserColor = customColor1.custom1;
        }
        else
        {
            laserColor = customColor1.custom1;
        }

        if (ship != null)
            ship.ChangeColor(ship, laserColor);
        ColorChanged.Invoke();

    }

}
