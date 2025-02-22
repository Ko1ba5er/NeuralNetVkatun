using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

//TODO: disable meshes for invisible GOs

public class CatDropper : MonoBehaviour
{
    [SerializeField] private Vector2 size;
    [SerializeField] private Cat cat;
    [SerializeField] private int GenerationAmount;
    [SerializeField] private int MinGenerationAmount;
    [SerializeField] private int GenerationLayers = 10;
    [SerializeField] private float RunTime;
    [SerializeField] private float mutationPower;
    [SerializeField] private float mutationAmount;
    [SerializeField] private FoodDropper foodDropper;
    private bool isLoaded = false;
    
    public List<Cat> generation;
    private float genStartTime = 1000;

    public void Load(string[] brains = null)
    {
        generation = new List<Cat>();
        for (int i = 0; i < GenerationAmount * GenerationLayers; i++)
            generation.Add(Instantiate(cat, new Vector3(Random.Range(-size.x, size.x), 0, Random.Range(-size.y, size.y)), Quaternion.Euler(0, Random.Range(0, 360), 0), transform));

        if (brains != null)
            for (int i = 0; i < GenerationAmount; i++)
                generation[i].Brain = new NeuralNetwork(brains[i % generation.Count].Split('*').Select(s => s.Split('_').Where(s => float.TryParse(s, out _)).Select(s1 => float.Parse(s1)).ToArray()).Take(3).ToArray(), 6, 3);

        isLoaded = true;
        Restart();
    }

    private void Restart()
    {
        foodDropper.Restart();

        genStartTime = Time.time;
        IEnumerable<NeuralNetwork> BestBrains = generation.OrderByDescending(cat => cat.score).Take(MinGenerationAmount * GenerationLayers).Select(cat => cat.Brain);
        NeuralNetwork[] MutatedBrains = new NeuralNetwork[GenerationAmount * GenerationLayers];
        for (int i = 0; i < GenerationAmount * GenerationLayers; i++)
            MutatedBrains[i] = BestBrains.ElementAt(i % MinGenerationAmount).Mutate(mutationPower, mutationAmount);

        for (int j = 0; j < GenerationLayers; j++)
            for (int i = 0; i < GenerationAmount; i++)
                if (i + j * GenerationLayers < generation.Count)
                {
                    generation[i + j * GenerationLayers].dead = false;
                    generation[i + j * GenerationLayers].transform.SetPositionAndRotation(new Vector3(transform.position.x + Random.Range(-size.x, size.x), -j * 3, transform.position.z + Random.Range(-size.y, size.y)), Quaternion.Euler(0, Random.Range(0, 360), 0));
                }
                else
                    generation.Add(Instantiate(cat, new Vector3(transform.position.x + Random.Range(-size.x, size.x), -j * 3, transform.position.z + Random.Range(-size.y, size.y)), Quaternion.Euler(0, Random.Range(0, 360), 0), transform));

        for (int i = 0; i < GenerationAmount * GenerationLayers; i++)
            generation[i].Brain = MutatedBrains[i];
    }

    private void Update()
    {
        if (!isLoaded)
        {
            if (Time.time >= 10)
                Load();
            return;
        }

        if (genStartTime + RunTime <= Time.time || generation.Where(c => !c.IsDestroyed()).Count() <= MinGenerationAmount)
            Restart();
    }
}