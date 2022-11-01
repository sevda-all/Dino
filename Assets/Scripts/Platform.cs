using UnityEngine;

public class Platform : GeneratedObject
{
    public PlatformType PlatformType;
    public bool ShouldObstacleBeSpawned;
    public bool ShouldBonusBeSpawned;
    public static GameObject[] Obstacles;
    public static GameObject[] Bonuses;

    private float _speed = 5;
    void Update()
    {
        transform.position += Vector3.left * ((IsMooving ? _speed : 0) * Time.deltaTime);
        if (!(transform.position.x < - (PlatformController.screenBounds.x + 2.5)))
            return;
        EventManager.Instance.PostNotification(EventType.PlatformReachesScreenBound, this);
        gameObject.SetActive(false);
    }
    public override void OnObjectSpawn()
    {
        if (ShouldObstacleBeSpawned)
        {
            var obstacle = ObjectPooler.Instance.SpawnFromPool(Obstacles[Random.Range(0, Obstacles.Length)], transform.position);
            ShouldObstacleBeSpawned = false;
            obstacle.gameObject.SetActive(true);
        }

        if (ShouldBonusBeSpawned)
        {
            var bonus = ObjectPooler.Instance.SpawnFromPool(Bonuses[Random.Range(0, Bonuses.Length)], transform.position+Vector3.up*Random.Range(0,6));
            ShouldObstacleBeSpawned = false;
            bonus.gameObject.SetActive(true);    
        }
    }
}

public enum PlatformType
{
    Base,
    LeftEnd,
    RightEnd
}
