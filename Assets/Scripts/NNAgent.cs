using UnityEngine;

public class NNAgent : MonoBehaviour
{
    public virtual NeuralNetwork Brain { get; set; }
    public virtual bool dead { get; set; } = false;
    public virtual float score { get; set; } = 0;
    public virtual Color color { get; set; }
    public virtual void Revive(NeuralNetwork brain, Color color) { }
}