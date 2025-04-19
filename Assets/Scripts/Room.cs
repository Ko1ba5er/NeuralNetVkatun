using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class Room : MonoBehaviour
{
    [SerializeField] private ConvergenceChart ConChart;
    [SerializeField] private List<float> convergence;
    [Space]

    [SerializeField] private TMP_Text genText;
    [SerializeField] private TMP_Text ScoreText;
    private int generation = 0;
    protected List<NNAgent> agents = new ();
    public ReadOnlyCollection<NNAgent> Agents => new ReadOnlyCollection<NNAgent>(agents);
    private float maxScore = 0;

    [Space]
    [SerializeField] private float mutationPower = 0.1f;
    [SerializeField] private float mutationAmount = 10f;

    [Space]
    [SerializeField] public NNAgent agentPrefab;

    protected Random rnd = new();

    public void Load(string[] brains = null)
    {
        for (int i = 0; i < 50; i++)
            agents.Add(Instantiate(agentPrefab, transform));

        foreach (NNAgent d in agents)
            d.color = new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());

        if (brains is null)
            return;

        for (int i = 0; i < brains.Length; i++)
            agents[i].Revive(new NeuralNetwork(brains[i % agents.Count].Split('*').Select(s => s.Split('_').Where(s => float.TryParse(s, out _)).Select(s1 => float.Parse(s1)).ToArray()).Take(3).ToArray(), agents[i].Brain.inputAmount, agents[i].Brain.outputAmount), agents[i].color);

        Restart();
    }

    protected virtual void Restart()
    {
        genText.text = "Поколение: " + ++generation;
        if (agents.Max(agents => agents.score) > maxScore)
            maxScore = agents.Max(agents => agents.score);

        convergence.Add(agents.Max(agents => agents.score));
        ConChart.UpdateChart(convergence);

        List<NNAgent> BestAgents = agents.OrderByDescending(agent => agent.score).ToList().Take(agents.Count / 2).ToList();
        List<Color> bestColors = BestAgents.Select(d => d.color).ToList();
        List<NeuralNetwork> bestBrains = BestAgents.Select(d => d.Brain).ToList();

        for (int i = 0; i < agents.Count(); i++)
        {
            if (i < bestBrains.Count)
            {
                agents[i].Revive(bestBrains[i].Mutate(0, 0), bestColors.ElementAt(i % bestColors.Count));
            }
            else
            {
                agents[i].Revive(bestBrains[i - bestBrains.Count].Mutate(mutationPower, mutationAmount), bestColors.ElementAt(i % bestColors.Count));
            }
        }
    }

    public virtual void FixedUpdate()
    {
        ScoreText.text = "Счет: " + agents.Max(d => d.score) + "; макс.: " + maxScore;
        
        if (agents.All(d => d.dead))
            Restart();
    }
}