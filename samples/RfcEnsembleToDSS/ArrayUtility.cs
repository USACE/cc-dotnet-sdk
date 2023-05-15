namespace RfcEnsembleToDSS
{
  class ArrayUtility
  {
    /// <summary>
    /// transpose float[,] to double[,]
    /// </summary>
    /// <param name=""></param>
    internal static void TransposeFloatToDouble(float[,] src, ref double[,] data)
    {
      int width = src.GetLength(1);
      int height = src.GetLength(0);
      if (data == null || data.GetLength(1) != width || data.GetLength(0) != height)
        data = new double[width, height];
      for (int r = 0; r < height; r++)
      {
        for (int c = 0; c < width; c++)
        {
          data[c, r] = src[r, c];
        }
      }
    }

    internal static void TransposeDoubleToFloat(double[,] src, ref float[,] data)
    {
      int width = src.GetLength(1);
      int height = src.GetLength(0);
      if (data == null || data.GetLength(1) != width || data.GetLength(0) != height)
        data = new float[width, height];
      for (int r = 0; r < height; r++)
      {
        for (int c = 0; c < width; c++)
        {
          data[c, r] = (float) src[r, c];
        }
      }
    }
  }
}
