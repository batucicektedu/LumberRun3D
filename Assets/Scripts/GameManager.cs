using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;


public class GameManager : MonoBehaviour , CharacterControllerUiListener //TODO ADD CharacterListener
{
    public static GameManager Instance { private set; get; }

    //Game Settings
    public int defaultScoreMultiplier;
    [Header("Character")]
    public float forwardSpeed;
    public float sidewaysSpeed;
    public float maxOffset;

    public float rotationResetTweenDuration;
    public float rotateTweenDuration;
    public float rotationMultiplier;

    public float constantFallDownSpeed;

    [Header("BridgeBuilding")]
    public float delayBetweenBridges;
    public int decreaseWoodPerBridge;

    [Header("LadderBuilding")]
    public int decraseWoodPerLadder;

    [Header("TreeCollision")]
    public bool applyForce;
    public float appliedForce;

    [Header("RaftLocation")]
    public int raftWoodCount;

    [Header("FollowingCollectable")]
    [Tooltip("Kütüklerin takip etme hızı")]
    public float collectableSpeed;
    public float followStartDelay;
    public float woodIncrementPerTree;
    public float oneWoodsEqualCollectableWoodCount;

    //Game Variables
    [Space(20f)]
    public bool _gameStopped = true;
    public int carriedWoodCount;
    public float remainderWoodCount;
    public bool gameStarted;
    public bool startMovingDownAfterFail;
    public int lastTouchedLevelEndIndex;
    private bool canMoveToLevelEndCylindersMiddle;
    public bool levelEndReached;

    [Header("PersistingVariables")]
    public int totalGain;
    public int levelFinalGain;
    public int currentLevel;
    public int enviromentIndex;
    public int skyBoxColorIndex;
    public bool levelEndPlayerRelocationStarted;
    
    

    //Connections
    public TextMeshProUGUI _totalGain;
    public TextMeshProUGUI _levelFinalGain;
    public TextMeshProUGUI _Multipler;
    public TextMeshProUGUI levelCounter;
    public UIManager uIManager;
    public CharacterController character;
    public GameObject[] woods;
    public GameObject[] levels;
    public Material[] skyBoxMetarials;
    private LevelEndCylinder[] levelEndCylinders;

    public CameraFollow cameraFollowScript;
    

    private Animator playerAnimator;


