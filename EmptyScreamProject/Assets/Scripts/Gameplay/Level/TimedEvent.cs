using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimedEvent : Interactable
{
    [Header("Main Settings")]
    public float timeForTrigger;
    public GameObject lockedState;
    public GameObject unlockedState;
    public GameObject interactableTriggerToOpen;

    [Header("UI Settings")]
    public TextMeshProUGUI terminalTextTimer;
    public TextMeshProUGUI objetivesTimerText;

    private bool isTimerEnabled;
    private float currentTimer;
    private Button button;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        button = GetComponent<Button>();
        OnInteract += ExecuteAction;
        lockedState.SetActive(true);
        unlockedState.SetActive(false);
        interactableTriggerToOpen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(isTimerEnabled)
        {
            currentTimer -= Time.deltaTime;
            terminalTextTimer.text = ((int)currentTimer).ToString();
            if(objetivesTimerText)
            {
                objetivesTimerText.text = ((int)currentTimer).ToString();
            }

            if(currentTimer <= 0)
            {
                isTimerEnabled = false;
                currentTimer = timeForTrigger;
                lockedState.SetActive(false);
                unlockedState.SetActive(true);
                interactableTriggerToOpen.SetActive(true);
                terminalTextTimer.text = "Unlocked";
                if (objetivesTimerText)
                {
                    objetivesTimerText.text = "Unlocked";
                }
            }
        }
    }

    private void ExecuteAction()
    {
        if (canInteract)
        {
            if(!isTimerEnabled)
            {
                canInteract = false;
                button.onClick.Invoke();
            }
            
        }
    }

    public void StartTimer()
    {
        if (!isTimerEnabled)
        {
            currentTimer = timeForTrigger;
            isTimerEnabled = true;
        }
        
    }

    private void OnDestroy()
    {
        OnInteract -= ExecuteAction;
    }
}
