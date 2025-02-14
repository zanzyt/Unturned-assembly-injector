                                               // Unturned injector, for educational purposes only!
using System.Diagnostics;
using System.Reflection; 
using Newtonsoft.Json;
using SharpMonoInjector;

namespace EgguWareInjector
{
    class Program
    {
        // Dictionary to store language strings
        private static Dictionary<string, Dictionary<string, string>> languages = new Dictionary<string, Dictionary<string, string>>
        {
            {
                "English", new Dictionary<string, string>
                {
                    { "WelcomeMessage", "Made by zanzyt with <3!" },
                    { "LanguagePrompt", "Select your language:\n1. English\n2. Russian\nEnter your choice (1-2): " },
                    { "InvalidChoice", "Invalid choice. Defaulting to English." },
                    { "InjectorStarted", "Injector started." },
                    { "ProcessNotFound", "Target process not found: {0}. Attempting to launch Unturned..." },
                    { "ExeNotFound", "Unturned executable not found at: {0}" },
                    { "ProcessStartFailed", "Failed to start Unturned process." },
                    { "ProcessStarted", "Unturned process started (ID: {0}). Waiting for the process to initialize..." },
                    { "ProcessInitFailed", "Failed to find Unturned process after launching." },
                    { "ProcessFound", "Target process found: {0} (ID: {1})" },
                    { "DllNotFound", "DLL not found at: {0}" },
                    { "DllFound", "DLL found at: {0}" },
                    { "InjectionSuccess", "DLL injected successfully using SharpMonoInjector!" },
                    { "InjectorFinished", "Injector finished." },
                    { "PressAnyKey", "Press any key to exit..." },
                    { "Version", $"Version: {GetAssemblyVersion()}" }
                }
            },
            {
                "Russian", new Dictionary<string, string>
                {
                    { "WelcomeMessage", "Сделано занзутом с <3!" },
                    { "LanguagePrompt", "Выберите язык:\n1. Английский\n2. Русский\nВведите ваш выбор (1-2): " },
                    { "InvalidChoice", "Неверный выбор. По умолчанию используется английский." },
                    { "InjectorStarted", "Инжектор запущен." },
                    { "ProcessNotFound", "Целевой процесс не найден: {0}. Попытка запустить Unturned..." },
                    { "ExeNotFound", "Исполняемый файл Unturned не найден по пути: {0}" },
                    { "ProcessStartFailed", "Не удалось запустить процесс Unturned." },
                    { "ProcessStarted", "Процесс Unturned запущен (ID: {0}). Ожидание инициализации процесса..." },
                    { "ProcessInitFailed", "Не удалось найти процесс Unturned после запуска." },
                    { "ProcessFound", "Целевой процесс найден: {0} (ID: {1})" },
                    { "DllNotFound", "DLL не найдена по пути: {0}" },
                    { "DllFound", "DLL найдена по пути: {0}" },
                    { "InjectionSuccess", "DLL успешно внедрена с использованием SharpMonoInjector!" },
                    { "InjectorFinished", "Инжектор завершил работу." },
                    { "PressAnyKey", "Нажмите любую клавишу для выхода..." },
                    { "Version", $"Версия: {GetAssemblyVersion()}" } 
                }
            }
        };

        // Method to get the assembly version and configuration
        private static string GetAssemblyVersion()
        {
            // Get the currently executing assembly
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Get the assembly version
            Version version = assembly.GetName().Version;

            // Get the assembly configuration (e.g., Release, Debug)
            var configurationAttribute = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
            string configuration = configurationAttribute?.Configuration ?? "Unknown";

            // Format the version string (e.g., "1.0.0.0 (Release)")
            return $"{version} ({configuration})";
        }

