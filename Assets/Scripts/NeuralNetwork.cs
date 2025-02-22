
using UnityEngine;

public class NeuralNetwork
{
    private INeuron[] InputLayer;
    private INeuron[][] HiddenLayer;
    private INeuron[] OutputLayer;

    public readonly int inputAmount = 2;
    public readonly int[] hiddenAmount = { 5, 5 };
    public readonly int outputAmount = 2;

    public float[][] weights;

    public NeuralNetwork(float[][] weights, int inputAmount, int outputAmount)
        : this(inputAmount, outputAmount)
    {
        this.weights = weights;
    }

    public NeuralNetwork(int inputAmount, int outputAmount)
    {
        this.inputAmount = inputAmount;
        this.outputAmount = outputAmount;
        InputLayer = new INeuron[inputAmount];
        for (int i = 0; i < InputLayer.Length; i++)
            InputLayer[i] = new LinearNeuron();

        HiddenLayer = new INeuron[hiddenAmount.Length][];
        for (int i = 0; i < hiddenAmount.Length; i++)
        {
            HiddenLayer[i] = new INeuron[hiddenAmount[i]];
            for (int j = 0; j < HiddenLayer[i].Length; j++)
                HiddenLayer[i][j] = new LinearNeuron();
        }

        OutputLayer = new INeuron[outputAmount];
        for (int i = 0; i < OutputLayer.Length; i++)
            OutputLayer[i] = new HeavisideStepNeuron();

        weights = new float[1 + hiddenAmount.Length][];
        weights[0] = new float[inputAmount * hiddenAmount[0]];
        for (int i = 1; i < hiddenAmount.Length; i++)
        {
            weights[i] = new float[hiddenAmount[i - 1] * hiddenAmount[i]];
        }
        weights[weights.Length - 1] = new float[hiddenAmount[hiddenAmount.Length - 1] * outputAmount];

        for (int i = 0; i < weights.Length; i++)
            for (int j = 0; j < weights[i].Length; j++)
                weights[i][j] = 1;
    }

    public float[] proccess(params float[] inputs)
    {
        float[] results = new float[inputAmount];
        for (int i = 0; i < inputs.Length; i++)
            results[i] = InputLayer[i].proccess(inputs[i]);

        inputs = new float[hiddenAmount[0]];
        for (int i = 0; i < weights[0].Length; i++)
            inputs[i % inputs.Length] += weights[0][i] * results[i % results.Length];

        results = new float[hiddenAmount[0]];
        for (int i = 0; i < inputs.Length; i++)
            results[i] = HiddenLayer[0][i].proccess(inputs[i]);



        inputs = new float[hiddenAmount[1]];
        for (int i = 0; i < weights[1].Length; i++)
            inputs[i % inputs.Length] += weights[1][i] * results[i % results.Length];

        results = new float[hiddenAmount[1]];
        for (int i = 0; i < inputs.Length; i++)
            results[i] = HiddenLayer[1][i].proccess(inputs[i]);




        inputs = new float[outputAmount];
        for (int i = 0; i < weights[2].Length; i++)
            inputs[i % inputs.Length] += weights[2][i] * results[i % results.Length];

        results = new float[outputAmount];
        for (int i = 0; i < inputs.Length; i++)
            results[i] = HiddenLayer[0][i].proccess(inputs[i]);

        return results;
    }

    const float stdMutatePower = 0.3f;
    const float stdMutateAmount = 10f;
    public NeuralNetwork Mutate(float mutatePower = stdMutatePower, float mutateAmount = stdMutateAmount)
    {
        float[][] mutated_weights = weights;
        for (int i = 0; i < mutateAmount; i++)
        {
            int x = Random.Range(0, mutated_weights.Length);
            int y = Random.Range(0, mutated_weights[x].Length);
            mutated_weights[x][y] += Random.Range(-mutatePower, mutatePower);// * ((Random.value > 0.5f) ? -1 : 1);
        }

        return new NeuralNetwork(mutated_weights, inputAmount, outputAmount);
    }
}