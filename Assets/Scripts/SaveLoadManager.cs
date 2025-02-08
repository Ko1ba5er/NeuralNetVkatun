using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] private string path = "C:/NN/save.nns";
    [SerializeField] CatDropper[] catDroppers;

    //TODO: Load weights and send it to CatDroppers
    private void Start()
    {
        string[] droppers = File.ReadAllText(path).Split('\r');
        string[][] cats = droppers.Select(dropper => dropper.Split('\n')).ToArray();
        Debug.Log(droppers.Aggregate((s1, s2) => s1 + "\n__________________________" + s2));
        //string[][][] layers = cats.Select(cat => cat.Split('*')).ToArray();
        for (int i = 0; i < catDroppers.Length; i++)
        {
            //catDroppers[i].Run();

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
}