using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConvergenceChart : MonoBehaviour
{
    private List<Transform> dots = new();

    [SerializeField] Transform dotPrefab;

    public void UpdateChart(List<float> values)
    {
        while (dots.Count < values.Count)
        {
            dots.Add(Instantiate(dotPrefab, transform));
        }

        float stepX = (((RectTransform)transform).rect.size.x - 20) / values.Count;
        float stepY = (((RectTransform)transform).rect.size.y - 20) / Mathf.Max(values.Max(), 1);

        for (int i = 0; i < dots.Count; i++)
        {
            if (i >= values.Count)
            {
                dots[i].localPosition = Vector3.zero;
                continue;
            }

            dots[i].localPosition = new Vector3(stepX * i + 10, stepY * values[i] + 10, 0f);
        }
    }
}