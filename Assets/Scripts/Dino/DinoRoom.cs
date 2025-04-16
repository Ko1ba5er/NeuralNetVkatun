using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DinoRoom : Room
{
    [SerializeField] private Sprite Pterodaktyl;

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

    public static float NearestObstacleType()
    {
        if (obstacles.Count > 0)
            return obstacles.OrderBy(ob => ob.position.x).First().position.y > 100 ? 1 : 0;

        return 1000;
    }

    protected override void Restart()
    {
        base.Restart();

        foreach (Transform ob in obstacles)
            Destroy(ob.gameObject);

        obstacles.Clear();

        speed = 400;
        StopAllCoroutines();
        StartCoroutine(obSpammer());
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        obstacles = obstacles.Where(ob => !ob.IsDestroyed()).ToList();
        foreach(Transform ob in obstacles)
        {
            ob.position += Vector3.left * Time.fixedDeltaTime * speed;
            if (ob.transform.position.x <= 0)
            {
                Destroy(ob.gameObject);
                foreach (NNAgent a in agents)
                    if (!a.dead)
                        a.score += 5;
            }
        }

        speed += Time.fixedDeltaTime * 1f;
    }

    IEnumerator obSpammer()
    {
        while (true)
        {
            if (rnd.Next(2) == 0)
                obstacles.Add(Instantiate(obstacle, new Vector3(3000, 100, 0), Quaternion.identity, transform));
            else
            {
                Transform tr = Instantiate(obstacle, new Vector3(3000, 200, 0), Quaternion.identity, transform);
                obstacles.Add(tr);
                tr.GetComponent<Image>().sprite = Pterodaktyl;
            }
            yield return new WaitForSeconds(2);
        }
    }
}