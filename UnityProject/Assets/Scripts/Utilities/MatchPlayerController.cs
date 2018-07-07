using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class MatchPlayerController : MonoBehaviour
{
    [SerializeField] private GameObject listItemButtonPrefab;
    [SerializeField] private GameObject listItemTextPrefab;
    [SerializeField] private Transform shotListContainer;
    [SerializeField] private Text matchInfo;

    private JSONNode matchJSON;

    public bool LoadJSON(string path)
    {
        /*try
        {*/
            matchJSON = JSON.Parse(TennisSim.Utility.ReadFile(path));
            UpdateMatchInfo();
            PopulateShotList();
            return true;
        /*}
        catch (Exception e)
        {
            return false;
        }*/
    }

    public void PopulateShotList()
    {
        if (matchJSON != null)
        {
            TennisSim.Utility.RemoveAllChildren(shotListContainer);

            int setNumber = 1;
            foreach (JSONArray setData in matchJSON["sets"])
            {
                CreateListItemText("Set " + setNumber + " (" + setData[0][0] + "/" + setData[0][1] + ")", 0);
                setNumber++;

                int gameNumber = 1;
                foreach (JSONArray gameData in setData[1]["games"])
                {
                    CreateListItemText("Game " + gameNumber + " (" + gameData[0][0] + "/" + gameData[0][1] + ")", 1);
                    gameNumber++;

                    foreach (JSONArray pointData in gameData[1]["points"])
                    {
                        CreateListItemButton("Point (" + PointsToString(pointData[0][0], pointData[0][1]) + ")", 2);

                    }
                }
            }
        }
    }

    private string PointsToString(int point1, int point2)
    {
        if (point1 <= 3 && point2 <= 3)
        {
            return GetDisplayPoint(point1) + "/" + point2;
        }
        else
        {
            if (point1 == 4) return "AD/-";
            if (point2 == 4) return "-/AD";
        }
        return "";
    }

    private string GetDisplayPoint(int point)
    {
        switch (point)
        {
            case 0: return "0";
            case 1: return "15";
            case 2: return "30";
            case 3: return "40";
            case 4: return "AD";
        }
        return "";
    }

    private GameObject CreateListItemText(string text, int indentation)
    {
        string finalText = "";
        for (int i = 1; i <= indentation; i++)
        {
            finalText += "  ";
        }
        finalText += text;

        Text newTextItem = Instantiate(listItemTextPrefab, shotListContainer).GetComponent<Text>();
        newTextItem.text = finalText;
        newTextItem.alignment = TextAnchor.MiddleLeft;
        newTextItem.fontSize = 16;
        newTextItem.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 19.5f);
        newTextItem.color = new Color(.8f, .8f, .8f, 1);
        return newTextItem.gameObject;
    }

    private GameObject CreateListItemButton(string text, int indentation)
    {
        string finalText = "";
        for (int i = 1; i <= indentation; i++)
        {
            finalText += "  ";
        }
        finalText += text;

        Button newButtonItem = Instantiate(listItemButtonPrefab, shotListContainer).GetComponent<Button>();
        Text buttonText = newButtonItem.GetComponentInChildren<Text>();
        buttonText.text = finalText;
        buttonText.alignment = TextAnchor.MiddleLeft;
        buttonText.fontSize = 14;
        newButtonItem.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 24);

        return newButtonItem.gameObject;
    }

    private void UpdateMatchInfo()
    {
        string matchInfoString = "";

        matchInfoString += "Tournament: \n" + matchJSON["tournament"];
        matchInfoString += "\n";

        matchInfoString += "Player 1: " + matchJSON["players"][0];
        matchInfoString += "\n";
        matchInfoString += "Player 2: " + matchJSON["players"][1];
        matchInfoString += "\n";
        matchInfoString += "\n";

        matchInfo.text = matchInfoString;
    }
}
