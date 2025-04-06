using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class FlappyDinoRoom : MonoBehaviour
{
    [SerializeField] ConvergenceChart ConChart;
    [SerializeField] private List<float> convergence;
    [Space]

    [SerializeField] private TMP_Text genText;
    [SerializeField] private TMP_Text ScoreText;
    private int generation = 0;
    List<FlappyDino> dinos = new();
    static List<Transform> obstacles = new();
    public static float speed;
    private float maxScore = 0;

    [Space]
    [SerializeField] private float mutationPower = 0.1f;
    [SerializeField] private float mutationAmount = 10f;

    [Space]
    [SerializeField] Transform obstacle;
    [SerializeField] FlappyDino dinoPrefab;

    Random rnd = new();

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

    private void Start()
    {
        for (int i = 0; i < 50; i++)
            dinos.Add(Instantiate(dinoPrefab, new Vector3(500, 1000, 0), Quaternion.identity, transform));

        foreach (FlappyDino d in dinos)
            d.GetComponent<Image>().color = new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());

        Restart();
    }

    private void Restart()
    {
        genText.text = "Поколение: " + ++generation;
        if (dinos.Max(dinos => dinos.score) > maxScore)
            maxScore = dinos.Max(dinos => dinos.score);

        convergence.Add(dinos.Max(dinos => dinos.score));
        ConChart.UpdateChart(convergence);

        foreach (Transform ob in obstacles)
            Destroy(ob.gameObject);

        obstacles.Clear();

        IEnumerable<FlappyDino> BestDinos = dinos.OrderByDescending(dino => dino.score).Take(dinos.Count / 2);
        List<Color> bestColors = BestDinos.Select(d => d.GetComponent<Image>().color).ToList();
        List<NeuralNetwork> bestBrains = BestDinos.Select(d => d.Brain).ToList();

        Random rnd = new Random();
        for (int i = 0; i < dinos.Count(); i++)
        {
            if (i < bestBrains.Count)
                dinos[i].Revive(bestBrains[i], bestColors.ElementAt(i % bestColors.Count));
            else
                dinos[i].Revive(bestBrains[i - bestBrains.Count].Mutate(mutationPower, mutationAmount), bestColors.ElementAt(i % bestColors.Count));
        }

        speed = 200;
        StopAllCoroutines();
        StartCoroutine(obSpammer());
    }

    private void FixedUpdate()
    {
        ScoreText.text = "Счет: " + dinos.Max(d => d.score) + "; макс.: " + maxScore;

        obstacles = obstacles.Where(ob => !ob.IsDestroyed()).ToList();
        foreach (Transform ob in obstacles)
        {
            ob.position += Vector3.left * Time.fixedDeltaTime * speed;
            if (ob.transform.position.x <= 0)
            {
                Destroy(ob.gameObject);
                foreach (FlappyDino d in dinos)
                    if (!d.dead)
                        d.score += 5;
            }
        }

        foreach (FlappyDino d in dinos)
            if (!d.dead)
                d.score += Time.fixedDeltaTime;

        speed += Time.fixedDeltaTime * 1f;

        if (dinos.All(d => d.dead))
            Restart();
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