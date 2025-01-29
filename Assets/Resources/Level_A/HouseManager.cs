using UnityEngine;
using System.Collections.Generic;
using System.Linq;




public class HouseManager : MonoBehaviour
{
    // Lista przechowuj�ca obiekty HouseStreet
    public List<HouseStreet> HouseList = new List<HouseStreet>();

    // Metoda dodaj�ca nowy dom do listy
    public void AddHouse(HouseStreet house)
    {
        HouseList.Add(house);
    }

    // Metoda losuj�ca indeks z listy
    public int RandomizeIndex()
    {
        if (HouseList.Count == 0)
        {
            Debug.LogWarning("House list is empty!");
            return -1; // Zwraca -1, je�li lista jest pusta
        }

        // Losowanie indeksu
        int randomIndex = Random.Range(0, HouseList.Count);
        return randomIndex;
    }

    // Metoda zwracaj�ca losowy dom z listy
    public HouseStreet RandomizeHouse()
    {
        int randomIndex = RandomizeIndex();
        if (randomIndex == -1)
        {
            return null; // Zwraca null, je�li lista jest pusta
        }

        return HouseList[randomIndex];
    }

    // Przyk�ad u�ycia w Start
    void Start()
    {
        HouseList = FindObjectsByType<HouseStreet>(FindObjectsSortMode.InstanceID).ToList();
        // Losowanie i wy�wietlanie informacji o losowym domu
        HouseStreet randomHouse = RandomizeHouse();
        if (randomHouse != null)
        {
            Debug.Log("Randomized House:");
            randomHouse.DisplayInfo();
        }
    }
}