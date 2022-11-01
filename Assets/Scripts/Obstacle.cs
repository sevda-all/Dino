using UnityEngine;

public class Obstacle : GeneratedObject
{
    private float _speed = 5;
    void Update()
    {
        transform.position += Vector3.left * ((IsMooving ? _speed : 0) * Time.deltaTime);
        if (!(transform.position.x < -(PlatformController.screenBounds.x + 2.5)))
            return;
        gameObject.SetActive(false);
    }
}