    private void Awake()
    {
        if(Instance != this && Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        SetLevel();

        InitConnections();
        InitState();
        
        //ChangeSkyBox();
    }

    private void LateUpdate()
    {
        if (canMoveToLevelEndCylindersMiddle)
        {
            Vector3 middleOfEndCylider = levelEndCylinders[lastTouchedLevelEndIndex].transform.position;
            Vector3 vel = Vector3.zero;

            character.transform.position = Vector3.SmoothDamp(character.transform.position, new Vector3(middleOfEndCylider.x, character.transform.position.y, middleOfEndCylider.z), ref vel, 0.1f);

        }
    }

    private void InitConnections()
    {
        character.SetListener(this);
        playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();

        levelEndCylinders = GameObject.Find("Level_end").GetComponentsInChildren<LevelEndCylinder>();
    }

    private void InitState()
    {
        gameStarted = false;
        _gameStopped = true;
        startMovingDownAfterFail = false;
        levelEndReached = false;
        canMoveToLevelEndCylindersMiddle = false;


        lastTouchedLevelEndIndex = -1;

        if (carriedWoodCount > 0)
        {
            DisableWoods(carriedWoodCount);
        }

        uIManager.ActivateLevelStartUI();
        uIManager.DisableLevelCompleteUI();
        uIManager.DisableLevelFailedUI();


        totalGain = PlayerPrefs.GetInt("totalGain", 0);
        //_totalGain.text = "" + totalGain;
        PlayerPrefs.SetInt("totalGain", totalGain);
        
    }

    public void SetLevel()
    {
        currentLevel = PlayerPrefs.GetInt("currentLevel", currentLevel);
        int levelPrefabIndex = currentLevel;
        //levelCounter.text = "LEVEL " + (currentLevel + 1);
        if (levelPrefabIndex >= levels.Length)
        {
            levelPrefabIndex %= levels.Length;
        }
        GameObject createdLevel = Instantiate(levels[levelPrefabIndex]);

    }

    public void ChangeSkyBox()
    {
        skyBoxColorIndex = PlayerPrefs.GetInt("skyBoxColorIndex", 0);
        int levelSkyBoxIndex = skyBoxColorIndex;
        if (levelSkyBoxIndex >= skyBoxMetarials.Length)
        {
            levelSkyBoxIndex %= skyBoxMetarials.Length;
        }
        Material selectedMaterial = skyBoxMetarials[levelSkyBoxIndex];
        RenderSettings.skybox = selectedMaterial;

    }

    public void IncreaseCarriedWoodCount(float woodIncremenetCount)
    {
        EnableWoods((int)(woodIncremenetCount / oneWoodsEqualCollectableWoodCount));

        remainderWoodCount += woodIncremenetCount % oneWoodsEqualCollectableWoodCount;

        if(remainderWoodCount >= oneWoodsEqualCollectableWoodCount)
        {
            remainderWoodCount -= oneWoodsEqualCollectableWoodCount;
            EnableWoods(1);
        }
    }

    public void EnableWoods(int count)
    {
        int startIndex = carriedWoodCount;

        if(carriedWoodCount + count < woods.Length)
        {
            for (int i = startIndex; i < startIndex + count; i++)
            {
                woods[i].GetComponent<MeshRenderer>().enabled = true;
                carriedWoodCount++;
            }
        }
        else
        {
            for (int i = startIndex; i < woods.Length; i++)
            {
                woods[i].GetComponent<MeshRenderer>().enabled = true;
                carriedWoodCount++;
            }
        }

    }

    

    public void OnComplete()
    {
        print("<color=green>Level Complete !</color>");

        levelFinalGain = (int)((defaultScoreMultiplier + (defaultScoreMultiplier * currentLevel * 0.1f)) * carriedWoodCount);

        currentLevel++;
        enviromentIndex++;
        skyBoxColorIndex++;

        //_levelFinalGain.text = "" + _levelFinalGain;

        cameraFollowScript.offset = new Vector3(0, 2.91f, -2.81f);

        playerAnimator.SetTrigger("LevelCompleted");

        uIManager.ActivateLevelCompleteUI();

        if (carriedWoodCount > 0)
        {
            DisableWoods(carriedWoodCount);
        }

        totalGain += levelFinalGain;

        PlayerPrefs.SetInt("currentLevel", currentLevel);
        PlayerPrefs.SetInt(" enviromentIndex", enviromentIndex);
        PlayerPrefs.SetInt("skyBoxColorIndex", skyBoxColorIndex);
        PlayerPrefs.SetInt("totalGain", totalGain);
    }

    public void OnFail()
    {
        print("<color=red>Failed !</color>");

        _gameStopped = true;

        for (int i = 0; i < woods.Length; i++)
        {
            Destroy(woods[i].GetComponent<FixedJoint>());

            Rigidbody r = woods[i].GetComponent<Rigidbody>();

            woods[i].GetComponent<CapsuleCollider>().height *= 3;

            r.mass = 1;
            r.drag = 0;
            r.angularDrag = 0.05f;
            r.constraints = RigidbodyConstraints.None;

        }

        if (!character.trackUnderneath)
        {
            startMovingDownAfterFail = true;

            //character.GetComponentInParent<Transform>().DOMoveY(character.GetComponentInParent<Transform>().position.y - 20f, 10f);
            cameraFollowScript.enabled = false;
        }

        playerAnimator.SetTrigger("LevelFailed");
        uIManager.ActivateLevelFailedUI();
    }

    public void OnCoin()
    {
        
    }

    public void DisableWoods(int count)
    {
        int startIndex = carriedWoodCount - 1;

        StartCoroutine(DisableWoodsWithDelays(startIndex, count));
    }

    private IEnumerator DisableWoodsWithDelays(int startIndex, int count)
    {
        if (carriedWoodCount - count >= 0)
        {
            for (int i = startIndex; i > startIndex - count; i--)
            {
                woods[i].GetComponent<MeshRenderer>().enabled = false;
                carriedWoodCount--;

                yield return new WaitForSeconds(0.15f);
            }
        }
        else
        {
            for (int i = startIndex; i > startIndex - count; i--)
            {
                if (i < 0)
                {
                    break;
                }

                woods[i].GetComponent<MeshRenderer>().enabled = false;
                carriedWoodCount--;

                yield return new WaitForSeconds(0.15f);
            }

            //We Failed
            OnFail();
        }
    }

    public void StartLevel()
    {
        if (!gameStarted)
        {
            _gameStopped = false;
            gameStarted = true;

            playerAnimator.SetTrigger("LevelStarted");
            uIManager.DisableLevelStartUI();
        }
    }

    public void EndLevel()
    {
        OnComplete();
        _gameStopped = true;



        foreach (GameObject item in character.axes)
        {
            item.SetActive(false);
        }

        canMoveToLevelEndCylindersMiddle = true;

        character.transform.Rotate(new Vector3(0, 180f, 0));
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
