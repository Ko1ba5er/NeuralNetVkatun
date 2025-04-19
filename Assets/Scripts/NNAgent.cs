using System;
using UnityEngine;

public class NNAgent : MonoBehaviour
{
    public virtual NeuralNetwork Brain { get; set; }
    public virtual bool dead { get; set; } = false;
    public virtual float score { get; set; } = 0;
    public virtual Color color { get; set; }
    public virtual void Revive(NeuralNetwork brain, Color color)
    {
        Brain = brain;

        WCs = new WC[brain.weights.Length];
        for (int i = 0; i < brain.weights.Length; i++)
            WCs[i] = new WC() { _weights = brain.weights[i] };
    }

    [Serializable]
    public class WC
    {
        public float[] _weights;
    }
    public WC[] WCs;
}