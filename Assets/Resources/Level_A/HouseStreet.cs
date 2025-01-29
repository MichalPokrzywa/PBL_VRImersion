using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HouseStreet : MonoBehaviour
{
    public int HouseNumber;
    public Color StreetColor;
    public GameObject Mailbox;

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
