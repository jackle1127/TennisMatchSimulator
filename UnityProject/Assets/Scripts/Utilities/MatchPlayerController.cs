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
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private Transform shotListContainer;
    [SerializeField] private Text matchInfo;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private PointPicker player1XYPicker;
    [SerializeField] private PointPicker player2XYPicker;
    [SerializeField] private PointPicker heightPicker;
    [SerializeField] private PointPicker netPicker;
    [SerializeField] private Transform ball;

    private JSONNode matchJSON;
    private List<ShotDataObject> shotDataList = new List<ShotDataObject>();
    private Dictionary<JSONNode, ShotDataObject> jsonShotToShotData = new Dictionary<JSONNode, ShotDataObject>();
    private int currentShotIndex;
    private float currentShotTime = 0;
    private bool playing;

    private void Update()
    {
        if (playing)
        {
            ShotDataObject currentShot = shotDataList[currentShotIndex];
            ball.position = currentShot.Evaluate(currentShotTime);
            //Debug.Log(currentShotTime + "/" + currentShot.shotDuration);
            currentShotTime += Time.deltaTime;
            if (currentShotTime >= currentShot.shotDuration)
            {
                if (currentShotIndex >= shotDataList.Count)
                {
                    playing = false;
                    currentShotIndex--;
                }

                SetCurrentShot(currentShotIndex + 1);

                UpdateUI();
            }
        }
    }

    public bool LoadJSON(string path)
    {
        /*try
        {*/
        matchJSON = JSON.Parse(TennisSim.Utility.ReadFile(path));
        //UpdateMatchInfo();
        PopulateShotList();
        AnalyzeShots();
        UpdateUI();

        return true;
        /*}
        catch (Exception e)
        {
            return false;
        }*/
    }

    private void PopulateShotList()
    {
        shotDataList.Clear();
        jsonShotToShotData.Clear();

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
                        Button pointButton = CreateListItemButton("Point (" + PointsToString(pointData[0][0], pointData[0][1]) + ")", 2).GetComponent<Button>();
                        ShotDataObject beginningShot = null;

                        int shotNumber = 0;
                        int pointBeginningShotIndex = shotDataList.Count;
                        foreach (JSONNode shotData in pointData[1]["shots"])
                        {
                            ShotDataObject shotDataObject = new ShotDataObject();
                            shotDataObject.set = setData;
                            shotDataObject.game = gameData;
                            shotDataObject.point = pointData;
                            shotDataObject.shot = shotData;
                            shotDataObject.shotIndexInPoint = shotNumber;
                            shotDataObject.pointBeginningShotIndex = pointBeginningShotIndex;
                            shotNumber++;

                            shotDataList.Add(shotDataObject);
                            jsonShotToShotData.Add(shotData, shotDataObject);
                            if (beginningShot == null)
                            {
                                beginningShot = shotDataObject;
                            }
                        }

                        pointButton.onClick.AddListener(delegate ()
                        {
                            SetCurrentShot(beginningShot);
                            Pause();
                        });
                    }
                }
            }
        }

        SetCurrentShot(0);
    }

    public void UpdateUI()
    {
        ShotDataObject currentShot = shotDataList[currentShotIndex];
        progressSlider.maxValue = currentShot.point[1]["shots"].Count;
        progressSlider.value = currentShot.shotIndexInPoint;

        string matchInfoString = "";

        matchInfoString += "Tournament: \n" + matchJSON["tournament"];
        matchInfoString += "\n";
        matchInfoString += "\n";

        matchInfoString += "Player 1: " + matchJSON["players"][0];
        matchInfoString += "\n";
        matchInfoString += "Match score: " + currentShot.set[0][0];
        matchInfoString += "\n";
        matchInfoString += "Set score: " + currentShot.game[0][0];
        matchInfoString += "\n";
        matchInfoString += "\n";

        matchInfoString += "Player 2: " + matchJSON["players"][1];
        matchInfoString += "\n";
        matchInfoString += "Match score: " + currentShot.set[0][1];
        matchInfoString += "\n";
        matchInfoString += "Set score: " + currentShot.game[0][1];
        matchInfoString += "\n";
        matchInfoString += "\n";

        matchInfoString += "Game score: " + PointsToString(currentShot.point[0][0], currentShot.point[0][1]);
        matchInfoString += "\n";
        matchInfoString += "\n";

        matchInfoString += "Shot by: " + matchJSON["players"][(int) currentShot.shot["player"]];
        matchInfoString += "\n";
        matchInfoString += "Shot type: " + TennisSim.Utility.CapitalizeFirstCharacter(currentShot.shot["category"]);
        matchInfoString += "\n";
        matchInfoString += "Shot depth: " + TennisSim.Utility.CapitalizeFirstCharacter(currentShot.shot["depth"]);
        matchInfoString += "\n";
        matchInfoString += "Shot direction: " + TennisSim.Utility.CapitalizeFirstCharacter(currentShot.shot["direction"]);
        matchInfoString += "\n";

        matchInfo.text = matchInfoString;
    }

    private void AnalyzeShots()
    {
        for (int i = 0; i < shotDataList.Count; i++)
        {
            PointPicker playerPointPicker, opponentPointPicker;
            ShotDataObject shot = shotDataList[i];
            JSONNode shotJSON = shot.shot;

            shot.shotDuration = .69f;
            //Debug.Log(shotJSON["player"] + ", " + (shotJSON["player"] == 0));
            if (shotJSON["player"] == 0)
            {
                playerPointPicker = player1XYPicker;
                opponentPointPicker = player2XYPicker;
            }
            else
            {
                playerPointPicker = player2XYPicker;
                opponentPointPicker = player1XYPicker;
            }

            if (shotJSON["category"] == "serve")
            {
                shot.startPoint = playerPointPicker.PickRandomWithMultiplier(0, 0, 0, 0, 1, 1);
                shot.startPoint.y = heightPicker.PickRandomWithMultiplier(0, .1f, 2).y;
                //Debug.Log(shot.startPoint);
            }
            
            if (shotJSON["point"] == "no" && shotJSON["error"] == "none")
            {
                ShotDataObject nextShot = shotDataList[i + 1];
                nextShot.startPoint = opponentPointPicker.PickRandom();
                nextShot.startPoint.y = heightPicker.PickRandomWithMultiplier(0, .1f, 2).y;
                shot.endPoint = nextShot.startPoint;
                shot.ProcessShotWithBounce(netPicker.PickRandomWithMultiplier(.8f, 1));
            }
            else
            {
                shot.endPoint = opponentPointPicker.PickRandom();
                shot.ProcessShotWithoutBounce(netPicker.PickRandomWithMultiplier(.8f, 1));
            }

            //Debug.Log(i + ": " + shot.startPoint + ", " + shot.endPoint);
        }
    }

    public void SetCurrentShot(ShotDataObject shotDataObject)
    {
        SetCurrentShot(shotDataList.IndexOf(shotDataObject));
    }

    public void SetCurrentShot(int shotIndex)
    {
        currentShotIndex = shotIndex;
        currentShotTime = 0;
        UpdateUI();
    }

    private string PointsToString(int point1, int point2)
    {
        if (point1 <= 3 && point2 <= 3)
        {
            return GetDisplayPoint(point1) + "/" + GetDisplayPoint(point2);
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

    public void Play()
    {
        playing = true;
        pauseButton.SetActive(true);
        playButton.SetActive(false);
    }

    public void Pause()
    {
        playing = false;
        pauseButton.SetActive(false);
        playButton.SetActive(true);
    }
}
