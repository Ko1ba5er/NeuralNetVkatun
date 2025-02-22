using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] private string path = "C:/NN/save.nns";
    [SerializeField] CatDropper[] catDroppers;

    private void Start()
    {
        if (!File.Exists(path))
        {
            foreach (CatDropper cd in catDroppers)
                cd.Load();

            return;
        }

        string[] droppers = File.ReadAllText(path).Split('\r');
        string[][] cats = droppers.Select(dropper => dropper.Split('\n')).ToArray();
        for (int i = 0; i < catDroppers.Length; i++)
        {
            catDroppers[i].Load(cats[i]);
        }
    }

    public void Save()
    {
        StringBuilder sb = new StringBuilder();
        foreach (CatDropper dropper in catDroppers)
        {
            foreach (Cat cat in dropper.generation)
            {
                foreach (float[] w in cat.Brain.weights)
                {
                    foreach (float _w in w)
                        sb.Append(_w.ToString() + "_");
                    sb.Append("*");
                }
                sb.Append("\n");
            }
            sb.Append("\r");
        }

        File.WriteAllText(path, sb.ToString());
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}