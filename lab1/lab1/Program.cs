using System;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await InitializeStartFile();
            InitializeCrcTable();

            Console.WriteLine("Wybierz numer zadania.");
            Enumerable.Range(0, 8).ToList().ForEach(x => Console.WriteLine($"{x + 1} - Zadanie {x + 1}"));

            var opt = Convert.ToInt32(Console.ReadLine());
            switch (opt)
            {
                case 1:
                    await ReadFromFile_1($"{Environment.CurrentDirectory}\\file.txt");
                    break;
                case 2:
                    Console.WriteLine("Co wpisać do pliku file.txt?");
                    await WriteToFile_2($"{Environment.CurrentDirectory}\\file.txt");
                    break;
                case 3:
                    int[] array = { 4, 5, 7, 11, 12, 15, 15, 21, 40, 45 };
                    Console.WriteLine("Wynik: " + SearchIndex_3(array, 11));
                    break;
                case 4:
                    Console.WriteLine("Crc32:" + CalculateCrc32_4("This is example text ..."));
                    break;
                case 5:
                    LocalAndGlobalDateTime_5();
                    break;
                case 6:
                    await FileToLines_6($"{Environment.CurrentDirectory}\\file.txt");
                    break;
                case 7:
                    ToJson_7();
                    break;
                case 8:
                    ToModel_8();
                    break;

                default:
                    Console.WriteLine("Brak zadania z tym numerem.");
                    break;
            }

            Console.ReadLine();
        }

        private static async Task InitializeStartFile()
        {
            await File.AppendAllTextAsync($"{Environment.CurrentDirectory}\\file.txt", string.Empty,
                CancellationToken.None);
        }

        private static readonly uint[] CrcTable = new uint[256];
        private static void InitializeCrcTable()
        {
            for (uint i = 0; i < 256; ++i)
            {
                var code = i;
                for (int j = 0; j < 8; ++j)
                {
                    code = (code & 1) != 0 ? 0xEDB88320 ^ (code >> 1) : (code >> 1);
                }

                CrcTable[i] = code;
            }
        }

        private static async Task ReadFromFile_1(string path)
        {
            var result = await File.ReadAllTextAsync(path);
            Console.WriteLine(result);
        }

        private static async Task WriteToFile_2(string path)
        {
            Console.WriteLine("Napisz tekst, który ma zostać zapisany do pliku:");
            var read = Console.ReadLine();
            await File.WriteAllTextAsync(path, read);
        }

        private static int SearchIndex_3(int[] array, int value)
        {
            int index = 0;
            int limit = array.Length - 1;
            while (index <= limit)
            {
                int point = (int)Math.Ceiling((double)(index + limit) / 2);
                var entry = array[point];
                if (value > entry)
                {
                    index = point + 1;
                    continue;
                }
                if (value < entry)
                {
                    limit = point - 1;
                    continue;
                }
                return point; // value == entry
            }
            return -1;
        }

        private static long CalculateCrc32_4(string text)
        {
            long crc = -1;
            for (var index = 0; index < text.Length; ++index)
            {
                var z = text[index];
                var b = (byte)z;
                crc = CrcTable[(b ^ crc) & 0xFF] ^ (crc >> 8);
            }

            return (-1 ^ crc) >> 0;
        }

        private static void LocalAndGlobalDateTime_5()
        {
            Console.WriteLine("Global Time: " + DateTime.UtcNow);
            Console.WriteLine("Local Time: " + DateTime.Now);
        }

        private static async Task FileToLines_6(string path)
        {
            var result = await File.ReadAllLinesAsync(path);
            for (var i = 0; i < result.Length; i++)
            {
                Console.WriteLine($"{i++}. {result[i]}");
            }
        }

        private static void ToJson_7()
        {
            var user = new User("John", 21);
            var serialized = JsonSerializer.Serialize(user);
            Console.WriteLine(serialized);
        }

        private static void ToModel_8()
        {
            var json = "{\"Name\":\"John\",\"Age\":21}";
            var model = JsonSerializer.Deserialize<User>(json);

            Console.WriteLine($"Imie: {model.Name}");
            Console.WriteLine($"Wiek: {model.Age}");
        }

        private record User(string Name, int Age);
    }
}
