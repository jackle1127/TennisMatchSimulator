using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [SerializeField] PanelController[] panels;

    public void ShowPanel(PanelController panelToShow)
    {
        foreach (PanelController panel in panels)
        {
            if (panel == null) continue;
            if (panel == panelToShow)
            {
                panel.Show();
            }
            else
            {
                panel.Hide();
            }
        }
    }
}
