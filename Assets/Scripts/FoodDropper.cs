using System.Collections;
using UnityEngine;

public class FoodDropper : MonoBehaviour
{
    [SerializeField] private Vector2 size;
    [SerializeField] private GameObject food;
    [SerializeField] private int foodAmount;
    [SerializeField] private int foodDelay;

    private void Start()
    {
        StartCoroutine(FoodDrop());
    }

    IEnumerator FoodDrop()
    {
        for (int i = 0; i < foodAmount; i++)
            Instantiate(food, new Vector3(transform.position.x + Random.Range(-size.x, size.x), 0, transform.position.z + Random.Range(-size.y, size.y)), Quaternion.identity,  transform);

        yield return new WaitForSeconds(foodDelay);
        StartCoroutine(FoodDrop());
    }
}