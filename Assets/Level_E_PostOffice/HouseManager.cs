using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Analytics;



public class HouseManager : MonoBehaviour
{
    // Lista przechowuj�ca obiekty HouseStreet
    [SerializeField] VRClickablePanel infoPanel;
    public List<HouseStreet> HouseList = new List<HouseStreet>();
    public int seed = 123;
    public GameObject PackagePrefab;
    public Transform PackegeSpawnPoint;
    public MovementStats movementStats;
    public int packagesToWin = 3;
    private int packagesCounter = 0;
    public GameObject player;
    private bool gameWinState = false;
    private bool gameStarted = false;
    private string winMssg = "Brawo dostarczy�e� wszystkie paczki. DOBRA ROBOTA!";
    private string startMssg = "Dostarcz wszystkie przesy�ki do odpowiednich dom�w. Powodzenia!";


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
        int randomIndex = UnityEngine.Random.Range(0, HouseList.Count);
        return randomIndex;
    }

    void SortHouseList()
    {
        HouseList = HouseList
            .OrderBy(h => h.HouseNumber)
            .ThenBy(h => h.StreetColor.ToString()) // Sorting colors as strings
            .ToList();
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
        HouseStreet.onPackageDelivered += PackageDeliverd;
        HouseList = FindObjectsByType<HouseStreet>(FindObjectsSortMode.InstanceID).ToList();
        UnityEngine.Random.InitState(seed);
        SortHouseList();
        SetUp();
    }

    private void SetUp()
    {
        gameWinState = false;
        var actions = new VRClickablePanel.ButtonAction[] { new(StartGame), new(ReturnToMenu) };
        infoPanel.ShowPanel(startMssg, actions);
    }

    private void StartGame()
    {
        gameStarted = true;
         packagesCounter = 0;
        foreach (HouseStreet house in HouseList)
        {
            house.SetMailbox(false);
        }
        // Losowanie i wy�wietlanie informacji o losowym domu
        PackageSpawn();

        movementStats.StartMeasuring($"Assets/Stats/Level_E_Seed{seed}_{System.DateTime.Now.ToString().Replace(' ', '_').Replace(':', '_')}.json");
    }

    private void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    private void ReturnToMenu()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    private void PackageDeliverd()
    {
        packagesCounter++;
        if (packagesCounter == packagesToWin)
        {
            gameWinState = true;
            movementStats.StopMeasuring(true);
            UpdatePanel();
            Debug.Log("All packages delivered");
        }
        else PackageSpawn();
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
    private void OnDestroy()
    {
        if (packagesCounter != packagesToWin && gameStarted)
            movementStats.StopMeasuring(false);
        HouseStreet.onPackageDelivered -= PackageDeliverd;
    }

    void UpdatePanel()
    {
        string mssg = winMssg;
        var actions = new VRClickablePanel.ButtonAction[] { new("Graj jeszcze raz", Reset), new(ReturnToMenu) };
        infoPanel.ShowPanel(winMssg, actions);
    }
}