using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchesPanelController : PanelController
{
    [SerializeField] private string matchDataFolder = "Data";
    [SerializeField] private ListController matchesList;
    [SerializeField] private PanelManager rightPanelManager;
    [SerializeField] private PanelController matchPlayerPanelController;
    [SerializeField] private GameObject currentMatchButtonContainer;
    [SerializeField] private GameObject matchNotFound;
    [SerializeField] private MatchPlayerController matchPlayerController;

    public override void Show()
    {
        base.Show();
        matchesList.Clear();
        string fullDirectoryPath = Application.dataPath + "/" + matchDataFolder;
        matchNotFound.SetActive(true);
        try
        {
            DirectoryInfo matchDataInfo = new DirectoryInfo(fullDirectoryPath);
            //Debug.Log(fullPath);
            if (matchDataInfo != null)
            {
                FileInfo[] matchFiles = matchDataInfo.GetFiles();
                foreach (FileInfo fileInfo in matchFiles)
                {
                    if (fileInfo.Extension.ToLower() == ".json")
                    {
                        matchNotFound.SetActive(false);
                        matchesList.AddItem(fileInfo.Name, delegate ()
                        {
                            if (matchPlayerController.LoadJSON(fileInfo.FullName))
                            {
                                if (rightPanelManager && matchPlayerPanelController)
                                {
                                    rightPanelManager.ShowPanel(matchPlayerPanelController);
                                    if (currentMatchButtonContainer)
                                    {
                                        currentMatchButtonContainer.SetActive(true);
                                    }
                                }
                            }
                        });
                    }
                }
                matchesList.RefreshList();
            }
        }
        catch (Exception ex)
        {
        }
    }
}
