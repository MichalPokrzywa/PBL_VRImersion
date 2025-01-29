using UnityEngine;
using System.Collections.Generic;
using System.Linq;




public class HouseManager : MonoBehaviour
{
    // Lista przechowuj¹ca obiekty HouseStreet
    public List<HouseStreet> HouseList = new List<HouseStreet>();
    public GameObject PackagePrefab;
    public Transform PackegeSpawnPoint;

    // Metoda dodaj¹ca nowy dom do listy
    public void AddHouse(HouseStreet house)
    {
        HouseList.Add(house);
    }

    // Metoda losuj¹ca indeks z listy
    public int RandomizeIndex()
    {
        if (HouseList.Count == 0)
        {
            Debug.LogWarning("House list is empty!");
            return -1; // Zwraca -1, jeœli lista jest pusta
        }

        // Losowanie indeksu
        int randomIndex = Random.Range(0, HouseList.Count);
        return randomIndex;
    }

    // Metoda zwracaj¹ca losowy dom z listy
    public HouseStreet RandomizeHouse()
    {
        int randomIndex = RandomizeIndex();
        if (randomIndex == -1)
        {
            return null; // Zwraca null, jeœli lista jest pusta
        }

        return HouseList[randomIndex];
    }


    // Przyk³ad u¿ycia w Start
    void Start()
    {
        HouseStreet.onPackageDelivered += PackageSpawn;
        HouseList = FindObjectsByType<HouseStreet>(FindObjectsSortMode.InstanceID).ToList();

        foreach (HouseStreet house in HouseList)
        {
            house.SetMailbox(false);
        }
        // Losowanie i wyœwietlanie informacji o losowym domu
        PackageSpawn();
    }

    private void PackageSpawn()
    {
        HouseStreet randomHouse = RandomizeHouse();
        if (randomHouse != null)
        {
            Debug.Log("Randomized House:");
            randomHouse.DisplayInfo();
            randomHouse.SetMailbox(true);
            GameObject pack = Instantiate(PackagePrefab, PackegeSpawnPoint.position,Quaternion.identity);
            pack.GetComponent<Package>().SetPackage(randomHouse.HouseNumber, randomHouse.StreetColor);
        }
    }
}