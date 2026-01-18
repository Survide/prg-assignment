namespace prg_asg;

public class Helper {
    public static string GetValidInput(string prompt, string errorMsg, Func<string, bool>? validator = null) {
        // if validator is null, then will only check for empty strings
        while (true) {
            Console.Write(prompt);
            string input = Console.ReadLine() ?? "";
            if (!string.IsNullOrEmpty(input) && (validator == null || validator(input))) {
                return input;
            }
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"Error: {errorMsg}");
            Console.ResetColor();
        }
    }
} 
