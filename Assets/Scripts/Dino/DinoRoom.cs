using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class DinoRoom : MonoBehaviour
{
    [SerializeField] private Sprite Pterodaktyl;
    [Space]

    [SerializeField] private TMP_Text genText;
    [SerializeField] private TMP_Text ScoreText;
    [SerializeField] private int generation = 0;
    List<Dino> dinos = new ();
    static List<Transform> obstacles = new();
    public static float speed;
    private int maxScore = 0;

    [Space]
    [SerializeField] private float mutationPower = 0.1f;
    [SerializeField] private float mutationAmount = 10f;

    [Space]
    [SerializeField] Transform obstacle;
    [SerializeField] Dino dinoPrefab;

    System.Random rnd = new System.Random();

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

    private void Start()
    {
        for(int i = 0; i < 50; i++)
            dinos.Add(Instantiate(dinoPrefab, new Vector3(100, 100, 0), Quaternion.identity, transform));

        foreach (Dino d in dinos)
            d.GetComponent<Image>().color = new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());

        Restart();
    }

    private void Restart()
    {
        genText.text = "Поколение: " + ++generation;
        if (dinos.Max(dinos => dinos.score) > maxScore)
            maxScore = dinos.Max(dinos => dinos.score);

        foreach (Transform ob in obstacles)
            Destroy(ob.gameObject);

        obstacles.Clear();

        IEnumerable<Dino> BestDinos = dinos.OrderByDescending(dino => dino.score).Take(dinos.Count / 2);
        List<Color> bestColors = BestDinos.Select(d => d.GetComponent<Image>().color).ToList();
        List<NeuralNetwork> bestBrains = BestDinos.Select(d => d.Brain).ToList();

        Random rnd = new Random();
        for (int i = 0; i < dinos.Count(); i++)
        {
            dinos[i].transform.position = new Vector3(100, 100, 0);
            dinos[i].dead = false;
            dinos[i].rb.simulated = true;
            dinos[i].score = 0;
            dinos[i].Brain = bestBrains.ElementAt(i / 2).Mutate(mutationPower, mutationAmount);
            dinos[i].GetComponent<Image>().color = bestColors.ElementAt(i / 2);
        }

        speed = 400;
        StopAllCoroutines();
        StartCoroutine(obSpammer());
    }

    private void FixedUpdate()
    {
        ScoreText.text = "Счет: " + dinos.Max(d => d.score) + "; макс.: " + maxScore;

        obstacles = obstacles.Where(ob => !ob.IsDestroyed()).ToList();
        foreach(Transform ob in obstacles)
        {
            ob.position += Vector3.left * Time.fixedDeltaTime * speed;
            if (ob.transform.position.x <= 0)
            {
                Destroy(ob.gameObject);
                foreach (Dino d in dinos)
                    if (!d.dead)
                        d.score += 5;
            }
        }

        speed += Time.fixedDeltaTime * 1f;

        if (dinos.All(d => d.dead))
            Restart();
    }

    IEnumerator obSpammer()
    {
        while (true)
        {
            if (rnd.Next(2) == 0)
                obstacles.Add(Instantiate(obstacle, new Vector3(1080, 100, 0), Quaternion.identity, transform));
            else
            {
                Transform tr = Instantiate(obstacle, new Vector3(1080, 200, 0), Quaternion.identity, transform);
                obstacles.Add(tr);
                tr.GetComponent<Image>().sprite = Pterodaktyl;
            }
            yield return new WaitForSeconds(2);
        }
    }
}