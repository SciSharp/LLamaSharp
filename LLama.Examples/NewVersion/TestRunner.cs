namespace LLama.Examples.NewVersion
{
    public class NewVersionTestRunner
    {
        public static async Task Run()
        {
            Console.WriteLine("================LLamaSharp Examples (New Version)==================\n");

            Console.WriteLine("Please input a number to choose an example to run:");
            Console.WriteLine("0: Run a chat session without stripping the role names.");
            Console.WriteLine("1: Run a chat session with the role names stripped.");
            Console.WriteLine("2: Interactive mode chat by using executor.");
            Console.WriteLine("3: Instruct mode chat by using executor.");
            Console.WriteLine("4: Stateless mode chat by using executor.");
            Console.WriteLine("5: Load and save chat session.");
            Console.WriteLine("6: Load and save state of model and executor.");
            Console.WriteLine("7: Get embeddings from LLama model.");
            Console.WriteLine("8: Quantize the model.");
            Console.WriteLine("9: Automatic conversation.");
            Console.WriteLine("10: Constrain response to json format using grammar.");
            Console.WriteLine("11: Semantic Kernel Prompt.");
            Console.WriteLine("12: Semantic Kernel Chat.");
            Console.WriteLine("13: Semantic Kernel Memory.");
            Console.WriteLine("14: Semantic Kernel Memory Skill.");

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
                else if (choice == 10)
                {
                    GrammarJsonResponse.Run();
                }
                else if (choice == 11)
                {
                    await SemanticKernelPrompt.Run();
                }
                else if (choice == 12)
                {
                    await SemanticKernelChat.Run();
                }
                else if (choice == 13)
                {
                    await SemanticKernelMemory.Run();
                }
                else if (choice == 14)
                {
                    await SemanticKernelMemorySkill.Run();
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
