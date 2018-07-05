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

    public delegate void OnClickEvent();

    private void Start()
    {
        RefreshList();
    }
    
    public void RefreshList()
    {
        if (!listContainer) return;

        for (int i = listContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(listContainer.GetChild(i).gameObject);
        }

        foreach (ListItemInformationGroup itemInfo in listItems)
        {
            GameObject newItem = Instantiate(listItemPrefab, listContainer);
            newItem.name = itemInfo.label;
            newItem.GetComponentInChildren<Text>().text = itemInfo.label;
            if (itemInfo.onClick != null)
            {
                newItem.GetComponent<Button>().onClick.AddListener(delegate () { itemInfo.onClick.Invoke(); });
            }
        }
    }

    public void Clear()
    {
        listItems.Clear();
        RefreshList();
    }

    public void AddItem(String label, OnClickEvent onClickEvent)
    {
        ListItemInformationGroup newItem = new ListItemInformationGroup();
        newItem.label = label;
        if (onClickEvent != null)
        {
            newItem.onClick.AddListener(delegate () { onClickEvent(); });
        }
        listItems.Add(newItem);
    }
}
