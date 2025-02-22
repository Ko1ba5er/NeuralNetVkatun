using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FoodDropper : MonoBehaviour
{
    [SerializeField] private Vector2 size;
    [SerializeField] private Transform food;
    [SerializeField] private int foodAmount;
    [SerializeField] private int foodDelay;

    private int RoomLayers = 10;//TODO: fix shit

    private List<Transform> foods = new List<Transform>();
    private Coroutine mainCor;

    private void Start()
    {
        mainCor = StartCoroutine(FoodDrop());
    }

    IEnumerator FoodDrop()
    {
        while (true)
        {
            for (int j = 0; j < RoomLayers; j++)
                for (int i = 0; i < foodAmount; i++)
                    DropFood(j);

            yield return new WaitForSeconds(foodDelay);
        }
    }

    public void Restart()
    {
        foreach (Transform f in foods)
            f.transform.position = Vector3.up * 100;

        StopCoroutine(mainCor);
        mainCor = StartCoroutine(FoodDrop());
    }

    public void DropFood(int YLayer)
    {
        Transform f = foods.FirstOrDefault(f => f.transform.position.y >= 100);
        if (f != null)
            f.position = new Vector3(transform.position.x + Random.Range(-size.x, size.x), -YLayer * 3, transform.position.z + Random.Range(-size.y, size.y));
        else
            foods.Add(Instantiate(food, new Vector3(transform.position.x + Random.Range(-size.x, size.x), -YLayer * 3, transform.position.z + Random.Range(-size.y, size.y)), Quaternion.identity, transform));
    }
}