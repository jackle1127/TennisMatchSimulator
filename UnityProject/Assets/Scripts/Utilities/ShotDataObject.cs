using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class ShotDataObject
{
    private const int GROUND_POINT_ITERATION = 10;
    private const float Y_DIFFERENCE_TOLERANCE = .001f;

    public JSONNode set, game, point, shot;
    public int shotIndexInPoint;
    public int pointBeginningShotIndex;
    public float shotDuration;

    public Vector3 startPoint, endPoint;

    private Vector3 xAxisVector, yAxisVector, origin;
    private Vector2 startPoint2D, endPoint2D;
    private float bounceTime;
    private Trajectory[] trajectories;

    public void ProcessShotWithoutBounce(Vector3 netHeightPoint)
    {
        Process2DSpace();
        Vector2 netPoint2D = GetNetPoint(netHeightPoint);

        trajectories = new Trajectory[1];
        trajectories[0] = new Trajectory(startPoint2D, netPoint2D, endPoint2D);
        //trajectories[0].Calculate(new Vector2(0, 0), new Vector2(2, 2), new Vector2(5, .1f));
        //TennisSim.Utility.LogMultiple(trajectories[0].Evaluate(0), trajectories[0].Evaluate(2), trajectories[0].Evaluate(5));
        //TennisSim.Utility.LogMultiple(trajectories[0].scale, trajectories[0].translateX, trajectories[0].translateY);
        //Debug.Log(startPoint2D + ", " + netPoint2D + ", " + endPoint2D + " - " + netHeightPoint);
    }

    public void ProcessShotWithBounce(Vector3 netHeightPoint)
    {
        Process2DSpace();
        Vector2 netPoint2D = GetNetPoint(netHeightPoint);

        trajectories = new Trajectory[2];
        trajectories[0] = new Trajectory();
        trajectories[1] = new Trajectory();

        // Binary search for ground point that bounces the ball right at the end point.
        float minX = netPoint2D.x;
        float maxX = endPoint2D.x;
        Vector2 groundHit = Vector2.zero;
        for (int iteration = 1; iteration <= GROUND_POINT_ITERATION; iteration++)
        {
            groundHit = new Vector2((minX + maxX) / 2, 0);
            trajectories[0].Calculate(startPoint2D, netPoint2D, groundHit);

            // The bouncing trajectory is identical to the shot trajectory but shifted forward.
            trajectories[1].Copy(trajectories[0]);
            float[] groundIntersections = trajectories[0].SolveForX(0);
            trajectories[1].translateX += groundIntersections[1] - groundIntersections[0];
            //break;
            float testY = trajectories[1].Evaluate(endPoint2D.x);

            if (Mathf.Abs(testY - endPoint2D.y) <= Y_DIFFERENCE_TOLERANCE)
            {
                // The ground hit is tolerably precise.
                //Debug.Log("suh dude");
                break;
            }
            else
            {
                if (testY > endPoint2D.y)
                {
                    // Ground hit point too close.
                    //Debug.Log("Forward");
                    minX = groundHit.x;
                }
                else if (testY < endPoint2D.y)
                {
                    //Debug.Log("Backward");
                    maxX = groundHit.x;
                }
            }
        }

        bounceTime = shotDuration * (groundHit.x - startPoint2D.x) / (endPoint2D.x - startPoint2D.x);
        //TennisSim.Utility.LogMultiple(bounceTime, shotDuration);
        //TennisSim.Utility.LogMultiple(groundHit.x, startPoint.x, endPoint.x, startPoint.x);
    }

    public Vector3 Evaluate(float time)
    {
        float alpha = time / shotDuration;
        Vector3 ballPosition = Vector3.Lerp(startPoint, endPoint, alpha);
        float x = alpha * (endPoint2D.x);
        if (trajectories != null)
        {
            if (trajectories.Length == 1)
            {
                //Debug.Log(trajectories[0].scale + ", " + trajectories[0].translateX + ", " + trajectories[0].translateY);
                ballPosition.y = trajectories[0].Evaluate(x);
            }
            else if (trajectories.Length == 2)
            {
                if (time <= bounceTime)
                {
                    ballPosition.y = trajectories[0].Evaluate(x);
                }
                else
                {
                    //Debug.Log("BOUNCE!!");
                    ballPosition.y = trajectories[1].Evaluate(x);
                }
            }
        }
        return ballPosition;
    }

    private Vector2 GetNetPoint(Vector3 netHeightPoint)
    {
        Vector2 netPoint2D = new Vector2(0, netHeightPoint.y);
        float startToNetX = netHeightPoint.x - startPoint.x;
        float netToEndX = endPoint.x - netHeightPoint.x;
        float alpha = startToNetX / (startToNetX + netToEndX);
        netPoint2D.x = Mathf.Lerp(startPoint2D.x, endPoint2D.x, alpha);

        return netPoint2D;
    }

    private void Process2DSpace()
    {
        xAxisVector = (endPoint - startPoint).normalized;
        yAxisVector = Vector3.up;
        origin = startPoint;
        origin.y = 0;
        startPoint2D = new Vector2(Vector3.Dot(xAxisVector, startPoint - origin), startPoint.y);
        endPoint2D = new Vector2(Vector3.Dot(xAxisVector, endPoint - origin), endPoint.y);
    }
}
