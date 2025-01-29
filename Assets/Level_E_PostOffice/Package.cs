
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Package : MonoBehaviour
{
    // Numer paczki
    public int PackageNumber;

    // Kolor paczki (RGB)
    public Color PackageColor;
    public List<GameObject> labelsList = new List<GameObject>();

    private void Awake()
    {
        ColorPackage();
    }
    // Metoda wyœwietlaj¹ca informacje o paczce
    public void DisplayInfo()
    {
        Debug.Log($"Package Number: {PackageNumber}, Package Color: {PackageColor}");
    }

    // Przyk³ad u¿ycia w Start
    void Start()
    {
        // Wyœwietlanie informacji o paczce po uruchomieniu
        DisplayInfo();
    }


    public void SetPackage(int packageNumber, Color packageColor)
    {
        PackageNumber = packageNumber;
        PackageColor = packageColor;
        ColorPackage();

    }

    void ColorPackage()
    {
        foreach (var side in labelsList)
        {
            side.GetComponentInChildren<TMP_Text>().text = PackageNumber.ToString();
            side.GetComponentInChildren<Image>().color = PackageColor;
        }
    }
}