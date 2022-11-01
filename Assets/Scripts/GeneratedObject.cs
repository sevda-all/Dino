using UnityEngine;

public abstract class GeneratedObject : MonoBehaviour, IPoolingObject
{
    private void Start()
    {
        EventManager.Instance.AddListener(EventType.PlayerDied, OnEvent);
        EventManager.Instance.AddListener(EventType.GameRestart, OnEvent);
    }

    private void OnEvent(EventType eventType, Component sender, object param)
    {
        switch (eventType)
        {
            case EventType.GameRestart:
                IsMooving = true;
                break;
            case EventType.PlayerDied:
                IsMooving = false;
                break;
        }
    }

    public static bool IsMooving; 
    public virtual bool IsFree => !gameObject.activeSelf;

    public virtual void OnObjectSpawn()
    {
        //Do nothing
    }
}