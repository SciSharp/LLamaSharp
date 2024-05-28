namespace LLama.Examples.Examples
{
    public class QuantizeModel
    {
        public static void Run()
        {
            string inputPath = UserSettings.GetModelPath();

            Console.Write("Please input your output model path: ");
            var outputPath = Console.ReadLine();

            Console.Write("Please input the quantize type (one of q4_0, q4_1, q5_0, q5_1, q8_0): ");
            var quantizeType = Console.ReadLine();

            if (LLamaQuantizer.Quantize(inputPath, outputPath, quantizeType))
            {
                Console.WriteLine("Quantization succeeded!");
            }
            else
            {
                Console.WriteLine("Quantization failed!");
            }
        }
    }
}
