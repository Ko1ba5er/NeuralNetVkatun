using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CatDropper : MonoBehaviour
{
    [SerializeField] private Vector2 size;
    [SerializeField] private Cat cat;
    [SerializeField] private int GenerationAmount;
    [SerializeField] private int MinGenerationAmount;
    [SerializeField] private float RunTime;
    
    public List<Cat> generation;
    private float genStartTime = 1000;

    private void Start()
    {
        Load();
    }

    //TODO: SaveLoadManager send message with loaded brain
    public void Load()
    {
        generation = new List<Cat>(GenerationAmount);
        for (int i = 0; i < GenerationAmount; i++)
            generation.Add(Instantiate(cat, new Vector3(Random.Range(-size.x, size.x), 0, Random.Range(-size.y, size.y)), Quaternion.Euler(0, Random.Range(0, 360), 0), transform));

        Restart();
    }

    private void Restart()
    {
        //TODO: Удалить и сделать нормально
        GameObject[] food = GameObject.FindGameObjectsWithTag("Food");
        foreach (GameObject go in food)
            Destroy(go);

        genStartTime = Time.time;
        IEnumerable<NeuralNetwork> BestBrains = generation.OrderByDescending(cat => cat.ate).Take(MinGenerationAmount).Select(cat => cat.Brain);
        NeuralNetwork[] MutatedBrains = new NeuralNetwork[GenerationAmount];
        for (int i = 0; i < GenerationAmount; i++)
            MutatedBrains[i] = BestBrains.ElementAt(i % MinGenerationAmount).Mutate();

        foreach (var cat in generation.Where(cat => !cat.IsDestroyed()))
            Destroy(cat.gameObject);
        generation.Clear();

        for (int i = 0; i < GenerationAmount; i++)
            generation.Add(Instantiate(cat, new Vector3(transform.position.x + Random.Range(-size.x, size.x), 0, transform.position.z + Random.Range(-size.y, size.y)), Quaternion.Euler(0, Random.Range(0, 360), 0), transform));

        for (int i = 0; i < GenerationAmount; i++)
            generation[i].Brain = MutatedBrains[i];
    }

    private void Update()
    {
        if (genStartTime + RunTime <= Time.time || generation.Where(c => !c.IsDestroyed()).Count() <= MinGenerationAmount)
            Restart();
    }
}