using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPrefab : MonoBehaviour
{
    public Ball ballScript;
    public GameObject ballPrefab;
    public float speed;
    public Rigidbody2D rb;
    public Vector3 StartPosition;

    void Start()
    {
        GameObject ballObject = Instantiate(ballPrefab);
        ballScript = ballObject.GetComponent<Ball>();
    }

    public void Reset()
    {
        rb.linearVelocity = Vector3.zero;
        transform.position = new Vector3(0, -6.23f, 0);
    }

    public void ResetBall()
    {
        ballScript.Reset();
        //ballScript.Launch();

        // TODO: Дописать метод сброса мяча.
        // 1. Вызвать ballScript.Reset() (этот метод уже есть в Ball).
        // 2. Сгенерировать случайное направление (x и y = ±1).
        // 3. Установить rb.linearVelocity = new Vector3(speed * x, speed * y).
    }
}