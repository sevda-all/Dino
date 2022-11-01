using UnityEngine;

public class Bonus : GeneratedObject
{
    public int BonusPoints = 10;
    private float _speed = 7;

    void Update()
    {
        transform.position += Vector3.left * ((IsMooving ? _speed : 0) * Time.deltaTime);
        if (!(transform.position.x < - (PlatformController.screenBounds.x + 2.5)))
            return;
        gameObject.SetActive(false);
    }
}
