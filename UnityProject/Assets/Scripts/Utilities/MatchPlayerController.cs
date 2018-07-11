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
    [SerializeField] private Slider speedSlider;
    [SerializeField] private Text speedText;
    [SerializeField] private BallController ballController;
    [SerializeField] private TennisPlayerController player1Controller;
    [SerializeField] private TennisPlayerController player2Controller;

    private JSONNode matchJSON;
    private List<ShotDataObject> shotDataList = new List<ShotDataObject>();
    private List<Button> pointButtons = new List<Button>();
    private Dictionary<JSONNode, ShotDataObject> jsonShotToShotData = new Dictionary<JSONNode, ShotDataObject>();
    private int currentShotIndex;
    private float currentShotTime = 0;
    private bool playing;
    private bool waitForServe = false;

    private void Update()
    {
        if (playing && !waitForServe)
        {
            ShotDataObject currentShot = shotDataList[currentShotIndex];
            ball.position = currentShot.Evaluate(currentShotTime);
            //Debug.Log(currentShotTime + "/" + currentShot.shotDuration);
            currentShotTime += Time.deltaTime;
            if (currentShotTime >= currentShot.shotDuration)
            {
                if (currentShot.shot["point"] == "no" && currentShot.shot["error"] == "none")
                {
                    SetCurrentShot(currentShotIndex + 1);
                }
                else
                {
                    if (currentShot.shot["error"] != "none")
                    {
                        PointLost();
                    }
                    else
                    {
                        PointWin();
                    }
                }
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

                    bool oddPoint = false;
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
                            shotDataObject.oddPoint = oddPoint;
                            shotDataObject.pointButton = pointButton;
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

                        pointButtons.Add(pointButton);

                        oddPoint = !oddPoint;
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

        matchInfoString += "Shot by: " + matchJSON["players"][(int)currentShot.shot["player"]];
        matchInfoString += "\n";
        matchInfoString += "Shot type: " + TennisSim.Utility.CapitalizeFirstCharacter(TennisSim.Utility.Decamelize(currentShot.shot["shotType"]));
        matchInfoString += "\n";
        matchInfoString += "Shot category: " + TennisSim.Utility.CapitalizeFirstCharacter(currentShot.shot["category"]);
        matchInfoString += "\n";
        matchInfoString += "Shot depth: " + TennisSim.Utility.CapitalizeFirstCharacter(currentShot.shot["depth"]);
        matchInfoString += "\n";
        matchInfoString += "Shot direction: " + TennisSim.Utility.CapitalizeFirstCharacter(currentShot.shot["direction"]);
        matchInfoString += "\n";
        matchInfoString += "Point: " + TennisSim.Utility.CapitalizeFirstCharacter(TennisSim.Utility.Decamelize(currentShot.shot["point"]));
        matchInfoString += "\n";
        matchInfoString += "Error: " + TennisSim.Utility.CapitalizeFirstCharacter(TennisSim.Utility.Decamelize(currentShot.shot["error"]));
        matchInfoString += "\n";

        matchInfo.text = matchInfoString;

        foreach (Button pointButton in pointButtons)
        {
            ColorBlock temp = pointButton.colors;
            if (pointButton == currentShot.pointButton)
            {
                temp.normalColor = new Color(.106f, 455f, 0);
            }
            else
            {
                temp.normalColor = Color.black;
            }
            pointButton.colors = temp;
        }

    }

    private void AnalyzeShots()
    {
        for (int i = 0; i < shotDataList.Count; i++)
        {
            PointPicker playerPointPicker, opponentPointPicker;
            TennisPlayerController playerController, opponentController;
            ShotDataObject shot = shotDataList[i];
            JSONNode shotJSON = shot.shot;

            //Debug.Log(shotJSON["player"] + ", " + (shotJSON["player"] == 0));
            if (shotJSON["player"] == 0)
            {
                playerPointPicker = player1XYPicker;
                opponentPointPicker = player2XYPicker;
                playerController = player1Controller;
                opponentController = player2Controller;
            }
            else
            {
                playerPointPicker = player2XYPicker;
                opponentPointPicker = player1XYPicker;
                playerController = player2Controller;
                opponentController = player1Controller;
            }

            if (shotJSON["category"] == "serve")
            {
                PickShotStartPosition(null, shot, playerPointPicker, playerController);
            }

            if (shotJSON["point"] == "no" && shotJSON["error"] == "none")
            {
                ShotDataObject nextShot = shotDataList[i + 1];
                PickShotStartPosition(shot, nextShot, opponentPointPicker, playerController);
                shot.endPosition = nextShot.startPosition;
                if (nextShot.shot["shotType"] == "forehandVolley" || nextShot.shot["shotType"] == "backhandVolley")
                {
                    shot.shotDuration *= .92f;
                    shot.ProcessShotWithoutBounce(netPicker.PickRandomWithMultiplier(.8f, 1));
                }
                else
                {
                    shot.ProcessShotWithBounce(netPicker.PickRandomWithMultiplier(.8f, 1));
                }
            }
            else
            {
                if (shot.shot["error"] == "net")
                {
                    shot.endPosition = playerPointPicker.PickRandomWithMultiplier(1, 1, 1, 0, 0, 0);
                    shot.endPosition.y = heightPicker.PickRandomWithMultiplier(.03f, 1, 0).y;
                }
                else if (shot.shot["error"] == "wide")
                {
                    // Randomly pick a side to drop the ball out side
                    if (UnityEngine.Random.value > .5f)
                    {
                        shot.endPosition = opponentPointPicker.PickRandomWithMultiplier(1, 0, 0, 1, 0, 0);
                    }
                    else
                    {
                        shot.endPosition = opponentPointPicker.PickRandomWithMultiplier(0, 0, 1, 0, 0, 1);
                    }
                }
                else if (shot.shot["error"] == "deep")
                {
                    shot.endPosition = opponentPointPicker.PickRandomWithMultiplier(0, 0, 0, 1, 1, 1);
                }
                else if (shot.shot["error"] == "wideAndDeep")
                {
                    // Randomly pick a side to drop the ball out side
                    if (UnityEngine.Random.value > .5f)
                    {
                        shot.endPosition = opponentPointPicker.PickPointByWeights(0, 0, 0, 1, 0, 0);
                    }
                    else
                    {
                        shot.endPosition = opponentPointPicker.PickPointByWeights(0, 0, 0, 0, 0, 1);
                    }
                }
                else
                {
                    shot.endPosition = opponentPointPicker.PickRandomWithMultiplier(1, 1, 1, .4f, .4f, .4f);
                }
                if (shot.shot["error"] != "net")
                {
                    shot.ProcessShotWithoutBounce(netPicker.PickRandomWithMultiplier(.8f, 1));
                    shot.shotDuration = UnityEngine.Random.Range(.8f, .85f);
                }
                else
                {
                    Vector3 midPoint = Vector3.Lerp(shot.startPosition, shot.endPosition, .5f) + Vector3.up * UnityEngine.Random.Range(.17f, .41f);
                    shot.ProcessShotWithoutBounce(midPoint);
                    shot.shotDuration = UnityEngine.Random.Range(.37f, .56f);
                    //TennisSim.Utility.LogMultiple(shot.startPosition, midPoint, shot.endPosition);
                }
            }
            //Debug.Log(i + ": " + shot.startPosition + ", " + shot.endPosition);
        }
    }

    private void PickShotStartPosition(ShotDataObject prevShot, ShotDataObject currentShot, PointPicker pointPicker, TennisPlayerController playerController)
    {
        if (prevShot != null)
        {
            float[] xyWeightMultiplier = new float[6];
            float[] heightWeights = new float[3];
            if (prevShot.shot["category"] == "serve")
            {
                // Current rally can only be around the cross course area.
                xyWeightMultiplier[1] = .9f;
                xyWeightMultiplier[4] = 1;

                if (prevShot.oddPoint)
                {
                    xyWeightMultiplier[0] = .9f;
                    xyWeightMultiplier[3] = .9f;

                    if (prevShot.shot["direction"] == "wide")
                    {
                        xyWeightMultiplier[0] *= 3;
                        xyWeightMultiplier[3] *= 3;
                    }
                }
                else
                {
                    xyWeightMultiplier[2] = .9f;
                    xyWeightMultiplier[5] = .9f;

                    if (prevShot.shot["direction"] == "wide")
                    {
                        xyWeightMultiplier[2] *= 3;
                        xyWeightMultiplier[5] *= 3;
                    }
                }

                if (prevShot.shot["direction"] == "downTheT")
                {
                    xyWeightMultiplier[1] *= 4;
                    xyWeightMultiplier[4] *= 4;
                }
            }
            else
            {
                xyWeightMultiplier[0] = 1;
                xyWeightMultiplier[1] = 1;
                xyWeightMultiplier[2] = 1;
                xyWeightMultiplier[3] = 1;
                xyWeightMultiplier[4] = 1;
                xyWeightMultiplier[5] = 1;

                if (prevShot.shot["direction"] == "right")
                {
                    xyWeightMultiplier[2] *= 1.4f;
                    xyWeightMultiplier[5] *= 1.4f;
                }
                if (prevShot.shot["direction"] == "downTheCourt")
                {
                    xyWeightMultiplier[1] *= 1.4f;
                    xyWeightMultiplier[4] *= 1.4f;
                }
                if (prevShot.shot["direction"] == "left")
                {
                    xyWeightMultiplier[0] *= 1.4f;
                    xyWeightMultiplier[3] *= 1.4f;
                }

                if (currentShot.shot["shotType"] == "forehandGround" || currentShot.shot["shotType"] == "forehandSlice"
                    || currentShot.shot["shotType"] == "forehandVolley" || currentShot.shot["shotType"] == "forehandLob"
                    || currentShot.shot["shotType"] == "forehandHalfVolley" || currentShot.shot["shotType"] == "forehandSwingingVolley")
                {
                    xyWeightMultiplier[2] *= 1.4f;
                    xyWeightMultiplier[5] *= 1.4f;
                }
                if (currentShot.shot["shotType"] == "backhandGround" || currentShot.shot["shotType"] == "backhandSlice"
                    || currentShot.shot["shotType"] == "backhandVolley" || currentShot.shot["shotType"] == "backhandLob"
                    || currentShot.shot["shotType"] == "backhandHalfVolley" || currentShot.shot["shotType"] == "backhandSwingingVolley")
                {
                    xyWeightMultiplier[0] *= 1.4f;
                    xyWeightMultiplier[3] *= 1.4f;
                }
                if (currentShot.shot["shotType"] == "forehandOverhead" || currentShot.shot["shotType"] == "backhandOverhead")
                {
                    xyWeightMultiplier[0] *= 1.4f;
                    xyWeightMultiplier[1] *= 1.4f;
                    xyWeightMultiplier[2] *= 1.4f;
                }
                if (currentShot.shot["shotType"] == "forehandGround" || currentShot.shot["shotType"] == "backhandGround"
                    || currentShot.shot["shotType"] == "forehandDropShot" || currentShot.shot["shotType"] == "backhandDropShot")
                {
                    xyWeightMultiplier[3] *= 1.4f;
                    xyWeightMultiplier[4] *= 1.4f;
                    xyWeightMultiplier[5] *= 1.4f;
                }

                if (prevShot.shot["position"] == "approach")
                {
                    xyWeightMultiplier[1] *= 1.4f;
                }
                if (prevShot.shot["position"] == "net")
                {
                    xyWeightMultiplier[1] *= 4f;
                }
                if (prevShot.shot["position"] == "baseline")
                {
                    xyWeightMultiplier[3] *= 2;
                    xyWeightMultiplier[4] *= 2;
                    xyWeightMultiplier[5] *= 2;
                }

                if (prevShot.shot["depth"] == "serviceBox")
                {
                    xyWeightMultiplier[0] *= 4;
                    xyWeightMultiplier[1] *= 4;
                    xyWeightMultiplier[2] *= 4;
                }
                if (prevShot.shot["depth"] == "baseline")
                {
                    xyWeightMultiplier[3] *= 4;
                    xyWeightMultiplier[4] *= 4;
                    xyWeightMultiplier[5] *= 4;
                }

                if (prevShot.shot["shotType"] == "forehandLob" || prevShot.shot["shotType"] == "backhandLob")
                {
                    xyWeightMultiplier[3] *= 2;
                    xyWeightMultiplier[4] *= 2;
                    xyWeightMultiplier[5] *= 2;
                }
            }

            currentShot.shotDuration = UnityEngine.Random.Range(1.1f, 1.2f);
            currentShot.startPosition = pointPicker.PickRandomWithMultiplier(xyWeightMultiplier);

            if (currentShot.startPosition.y == 0 || currentShot.startPosition.y == float.NaN)
            {
                currentShot.startPosition.y = heightPicker.PickPointByWeights(1, 2, 0).y;
            }
            // We want to pick the exact height because of animation complication.
            // This may change later on.
            currentShot.startPosition.y = playerController.GetShotPoint(currentShot.shot["shotType"]).y;
        }
        else
        {
            // Current shot is a serve
            if (currentShot.oddPoint)
            {
                currentShot.startPosition = pointPicker.PickRandomWithMultiplier(0, 0, 0, 1, 2, 0);
            }
            else
            {
                currentShot.startPosition = pointPicker.PickRandomWithMultiplier(0, 0, 0, 0, 2, 1);
            }
            currentShot.shotDuration = UnityEngine.Random.Range(.9f, .96f);
            currentShot.startPosition.y = playerController.GetShotPoint("serve").y;
        }
    }

    public void SetCurrentShot(ShotDataObject shotDataObject)
    {
        SetCurrentShot(shotDataList.IndexOf(shotDataObject));
    }

    public void SetCurrentShot(int shotIndex)
    {
        currentShotIndex = shotIndex;
        if (currentShotIndex >= shotDataList.Count)
        {
            playing = false;
            currentShotIndex = 0;
            return;
        }
        currentShotTime = 0;
        ShotDataObject currentShot = shotDataList[currentShotIndex];
        ball.position = currentShot.Evaluate(currentShotTime);
        if (currentShot.shot["category"] == "serve")
        {
            if (playing)
            {
                Serve();
            }
            else
            {
                waitForServe = true;
            }
        }
        else
        {
            GetCurrentPlayerController().shotCallback += delegate ()
            {
                StartCoroutine(MakeCurrentPlayerMoveTowardsCenter());
            };
            SetOpponentShot();
        }

        UpdateUI();
    }

    private void SetOpponentShot()
    {
        if (currentShotIndex < shotDataList.Count - 1)
        {
            ShotDataObject currentShot = shotDataList[currentShotIndex];
            if (currentShot.shot["point"] == "no" && currentShot.shot["error"] == "none")
            {
                ShotDataObject nextShot = shotDataList[currentShotIndex + 1];
                TennisPlayerController opponentController = GetCurrentOpponentController();
                opponentController.MakeShot(nextShot.startPosition, nextShot.shot["shotType"], currentShot.shotDuration);
            }
        }
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
        if (waitForServe)
        {
            Serve();
        }
    }

    public void Pause()
    {
        playing = false;
        pauseButton.SetActive(false);
        playButton.SetActive(true);
    }

    public void ProgressSliderHandler()
    {
        int shotIndexInPoint = (int)progressSlider.value;
        ShotDataObject currentShot = shotDataList[currentShotIndex];
        SetCurrentShot(currentShot.pointBeginningShotIndex + shotIndexInPoint);
    }

    public void SpeedSliderHandler()
    {
        Time.timeScale = Mathf.Pow(1.25f, speedSlider.value);
        speedText.text = "Speed " + Time.timeScale.ToString("##.#") + "x";
    }

    private void PointWin()
    {
        waitForServe = true;
        ballController.SetControlled(false);
        StartCoroutine(TempWaitCoroutine());
    }

    private void PointLost()
    {
        waitForServe = true;
        ballController.SetControlled(false);
        StartCoroutine(TempWaitCoroutine());
    }

    private void Serve()
    {
        waitForServe = true;
        TennisPlayerController servingPlayer = GetCurrentPlayerController();
        ballController.SetBallVisible(false);
        servingPlayer.shotCallback += delegate ()
        {
            ballController.SetBallVisible(true);
            waitForServe = false;
            SetOpponentShot();
        };
        Vector3 servingPoint = shotDataList[currentShotIndex].startPosition;
        float distanceToServingPoint = (servingPlayer.transform.position - servingPoint).magnitude;
        servingPlayer.MakeShot(servingPoint, "serve", distanceToServingPoint * .85f);
    }

    private TennisPlayerController GetCurrentPlayerController()
    {
        if (shotDataList[currentShotIndex].shot["player"] == 0)
        {
            return player1Controller;
        }
        else
        {
            return player2Controller;
        }
    }

    private TennisPlayerController GetCurrentOpponentController()
    {
        if (shotDataList[currentShotIndex].shot["player"] == 0)
        {
            return player2Controller;
        }
        else
        {
            return player1Controller;
        }
    }

    private PointPicker GetCurrentPlayerPointPicker()
    {
        if (shotDataList[currentShotIndex].shot["player"] == 0)
        {
            return player1XYPicker;
        }
        else
        {
            return player2XYPicker;
        }
    }

    private PointPicker GetCurrentOpponentPointPicker()
    {
        if (shotDataList[currentShotIndex].shot["player"] == 0)
        {
            return player2XYPicker;
        }
        else
        {
            return player1XYPicker;
        }
    }


    private IEnumerator TempWaitCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        ballController.SetControlled(true);
        SetCurrentShot(currentShotIndex + 1);
        //Serve();
    }

    private IEnumerator MakeCurrentPlayerMoveTowardsCenter()
    {
        yield return new WaitForSeconds(1.2f);
        GetCurrentPlayerController().RunTowards(GetCurrentPlayerPointPicker().PickPointByWeights(1, 1, 1, 1, 1, 1), 1.2f);
        //Serve();
    }
}
