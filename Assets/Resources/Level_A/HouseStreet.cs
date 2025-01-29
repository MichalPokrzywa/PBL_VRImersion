using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HouseStreet : MonoBehaviour
{
    public static Action onPackageDelivered;
    public int HouseNumber;
    public Color StreetColor;
    public GameObject Mailbox;
    public TMP_Text HouseNo;
    public Image HouseColorImage;

    private void Awake()
    {
        HouseNo.text = HouseNumber.ToString();
        HouseColorImage.color = StreetColor;
        Mailbox.GetComponent<XRSocketInteractor>().selectEntered.AddListener(PackageDelivered);
    }

    private void PackageDelivered(SelectEnterEventArgs arg0)
    {
        onPackageDelivered.Invoke();
        Debug.Log("PackageDelivered");
        Destroy(arg0.interactableObject.transform.gameObject);
    }

    public HouseStreet(int houseNumber, Color streetColor)
    {
        HouseNumber = houseNumber;
        StreetColor = streetColor;
    }

    public void DisplayInfo()
    {
        Debug.Log($"House Number: {HouseNumber}, Street Color: {StreetColor}");
    }

    public void SetMailbox(bool state)
    {
        Mailbox.SetActive(state);
    }

}
