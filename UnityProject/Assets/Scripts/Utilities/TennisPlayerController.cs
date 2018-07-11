using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisPlayerController : MonoBehaviour {
    [SerializeField] private Transform servePoint;
    [SerializeField] float serveDelay = 1.35f;
    [SerializeField] private Transform forehandGroundPoint;
    [SerializeField] float forehandGroundDelay = .1f;
    [SerializeField] private Transform backhandGroundPoint;
    [SerializeField] float backhandGroundDelay = .1f;
    [SerializeField] private Transform forehandSlicePoint;
    [SerializeField] float forehandSliceDelay = .1f;
    [SerializeField] private Transform backhandSlicePoint;
    [SerializeField] float backhandSliceDelay = .1f;
    [SerializeField] private Transform forehandVolleyPoint;
    [SerializeField] float forehandVolleyDelay = .1f;
    [SerializeField] private Transform backhandVolleyPoint;
    [SerializeField] float backhandVolleyDelay = .1f;
    [SerializeField] private Transform forehandOverheadPoint;
    [SerializeField] float forehandOverheadDelay = .1f;
    [SerializeField] private Transform backhandOverheadPoint;
    [SerializeField] float backhandOverheadDelay = .1f;
    [SerializeField] private Transform forehandDropShotPoint;
    [SerializeField] float forehandDropShotDelay = .1f;
    [SerializeField] private Transform backhandDropShotPoint;
    [SerializeField] float backhandDropShotDelay = .1f;
    [SerializeField] private Transform forehandHalfVolleyPoint;
    [SerializeField] float forehandHalfVolleyDelay = .1f;
    [SerializeField] private Transform backhandHalfVolleyPoint;
    [SerializeField] float backhandHalfVolleyDelay = .1f;
    [SerializeField] private Transform forehandSwingingVolleyPoint;
    [SerializeField] float forehandSwingingVolleyDelay = .1f;
    [SerializeField] private Transform backhandSwingingVolleyPoint;
    [SerializeField] float backhandSwingingVolleyDelay = .1f;
    [SerializeField] Transform modelContainer;
    [SerializeField] float runBlendTime = .1f;
    [SerializeField] Animator playerAnimator;
    [SerializeField] float distanceToRunTime = .2f;

    public delegate void ShotCallback();
    public event ShotCallback shotCallback;

    private Vector3 runTarget;
    private float timeToRun;
    private float currentRunBlendTime;
    private float runAnimationTime;

    private float currentShotDelayTime;
    private string shotToPlay = "";
    private float shotDelayTime;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float runningLayerAlpha = Mathf.Clamp01(1 - currentRunBlendTime / runBlendTime);
        runningLayerAlpha = Mathf.Clamp01(Mathf.Min(runningLayerAlpha, timeToRun / runBlendTime));
        playerAnimator.SetLayerWeight(1, runningLayerAlpha);

        if (timeToRun > 0)
        {
            float deltaTime = Time.deltaTime;
            if (deltaTime > timeToRun) deltaTime = timeToRun;

            Vector3 newPosition = Vector3.Lerp(transform.position, runTarget, deltaTime / timeToRun);
            float distance = (newPosition - transform.position).magnitude;
            runAnimationTime += distance * distanceToRunTime;
            runAnimationTime %= 1;

            // Run animation.
            playerAnimator.Play("Run", 1, runAnimationTime);

            Quaternion runDirection = Quaternion.LookRotation(runTarget - transform.position);
            modelContainer.rotation = Quaternion.Slerp(transform.rotation, runDirection, runningLayerAlpha);

            transform.position = newPosition;

            
            timeToRun -= deltaTime;
            currentRunBlendTime -= deltaTime;
            if (currentRunBlendTime < 0) currentRunBlendTime = 0;
        }
        else if (shotToPlay != "")
        {
            playerAnimator.Play(shotToPlay, 0);
            currentShotDelayTime = shotDelayTime;
            Debug.Log("PLAYING " + shotToPlay + ", " + currentShotDelayTime);
            shotToPlay = "";
        }

        if (currentShotDelayTime > 0)
        {
            currentShotDelayTime -= Time.deltaTime;
            if (currentShotDelayTime <= 0)
            {
                Debug.Log("POP");
                if (shotCallback != null)
                {
                    shotCallback();
                    shotCallback = null;
                }
            }
        }
	}

    public void RunTowards(Vector3 target, float runDuration)
    {
        runTarget = target;
        timeToRun = runDuration;
        currentRunBlendTime = runBlendTime;
    }

    public void MakeShot(Vector3 shotLocation, string shotType, float makeShotIn)
    {
        Vector3 runTarget = shotLocation + (transform.position - GetShotPoint(shotType));
        TennisSim.Utility.LogMultiple(shotLocation, runTarget);
        shotToPlay = GetShotAnimationName(shotType);
        shotDelayTime = GetShotDelay(shotType);
        RunTowards(runTarget, makeShotIn - shotDelayTime);
    }

    private string GetShotAnimationName(string shotCategory)
    {
        switch (shotCategory)
        {
            case "serve":
                return "Serve";
            case "forehandGround":
                return "Forehand Ground";
            case "backhandGround":
                return "Backhand Ground";
            case "forehandSlice":
                return "Forehand Slice";
            case "backhandSlice":
                return "Backhand Slice";
            case "forehandVolley":
                return "Forehand Ground";
            case "backhandVolley":
                return "Backhand Ground";
            case "forehandOverhead":
                return "Forehand Slice";
            case "backhandOverhead":
                return "Backhand Slice";
            case "forehandDropShot":
                return "Forehand Slice";
            case "backhandDropShot":
                return "Backhand Slice";
            case "forehandLob":
                return "Forehand Ground";
            case "backhandLob":
                return "Backhand Ground"; ;
            case "forehandHalfVolley":
                return "Forehand Ground";
            case "backhandHalfVolley":
                return "Backhand Ground";
            case "forehandSwingingVolley":
                return "Forehand Ground";
            case "backhandSwingingVolley":
                return "Backhand Ground";
            case "trick":
                return "Forehand Ground";
            case "unknown":
                return "Forehand Ground";
        }
        return "Forehand Ground";
    }

    public float GetShotDelay(string shotCategory)
    {
        switch (shotCategory)
        {
            case "serve":
                return serveDelay;
            case "forehandGround":
                return forehandGroundDelay;
            case "backhandGround":
                return backhandGroundDelay;
            case "forehandSlice":
                return forehandSliceDelay;
            case "backhandSlice":
                return backhandSliceDelay;
            case "forehandVolley":
                return forehandVolleyDelay;
            case "backhandVolley":
                return backhandVolleyDelay;
            case "forehandOverhead":
                return forehandOverheadDelay;
            case "backhandOverhead":
                return backhandOverheadDelay;
            case "forehandDropShot":
                return forehandDropShotDelay;
            case "backhandDropShot":
                return backhandDropShotDelay;
            case "forehandLob":
                return forehandGroundDelay;
            case "backhandLob":
                return backhandGroundDelay;
            case "forehandHalfVolley":
                return forehandHalfVolleyDelay;
            case "backhandHalfVolley":
                return backhandHalfVolleyDelay;
            case "forehandSwingingVolley":
                return forehandSwingingVolleyDelay;
            case "backhandSwingingVolley":
                return backhandSwingingVolleyDelay;
            case "trick":
                return backhandGroundDelay;
            case "unknown":
                return forehandGroundDelay;
        }
        return 0;
    }

    public Vector3 GetShotPoint(string shotCategory)
    {
        switch (shotCategory)
        {
            case "serve":
                return servePoint.position;
            case "forehandGround":
                return forehandGroundPoint.position;
            case "backhandGround":
                return backhandGroundPoint.position;
            case "forehandSlice":
                return forehandSlicePoint.position;
            case "backhandSlice":
                return backhandSlicePoint.position;
            case "forehandVolley":
                return forehandVolleyPoint.position;
            case "backhandVolley":
                return backhandVolleyPoint.position;
            case "forehandOverhead":
                return forehandOverheadPoint.position;
            case "backhandOverhead":
                return backhandOverheadPoint.position;
            case "forehandDropShot":
                return forehandDropShotPoint.position;
            case "backhandDropShot":
                return backhandDropShotPoint.position;
            case "forehandLob":
                return forehandGroundPoint.position;
            case "backhandLob":
                return backhandGroundPoint.position;
            case "forehandHalfVolley":
                return forehandHalfVolleyPoint.position;
            case "backhandHalfVolley":
                return backhandHalfVolleyPoint.position;
            case "forehandSwingingVolley":
                return forehandSwingingVolleyPoint.position;
            case "backhandSwingingVolley":
                return backhandSwingingVolleyPoint.position;
            case "trick":
                return backhandGroundPoint.position;
            case "unknown":
                return forehandGroundPoint.position;
        }
        return Vector3.zero;
    }
}
