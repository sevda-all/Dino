using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlatformController : MonoBehaviour
{
    [SerializeField] private List<Platform> Platforms;
    [SerializeField] [Range(0, 100)] private float ObstacleAppearancesFrequency = 50;
    [SerializeField] [Range(0, 100)] private float BonusAppearancesFrequency = 50;

    private Platform _lastGeneratedPlatform;
    public static Vector2 screenBounds;
    private EventManager _EventManager;
    private ObjectPooler _ObjectPooler;

    private void Awake()
    {
        _EventManager= new EventManager();
        _ObjectPooler = new ObjectPooler();
    }

    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        _lastGeneratedPlatform = ObjectPooler.Instance.SpawnFromPool(
            Platforms[0].gameObject,
            new Vector3(27.5f, -11.8f, 0), 30
            ).GetComponent<Platform>();
        EventManager.Instance.AddListener(EventType.PlatformReachesScreenBound, OnEvent);
        EventManager.Instance.AddListener(EventType.GameRestart, OnEvent);
        
        Platform.Obstacles = Resources.LoadAll<GameObject>("Obstacles/GameObjects");
        Platform.Bonuses = Resources.LoadAll<GameObject>("Bonus");
        GeneratedObject.IsMooving = false;
    }
    
    private void OnEvent(EventType eventType, Component sender, object param)
    {
        switch (eventType)
        {
            case EventType.PlatformReachesScreenBound:
                OnPlatformGone();
                break;
        }
    }

    private void OnPlatformGone()
    {
        GameObject platform = Platforms[0].gameObject;

        switch (_lastGeneratedPlatform.PlatformType)
        {
            case PlatformType.Base:
                if (Random.Range(0, 100) < 10)
                {
                    platform = Platforms[1].gameObject;
                }
                break;
            case PlatformType.LeftEnd:
                if (Random.Range(0, 100) < 5)
                {
                    platform = Platforms[1].gameObject;
                }
                break;
            case PlatformType.RightEnd:
                platform = Platforms[2].gameObject;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        _lastGeneratedPlatform = ObjectPooler.Instance.SpawnFromPool(platform,
            _lastGeneratedPlatform.gameObject.transform.position + Vector3.right * 2.5f).GetComponent<Platform>();
        _lastGeneratedPlatform.gameObject.SetActive(true);

        _lastGeneratedPlatform.ShouldObstacleBeSpawned = Random.Range(0, 100) < ObstacleAppearancesFrequency;
        _lastGeneratedPlatform.ShouldBonusBeSpawned = Random.Range(0, 100) < BonusAppearancesFrequency;
    }
}
