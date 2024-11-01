using System;
using System.Numerics.Tensors;

namespace LLama.Extensions;

/// <summary>
/// Extensions to span which apply <b>in-place</b> normalization
/// </summary>
public static class SpanNormalizationExtensions
{
    /// <summary>
    /// <b>In-place</b> multiple every element by 32760 and divide every element in the span by the max absolute value in the span
    /// </summary>
    /// <param name="vector"></param>
    /// <returns>The same array</returns>
    public static float[] MaxAbsoluteNormalization(this float[] vector)
    {
        vector.AsSpan().MaxAbsoluteNormalization();
        return vector;
    }

    /// <summary>
    /// <b>In-place</b> multiple every element by 32760 and divide every element in the span by the max absolute value in the span
    /// </summary>
    /// <param name="vector"></param>
    /// <returns>The same span</returns>
    public static Span<float> MaxAbsoluteNormalization(this Span<float> vector)
    {
        var factor = 32760 / TensorPrimitives.MaxMagnitude(vector);
        TensorPrimitives.Multiply(vector, factor, vector);
        return vector;
    }

    /// <summary>
    /// <b>In-place</b> divide every element in the array by the sum of absolute values in the array
    /// </summary>
    /// <remarks>Also known as "Manhattan normalization".</remarks>
    /// <param name="vector"></param>
    /// <returns>The same array</returns>
    public static float[] TaxicabNormalization(this float[] vector)
    {
        vector.AsSpan().TaxicabNormalization();
        return vector;
    }

    /// <summary>
    /// <b>In-place</b> divide every element in the span by the sum of absolute values in the span
    /// </summary>
    /// <remarks>Also known as "Manhattan normalization".</remarks>
    /// <param name="vector"></param>
    /// <returns>The same span</returns>
    public static Span<float> TaxicabNormalization(this Span<float> vector)
    {
        var sumAbs = TensorPrimitives.SumOfMagnitudes(vector);
        TensorPrimitives.Divide(vector, sumAbs, vector);
        return vector;
    }

    /// <summary>
    /// <b>In-place</b> divide every element by the euclidean length of the vector
    /// </summary>
    /// <remarks>Also known as "L2 normalization".</remarks>
    /// <param name="vector"></param>
    /// <returns>The same array</returns>
    public static float[] EuclideanNormalization(this float[] vector)
    {
        vector.AsSpan().EuclideanNormalization();
        return vector;
    }

    /// <summary>
    /// <b>In-place</b> divide every element by the euclidean length of the vector
    /// </summary>
    /// <remarks>Also known as "L2 normalization".</remarks>
    /// <param name="vector"></param>
    /// <returns>The same span</returns>
    public static Span<float> EuclideanNormalization(this Span<float> vector)
    {
        var norm = TensorPrimitives.Norm(vector);
        TensorPrimitives.Divide(vector, norm, vector);
        return vector;
    }

    /// <summary>
    /// Creates a new array containing an L2 normalization of the input vector.
    /// </summary>
    /// <param name="vector"></param>
    /// <returns>The same span</returns>
    public static float[] EuclideanNormalization(this ReadOnlySpan<float> vector)
    {
        var result = new float[vector.Length];
        TensorPrimitives.Divide(vector, TensorPrimitives.Norm(vector), result);
        return result;
    }

    /// <summary>
    /// <b>In-place</b> apply p-normalization. https://en.wikipedia.org/wiki/Norm_(mathematics)#p-norm
    /// <list type="bullet">
    /// <item>For p = 1, this is taxicab normalization</item>
    /// <item>For p = 2, this is euclidean normalization</item>
    /// <item>As p => infinity, this approaches infinity norm or maximum norm</item>
    /// </list>
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="p"></param>
    /// <returns>The same array</returns>
    public static float[] PNormalization(this float[] vector, int p)
    {
        vector.AsSpan().PNormalization(p);
        return vector;
    }

    /// <summary>
    /// <b>In-place</b> apply p-normalization. https://en.wikipedia.org/wiki/Norm_(mathematics)#p-norm
    /// <list type="bullet">
    /// <item>For p = 1, this is taxicab normalization</item>
    /// <item>For p = 2, this is euclidean normalization</item>
    /// <item>As p => infinity, this approaches infinity norm or maximum norm</item>
    /// </list>
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="p"></param>
    /// <returns>The same span</returns>
    public static Span<float> PNormalization(this Span<float> vector, int p)
    {
        if (p == 2)
            return vector.EuclideanNormalization();

        var sum = 0.0;
        for (var i = 0; i < vector.Length; i++)
            sum += MathF.Pow(vector[i], p);
        var divisor = (float)Math.Pow(sum, 1.0 / p);

        TensorPrimitives.Divide(vector, divisor, vector);

        return vector;
    }
}