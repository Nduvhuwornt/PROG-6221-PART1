using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace CybersecurityChatbot
{
    // This class handles all the nice console formatting and visuals
    static class ConsoleUI
    {
        // Voice Greeting
        public static void PlayVoiceGreeting()
        {
            try
            {
                // Resolve file from app output so both `dotnet run` and published exe find it.
                string path = Path.Combine(AppContext.BaseDirectory, "greeting.wav");

                if (!File.Exists(path))
                {
                    Console.WriteLine($"⚠️ Could not find '{path}'. Put 'greeting.wav' in the project output folder (or project root so the build copies it).\n");
                    return;
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    const uint SND_ASYNC = 0x0001;
                    const uint SND_FILENAME = 0x00020000;

                    bool played = PlaySound(path, IntPtr.Zero, SND_FILENAME | SND_ASYNC);
                    if (played)
                    {
                        Console.WriteLine("🎤 Voice greeting played!\n");
                    }
                    else
                    {
                        int err = Marshal.GetLastWin32Error();
                        Console.WriteLine($"⚠️ PlaySound failed (Win32 error {err}). Ensure 'greeting.wav' is a valid PCM WAV and audio device is available.\n");
                    }
                }
                else
                {
                    Console.WriteLine("⚠️ Sound playback not supported on this OS. Place 'greeting.wav' in the project output folder.\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Could not play voice: {ex.Message}\n");
            }
        }

        // P/Invoke PlaySound from winmm.dll (no extra package required)
        [DllImport("winmm.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool PlaySound(string pszSound, IntPtr hmod, uint fdwSound);

        // Beautiful ASCII art logo
        public static void DisplayAsciiArt()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
   _____ _           _                  _____                           _   
  / ____| |         | |                / ____|                         | |  
 | |    | | ___  ___| |__   ___  ___  | |  __  __ _ _ __ ___   ___  ___| |_ 
 | |    | |/ _ \/ __| '_ \ / _ \/ __| | | |_ |/ _` | '_ ` _ \ / _ \/ __| __|
 | |____| |  __/ (__| | | |  __/\__ \ | |__| | (_| | | | | | |  __/\__ \ |_ 
  \_____|_|\___|\___|_| |_|\___||___/  \_____|\__,_|_| |_| |_|\___||___/\__|
            ");
            Console.ResetColor();
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════\n");
        }

        public static string GetUserName()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("👋 Hello! What is your name? ");
            Console.ResetColor();

            string name = Console.ReadLine()?.Trim();

            while (string.IsNullOrEmpty(name))
            {
                ConsoleUI.ShowError("❌ Name cannot be empty. Please enter your name:");
                name = Console.ReadLine()?.Trim();
            }
            return name;
        }

        public static void ShowWelcomeMessage(string name)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║   Welcome to the Cybersecurity Awareness Bot, {name}!   ║");
            Console.WriteLine($"╚══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            TypeText("I'm here to help you stay safe online! 🛡️", 40);
            Console.WriteLine();
        }

        public static void ShowInstructions()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Type 'exit' anytime to quit.\n");
            Console.ResetColor();
        }

        public static void ShowPrompt(string userName)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"{userName}: ");
            Console.ResetColor();
        }

        public static void ShowBotResponse(string response)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("🤖 Bot: ");
            Console.ResetColor();
            TypeText(response);
            Console.WriteLine("───────────────────────────────────────────────────────────────\n");
        }

        public static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message + "\n");
            Console.ResetColor();
        }

        public static void ShowGoodbye()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("👋 Goodbye! Stay safe online!");
            Console.ResetColor();
        }

        // Typing effect (makes it look professional)
        private static void TypeText(string text, int delay = 30)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
            Console.WriteLine();
        }
    }
}