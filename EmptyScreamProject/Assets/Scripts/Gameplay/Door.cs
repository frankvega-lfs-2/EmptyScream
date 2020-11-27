using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Door : Interactable
{
    public enum DoorType
    {
        defaultDoor,
        automaticDoor,
        singleUseDoor,
        maxDoors
    }

    public DoorType doorType;

    [Header("Single use Door settings")]
    public GameObject door;
    public GameObject[] pickupsColliders;

    public OcclusionPortal occlusionPortal;
    public bool isOpen;
    public bool isLocked;
    private Animator animator;
    public Trigger doorTrigger;

    public int cantEnemigos;
    public bool isInCombatRoom;
    public bool doOnce;

    public bool enemyInTrigger;
    public bool wasEnemy;

    [Header("Hologram Settings"), Space]
    public GameObject panelObject;
    public GameObject unlockedScreen;
    public GameObject lockedScreen;
    public GameObject unlockedScreen2;
    public GameObject lockedScreen2;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).GetComponent<Trigger>())
            {
                doorTrigger = transform.GetChild(i).GetComponent<Trigger>();
            }
        }
        if(doorTrigger)
        {
            doorTrigger.OnEnter += CheckOnTriggerEnter;
            doorTrigger.OnExit += CheckOnTriggerExit;
        }
        if(doorType != DoorType.automaticDoor)
        {
            isOpen = true;
        }
        
        canInteract = true;
        //isLocked = false;
        animator = GetComponent<Animator>();
        OnInteract += InteractDoor;

        if(animator)
        {
            animator.SetBool("Close", false);
            animator.SetBool("Open", false);
        }

        for (int i = 0; i < pickupsColliders.Length; i++)
        {
            pickupsColliders[i].SetActive(false);
        }

        isInCombatRoom = false;
        cantEnemigos = 2;
        EnemyController.OnEnemyDeath += EnemyDied;
        if(isLocked)
        {
            if(panelObject)
            {
                unlockedScreen.SetActive(false);
                lockedScreen.SetActive(true);
                unlockedScreen2.SetActive(false);
                lockedScreen2.SetActive(true);
            }
        }
        else
        {
            if(panelObject)
            {
                panelObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // lo dejo por si queres hacer que una puerta se cierre hasta matar cierta cantidad de enemigos
        //if (cantEnemigos>0 && isInCombatRoom)
        //{
        //    if (!doOnce)
        //    {
        //        doorType = DoorType.defaultDoor;
        //        animator.SetBool("Close", true);
        //        animator.SetBool("Open", false);
        //        doOnce = true;
        //    }
            
        //}
        //else
        //{
        //    if (doOnce)
        //    {
        //        doorType = DoorType.automaticDoor;
        //        isOpen = false;
        //        doOnce=false;
        //    }
        //}
    }

    void EnemyDiedOnTrigger()
    {
        enemyInTrigger = false;
    }

    void EnemyDied()
    {
        Invoke("EnemyDiedOnTrigger", 1.0f);
    }
    public void InteractDoor()
    {
        if(canInteract)
        {
            if(!isLocked)
            {
                if(doorType != DoorType.singleUseDoor)
                {
                    isOpen = !isOpen;

                    if (isOpen)
                    {
                        animator.SetBool("Close", true);
                        animator.SetBool("Open", false);
                        canInteract = false;

                        // Close
                    }
                    else
                    {
                        panelObject.SetActive(false);

                        animator.SetBool("Open", true);
                        animator.SetBool("Close", false);
                        canInteract = false;

                        // Open
                    }
                }
                else
                {
                    if(door.activeSelf)
                    {
                        for (int i = 0; i < pickupsColliders.Length; i++)
                        {
                            pickupsColliders[i].SetActive(true);
                        }
                        door.SetActive(false);
                    }
                    
                }

                
            }
            
        }

    }

    public void EnableInteract()
    {
        canInteract = true;
        //ClosePortal();
    }

    private void CheckOnTriggerEnter(GameObject GO)
    {
        if (doorType == DoorType.automaticDoor)
        {
            if(!isLocked)
            {
                switch (GO.tag)
                {
                    case "Player":
                        {
                            if (!isOpen)
                            {
                                animator.SetBool("Open", true);
                                animator.SetBool("Close", false);
                                canInteract = false;
                                isOpen = true;
                                wasEnemy = false;
                                if (panelObject)
                                    panelObject.SetActive(false);
                            }

                        }
                        break;
                    case "enemy":
                        {
                            enemyInTrigger = true;
                            if (!isOpen)
                            {
                                animator.SetBool("Open", true);
                                animator.SetBool("Close", false);
                                canInteract = false;
                                isOpen = true;
                                wasEnemy = true;
                                if (panelObject)
                                    panelObject.SetActive(false);
                            }


                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                //Play Locked Sound
                switch (GO.tag)
                {
                    case "enemy":
                        {
                            if(isLocked)
                            {
                                if(!GO.GetComponent<EnemyController>().sight.playerInSight)
                                {
                                    GO.GetComponent<EnemyController>().ChangeState(EnemyController.States.Idle);
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            
        }
    }

    private void CheckOnTriggerExit(GameObject GO)
    {
        if (doorType == DoorType.automaticDoor)
        {
            if (!isLocked)
            {
                switch (GO.tag)
                {
                    case "Player":
                        {
                            if (isOpen && !enemyInTrigger)
                            {
                                animator.SetBool("Close", true);
                                animator.SetBool("Open", false);
                                canInteract = false;
                                isOpen = false;
                                wasEnemy = false;
                                if (panelObject)
                                    panelObject.SetActive(true);
                            }

                        }
                        break;
                    case "enemy":
                        {
                            enemyInTrigger = false;
                            if (isOpen)
                            {
                                animator.SetBool("Close", true);
                                animator.SetBool("Open", false);
                                canInteract = false;
                                isOpen = false;
                                wasEnemy = true;
                                if(panelObject)
                                panelObject.SetActive(true);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // Play Locked Sound
            }
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckOnTriggerEnter(other.gameObject);
    }

    public bool CheckInteract()
    {
        if(!isLocked)
        {
            return true;
        }

        return false;
        //return -1;
    }

    private void OnTriggerExit(Collider other)
    {
        CheckOnTriggerExit(other.gameObject);
    }

    public void OpenPortal()
    {
        if (occlusionPortal)
        {
            occlusionPortal.open = true;
        }
    }

    public void ClosePortal()
    {
        if(!wasEnemy)
        {
            if (occlusionPortal)
            {
                occlusionPortal.open = false;
            }
        }
        
    }

    public void PlayMechanicDoorSound()
    {
        AkSoundEngine.PostEvent("Door_action", this.gameObject);
        //OpenPortal();
    }

    public void PlayOpenDoorSound()
    {
        AkSoundEngine.PostEvent("Lockers_open", this.gameObject);
    }

    public void PlayCloseDoorSound()
    {
        AkSoundEngine.PostEvent("Lockers_close", this.gameObject);
    }

    public void LockState(bool lockState)
    {
        isLocked = lockState;

        if(isLocked)
        {
            unlockedScreen.SetActive(false);
            lockedScreen.SetActive(true);
            unlockedScreen2.SetActive(false);
            lockedScreen2.SetActive(true);
        }
        else
        {
            unlockedScreen.SetActive(true);
            lockedScreen.SetActive(false);
            unlockedScreen2.SetActive(true);
            lockedScreen2.SetActive(false);
        }
    }


    private void OnDestroy()
    {
        OnInteract -= InteractDoor;
        EnemyController.OnEnemyDeath -= EnemyDied;
        if (doorTrigger)
        {
            doorTrigger.OnEnter -= CheckOnTriggerEnter;
            doorTrigger.OnExit -= CheckOnTriggerExit;
        }
    }
}
