using System;
using System.Linq;
using Random = System.Random;

[Serializable]
public class NeuralNetwork
{
    [Serializable]
    public class WC
    {
        public float[] _weights;
    }
    public WC[] WCs;

    public static Random rnd = new();

    public readonly int inputAmount = 2;
    public readonly int[] hiddenAmount = { 3 };
    public readonly int outputAmount = 2;

    float[] results;

    private static Func<float, float> ActivationFunc;

    public float[][] weights;

    public NeuralNetwork(float[][] weights, int inputAmount, int outputAmount)
        : this(inputAmount, outputAmount)
    {
        this.weights = weights;

        WCs = new WC[weights.Length];
        for (int i = 0; i < weights.Length; i++)
            WCs[i] = new WC() { _weights = weights[i] };
    }

    public NeuralNetwork(int inputAmount, int outputAmount)
    {
        ActivationFunc = new LinearNeuron().proccess;
        this.inputAmount = inputAmount;
        this.outputAmount = outputAmount;

        weights = new float[1 + hiddenAmount.Length][];
        weights[0] = new float[(inputAmount + 1) * hiddenAmount[0]];
        for (int i = 1; i < hiddenAmount.Length; i++)
        {
            weights[i] = new float[(hiddenAmount[i - 1] + 1) * hiddenAmount[i]];
        }
        weights[weights.Length - 1] = new float[(hiddenAmount[hiddenAmount.Length - 1] + 1) * outputAmount];

        for (int i = 0; i < weights.Length; i++)
            for (int j = 0; j < weights[i].Length; j++)
                weights[i][j] = 0;

        WCs = new WC[weights.Length];
        for (int i = 0; i < weights.Length; i++)
            WCs[i] = new WC() { _weights = weights[i] };
    }

    public float[] proccess(params float[] inputs)
    {
        //StringBuilder sb = new StringBuilder();
        //sb.Append("inputs: " + inputs.Select(f => f.ToString()).Aggregate((s1, s2) => s1 + " " + s2));

        results = new float[inputAmount + 1];
        for (int i = 0; i < inputs.Length; i++)
            results[i] = ActivationFunc.Invoke(inputs[i]);
        
        results[results.Length - 1] = new BiasNeuron().proccess(0);
        
        //sb.Append("\nli res: " + results.Select(f => f.ToString()).Aggregate((s1, s2) => s1 + " " + s2));

        for (int j = 0; j < hiddenAmount.Length; j++)
        {
            inputs = new float[hiddenAmount[j]];
            for (int i = 0; i < weights[j].Length; i++)
                inputs[i % inputs.Length] += weights[j][i] * results[i % results.Length];

            //sb.Append("\nhl" + j + " ins: " + inputs.Select(f => f.ToString()).Aggregate((s1, s2) => s1 + " " + s2));

            results = new float[hiddenAmount[j] + 1];
            for (int i = 0; i < inputs.Length; i++)
                results[i] = ActivationFunc.Invoke(inputs[i]);

            results[results.Length - 1] = new BiasNeuron().proccess(0);

            //sb.Append("\nhl" + j + " res: " + results.Select(f => f.ToString()).Aggregate((s1, s2) => s1 + " " + s2));
        }

        inputs = new float[outputAmount];
        for (int i = 0; i < weights[hiddenAmount.Length].Length; i++)
            inputs[i % inputs.Length] += weights[hiddenAmount.Length][i] * results[i % results.Length];

        //sb.AppendLine("\nlo ins: " + inputs.Select(f => f.ToString()).Aggregate((s1, s2) => s1 + " " + s2));

        results = new float[outputAmount];
        for (int i = 0; i < inputs.Length; i++)
            results[i] = ActivationFunc.Invoke(inputs[i]);

        //sb.AppendLine("out: " + results.Select(f => f.ToString()).Aggregate((s1, s2) => s1 + " " + s2));
        //sb.AppendLine("weights:\n" + WCs.Select(wc => wc._weights.Select(f => f.ToString()).Aggregate((s1, s2) => s1 + " " + s2)).Aggregate((wc1, wc2) => wc1 + "\n" + wc2));
        //Debug.Log(sb.ToString());

        return results;
    }

    const float stdMutatePower = 0.3f;
    const float stdMutateAmount = 10f;
    public NeuralNetwork Mutate(float mutatePower = stdMutatePower, float mutateAmount = stdMutateAmount)
    {
        float[][] mutated_weights = weights.Select(fs => (float[])fs.Clone()).ToArray();
        for (int i = 0; i < mutateAmount; i++)
        {
            int x = (int)(rnd.NextDouble() * mutated_weights.Length);
            int y = (int)(rnd.NextDouble() * mutated_weights[x].Length);
            mutated_weights[x][y] += ((float)rnd.NextDouble() * 2 - 1) * mutatePower;
        }

        return new NeuralNetwork(mutated_weights, inputAmount, outputAmount);
    }
}