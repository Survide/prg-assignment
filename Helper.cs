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
            PrintColour(ConsoleColor.Red, $"Error: {errorMsg}");
        }
    }

    public static void PrintColour(ConsoleColor colour, string? message = null, Action? printFunc = null) {
        Console.ForegroundColor = colour;
        if (message != null) Console.WriteLine(message);
        printFunc?.Invoke();
        Console.ResetColor();
    }
}

