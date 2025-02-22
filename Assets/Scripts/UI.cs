using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject floor;

    public void ShowAllChanged(bool value)
    {
        floor.SetActive(value);
    }
}