namespace LLama.Examples.NewVersion
{
    public class NewVersionTestRunner
    {
        public static async Task Run()
        {
            Console.WriteLine("================LLamaSharp Examples (New Version)==================\n");

            Console.WriteLine("Please input a number to choose an example to run:");
            Console.WriteLine("0: Run a chat session without stripping the role names.");
            Console.WriteLine("1: Run a chat session with the role names strippped.");
            Console.WriteLine("2: Interactive mode chat by using executor.");
            Console.WriteLine("3: Instruct mode chat by using executor.");
            Console.WriteLine("4: Stateless mode chat by using executor.");
            Console.WriteLine("5: Load and save chat session.");
            Console.WriteLine("6: Load and save state of model and executor.");
            Console.WriteLine("7: Get embeddings from LLama model.");
            Console.WriteLine("8: Quantize the model.");
            Console.WriteLine("9: Automatic conversation.");

            while (true)
            {
                Console.Write("\nYour choice: ");
                int choice = int.Parse(Console.ReadLine());

                if (choice == 0)
                {
                    ChatSessionWithRoleName.Run();
                }
                else if (choice == 1)
                {
                    ChatSessionStripRoleName.Run();
                }
                else if(choice == 2)
                {
                    await InteractiveModeExecute.Run();
                }
                else if(choice == 3)
                {
                    InstructModeExecute.Run();
                }
                else if(choice == 4)
                {
                    StatelessModeExecute.Run();
                }
                else if(choice == 5)
                {
                    SaveAndLoadSession.Run();
                }
                else if(choice == 6)
                {
                    LoadAndSaveState.Run();
                }
                else if(choice == 7)
                {
                    GetEmbeddings.Run();
                }
                else if(choice == 8)
                {
                    QuantizeModel.Run();
                }
                else if (choice == 9)
                {
                    await TalkToYourself.Run();
                }
                else
                {
                    Console.WriteLine("Cannot parse your choice. Please select again.");
                    continue;
                }
                break;
            }
        }
    }

    
}
