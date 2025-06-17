using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SwipeStage_Selector : MonoBehaviour
{
    public RectTransform sampleListItem;
    public RectTransform contentPanel;
    public ScrollRect scrollRect;

    public HorizontalLayoutGroup HLG;
    public TMP_Text NameLabel;
    public string[] itemNames;

    public float snapForce;
    float snapSpeed;
    bool isSnapped;

    public StageManager stageManager;
    public enum ScrollView_Type { StageSelector, LevelSelector }
    public ScrollView_Type selectorType;


    private void Start()
    {
        isSnapped = false;
        //AutoFillNamesFromData();
    }

    void Update()
    {
        ScrollLoggic();
    }

    private void ScrollLoggic()
    {
        int currentItem = Mathf.RoundToInt((0 - contentPanel.localPosition.x / (sampleListItem.rect.width + HLG.spacing)));
        currentItem = Mathf.Clamp(currentItem, 0, itemNames.Length - 1);

        if (scrollRect.velocity.magnitude < 200 && !isSnapped)
        {
            scrollRect.velocity = Vector3.zero;
            snapSpeed += snapForce * Time.deltaTime;

            float targetX = 0 - (currentItem * (sampleListItem.rect.width + HLG.spacing));
            contentPanel.localPosition = new Vector3(
                Mathf.MoveTowards(contentPanel.localPosition.x, targetX, snapSpeed),
                contentPanel.localPosition.y,
                contentPanel.localPosition.z);

            NameLabel.text = itemNames[currentItem];

            switch (selectorType)
            {
                case ScrollView_Type.StageSelector:
                    if (stageManager.currentStageIndex != currentItem)
                    {
                        print("this is StageSelector");
                    }
                    break;

                case ScrollView_Type.LevelSelector:
                    if (stageManager.currentStageIndex != currentItem)
                    {
                        print("this is LevelSelector");
                    }
                    break;
            }

            if (contentPanel.localPosition.x == targetX)
                isSnapped = true;
        }

        if (scrollRect.velocity.magnitude > 200)
        {
            NameLabel.text = "_________";
            isSnapped = false;
            snapSpeed = 0;
        }
    }
    /*public void AutoFillNamesFromData()
    {
        switch (selectorType)
        {
            case ScrollView_Type.StageSelector:
                itemNames = new string[stageManager.customizationDatabase.taxiOptions.Count];
                for (int i = 0; i < itemNames.Length; i++)
                    itemNames[i] = stageManager.customizationDatabase.taxiOptions[i].taxiPrefab.name;
                break;

            case ScrollView_Type.LevelSelector:
                var bodyMaterials = stageManager.customizationDatabase.taxiOptions[stageManager.currentModelIndex].bodyMaterials;
                itemNames = new string[bodyMaterials.Count];
                for (int i = 0; i < itemNames.Length; i++)
                    itemNames[i] = bodyMaterials[i].name;
                break;

            case ScrollView_Type.SouthAfrica:
                var decals = stageManager.customizationDatabase.taxiOptions[stageManager.currentModelIndex].decalMaterials;
                itemNames = new string[decals.Count];
                for (int i = 0; i < itemNames.Length; i++)
                    itemNames[i] = decals[i].name;
                break;
        }
    }*/
}
