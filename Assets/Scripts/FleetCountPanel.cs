using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class FleetCountPanel : MonoBehaviour 
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Text countText;

    private Fleet fleet;

    public void SetFleet(Fleet fleet)
    {
        this.fleet = fleet;

        Refresh();
    }

    public void Refresh()
    {
        backgroundImage.color = fleet.dark;
        countText.color = Color.white;
        countText.text = string.Format("{0} - {1} ships", fleet.name, fleet.ships);
    }
}