        static void Main(string[] args)
        {
            // Display the welcome message in green
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(languages["English"]["WelcomeMessage"]);
            Console.ResetColor();

            // Display the version in yellow
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(languages["English"]["Version"]);
            Console.ResetColor();

            // Display the language selection menu in cyan
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(languages["English"]["LanguagePrompt"]);
            Console.ResetColor();

            // Read the user's choice
            string languageChoice = Console.ReadLine();

            // Set the selected language
            string selectedLanguage = languageChoice == "2" ? "Russian" : "English";
            if (languageChoice != "1" && languageChoice != "2")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(languages["English"]["InvalidChoice"]);
                Console.ResetColor();
                selectedLanguage = "English";
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nSelected language: {selectedLanguage}\n");
            Console.ResetColor();

            // Proceed with the injection process
            Logger.Log(languages[selectedLanguage]["InjectorStarted"], ConsoleColor.Blue);

            try
            {
                // Name of the target process (without extension)
                string targetProcessName = "Unturned";
                string unturnedExePath = @"C:\Program Files (x86)\Steam\steamapps\common\Unturned\Unturned.exe"; // Update this path to the actual location of Unturned.exe

                // Check if the Unturned process is already running
                Process targetProcess = GetTargetProcess(targetProcessName);
                if (targetProcess == null)
                {
                    Logger.Log(string.Format(languages[selectedLanguage]["ProcessNotFound"], targetProcessName), ConsoleColor.Yellow);

                    // Launch Unturned if its not running
                    if (!File.Exists(unturnedExePath))
                    {
                        Logger.Log(string.Format(languages[selectedLanguage]["ExeNotFound"], unturnedExePath), ConsoleColor.Red);
                        Pause(selectedLanguage);
                        return;
                    }

                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = unturnedExePath,
                        UseShellExecute = true
                    };

                    Process unturnedProcess = Process.Start(startInfo);
                    if (unturnedProcess == null)
                    {
                        Logger.Log(languages[selectedLanguage]["ProcessStartFailed"], ConsoleColor.Red);
                        Pause(selectedLanguage);
                        return;
                    }

                    Logger.Log(string.Format(languages[selectedLanguage]["ProcessStarted"], unturnedProcess.Id), ConsoleColor.Green);

                    // Wait for the process to initialize (optional delay)
                    System.Threading.Thread.Sleep(5000); // Adjust the delay as needed!

                    // Get the process again after launching
                    targetProcess = GetTargetProcess(targetProcessName);
                    if (targetProcess == null)
                    {
                        Logger.Log(languages[selectedLanguage]["ProcessInitFailed"], ConsoleColor.Red);
                        Pause(selectedLanguage);
                        return;
                    }
                }

                Logger.Log(string.Format(languages[selectedLanguage]["ProcessFound"], targetProcess.ProcessName, targetProcess.Id), ConsoleColor.Green);

                // Full path to the DLL you want to inject
                string dllPath = @"E:\Fixed-EgguWare-for-Unturned Main\EgguWare\bin\Debug\EgguWare.dll";
                if (!File.Exists(dllPath))
                {
                    Logger.Log(string.Format(languages[selectedLanguage]["DllNotFound"], dllPath), ConsoleColor.Red);
                    Pause(selectedLanguage);
                    return;
                }
                Logger.Log(string.Format(languages[selectedLanguage]["DllFound"], dllPath), ConsoleColor.Green);

                // Injection
                InjectUsingSharpMonoInjector(targetProcess.Id, dllPath, selectedLanguage);
            }
            catch (Exception ex)
            {
                Logger.Log($"Exception: {ex}", ConsoleColor.Red);
            }

            Logger.Log(languages[selectedLanguage]["InjectorFinished"], ConsoleColor.Blue);
            Pause(selectedLanguage);
        }
        private static Process GetTargetProcess(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0 ? processes[0] : null;
        }

        private static void InjectUsingSharpMonoInjector(int processId, string dllPath, string language)
        {
            try
            {
                // Create an instance of SharpMonoInjector
                Injector injector = new Injector(processId);

                // Define the namespace, class, and method to inject
                string @namespace = "EgguWare";
                string className = "Load";
                string methodName = "Start";

                byte[] dllBytes = File.ReadAllBytes(dllPath);

                // Inject the DLL
                injector.Inject(dllBytes, @namespace, className, methodName);

                Logger.Log(languages[language]["InjectionSuccess"], ConsoleColor.Green);
            }
            catch (InjectorException ex)
            {
                Logger.Log($"SharpMonoInjector error: {ex}", ConsoleColor.Red);
            }
            catch (Exception ex)
            {
                Logger.Log($"Exception: {ex}", ConsoleColor.Red);
            }
        }

        /// <summary>
        /// Pause the console so you can read the output.
        /// </summary>
        private static void Pause(string language)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(languages[language]["PressAnyKey"]);
            Console.ResetColor();
            Console.ReadKey();
        }
    }

    /// <summary>
    /// Logger class that writes log entries to a JSON file.
    /// </summary>
    public static class Logger
    {
        private static readonly string logFile = "InjectionLog.json";
        private static List<LogEntry> logEntries = new List<LogEntry>();

        public static void Log(string message, ConsoleColor color = ConsoleColor.White)
        {
            var entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Message = message
            };
            logEntries.Add(entry);

            // Write the entire log as JSON to the file
            try
            {
                File.WriteAllText(logFile, JsonConvert.SerializeObject(logEntries, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error writing log: " + ex);
                Console.ResetColor();
            }

            // Write the log message to the console 
            Console.ForegroundColor = color;
            Console.WriteLine($"{entry.Timestamp:HH:mm:ss} - {entry.Message}");
            Console.ResetColor();
        }
    }

    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
    }
}

//Thank to check the code =)