namespace LLama.Examples.Extensions
{
    public static class IAsyncEnumerableExtensions
    {
        /// <summary>
        /// Show a console spinner while waiting for the next result
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<string> Spinner(this IAsyncEnumerable<string> source)
        {
            var enumerator = source.GetAsyncEnumerator();

            var characters = new[] { '|', '/', '-', '\\' };

            while (true)
            {
                var next = enumerator.MoveNextAsync();

                var (Left, Top) = Console.GetCursorPosition();

                // Keep showing the next spinner character while waiting for "MoveNextAsync" to finish
                var count = 0;
                while (!next.IsCompleted)
                {
                    count = (count + 1) % characters.Length;
                    Console.SetCursorPosition(Left, Top);
                    Console.Write(characters[count]);
                    await Task.Delay(75);
                }

                // Clear the spinner character
                Console.SetCursorPosition(Left, Top);
                Console.Write(" ");
                Console.SetCursorPosition(Left, Top);

                if (!next.Result)
                    break;
                yield return enumerator.Current;
            }
        }
    }
}
