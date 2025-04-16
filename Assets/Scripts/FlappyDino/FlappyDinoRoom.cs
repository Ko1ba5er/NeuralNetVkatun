using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class FlappyDinoRoom : Room
{
    static List<Transform> obstacles = new();
    public static float speed;

    [Space]
    [SerializeField] Transform obstacle;

    public static float NearestObstacleX()
    {
        if (obstacles.Count > 0)
            return obstacles.Min(ob => ob.position.x);

        return 1000;
    }

    public static float NearestObstacleY()
    {
        if (obstacles.Count > 0)
            return obstacles.OrderBy(ob => ob.position.x).First().position.y;

        return 1000;
    }

    protected override void Restart()
    {
        base.Restart();

        foreach (Transform ob in obstacles)
            Destroy(ob.gameObject);

        obstacles.Clear();

        speed = 200;
        StopAllCoroutines();
        StartCoroutine(obSpammer());
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        obstacles = obstacles.Where(ob => !ob.IsDestroyed()).ToList();
        foreach (Transform ob in obstacles)
        {
            ob.position += Vector3.left * Time.fixedDeltaTime * speed;
            if (ob.transform.position.x <= 0)
            {
                Destroy(ob.gameObject);
                foreach (NNAgent d in agents)
                    if (!d.dead)
                        d.score += 5;
            }
        }

        foreach (NNAgent d in agents)
            if (!d.dead)
                d.score += Time.fixedDeltaTime;

        speed += Time.fixedDeltaTime * 1f;
    }

    IEnumerator obSpammer()
    {
        while (true)
        {
            obstacles.Add(Instantiate(obstacle, new Vector3(2000, 200 + rnd.Next((int)((RectTransform)transform).rect.size.y / 2), 0), Quaternion.identity, transform));
            
            yield return new WaitForSeconds(2);
        }
    }
}