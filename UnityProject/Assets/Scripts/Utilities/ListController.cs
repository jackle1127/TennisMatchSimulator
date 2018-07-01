using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ListController : MonoBehaviour
{
    [SerializeField] private GameObject listItemPrefab;
    [SerializeField] private Transform listContainer;
    [SerializeField] private List<ListItemInformationGroup> listItems;

    private delegate void CallBack();

    private void Start()
    {
        RefreshList();
    }
    
    public void RefreshList()
    {
        if (!listContainer) return;

        if (listContainer.childCount == 0)
        {
            PopulateList();
        }
        else
        {
            // Clear the list before populating.
            for (int i = listContainer.childCount - 1; i >= 0; i--)
            {
                // Populate the list after the last item is removed.
                if (i == 0)
                {
                    StartCoroutine(DestroyGameObject(listContainer.GetChild(i).gameObject, PopulateList));
                }
                else
                {
                    StartCoroutine(DestroyGameObject(listContainer.GetChild(i).gameObject, null));
                }
            }
        }
    }

    private void PopulateList()
    {
        foreach (ListItemInformationGroup itemInfo in listItems)
        {
            GameObject newItem = GameObject.Instantiate(listItemPrefab, listContainer);
            newItem.name = itemInfo.label;
            newItem.GetComponentInChildren<Text>().text = itemInfo.label;
            newItem.GetComponent<Button>().onClick.AddListener(delegate () { itemInfo.onClick.Invoke(); });
        }
    }

    private IEnumerator DestroyGameObject(GameObject go, CallBack callBack)
    {
        yield return new WaitForEndOfFrame();
        DestroyImmediate(go);
        if (callBack != null) callBack();
    }
}
