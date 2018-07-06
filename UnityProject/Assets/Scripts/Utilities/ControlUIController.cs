using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlUIController : MonoBehaviour {

    // Use this for initialization
    protected virtual void Start () {
        Hide();
    }

    // Update is called once per frame
    protected virtual void Update () {
		
	}

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    public bool isVisible
    {
        get { return gameObject.activeSelf; }
    }
}
