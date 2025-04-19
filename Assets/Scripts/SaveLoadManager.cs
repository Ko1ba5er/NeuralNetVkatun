using System.IO;
using System.Text;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] private string path = "C:/NN/save.nns";
    [SerializeField] private Room room;

    public void Start()
    {
        Load();
    }

    public void Load()
    {
        if (!File.Exists(path))
        {
            room.Load();
            return;
        }

        string[] brains = File.ReadAllText(path).Trim().Split('\n');
        room.Load(brains);
    }

    public void Save()
    {
        StringBuilder sb = new StringBuilder();
        foreach (NNAgent agent in room.Agents)
        {
            foreach (float[] w in agent.Brain.weights)
            {
                foreach (float _w in w)
                    sb.Append(_w.ToString() + "_");
                sb.Append("*");
            }
            sb.Append("\n");
        }

        File.WriteAllText(path, sb.ToString());
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}