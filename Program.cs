using System;
using System.Net.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using CLNobel;

namespace Exercicio.NET
{
    class Program
    {

        static async Task Main(string[] args)
        {
            String _PathLastRun = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\lastrun.txt";
            DateTime date1 = DateTime.MinValue;
           
            if (File.Exists(_PathLastRun))
            {
                string[] _LastRun = File.ReadAllLines(_PathLastRun);
                date1 = Convert.ToDateTime(_LastRun[0]);
            }

            File.WriteAllText(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\lastrun.txt", DateTime.Now.ToString());

            Nobel _Nobel = new();
            String _Path = "";
                   
           bool _Loop;        
                           
            do
            {
                Console.Write("Enter a Path: ");
                _Path = Console.ReadLine();

               _Loop = false;

                if (!File.Exists(_Path))
                {
                    Console.WriteLine("FILE NOT FOUND");                   
                    _Loop = true;
                }

                if (!_Path.EndsWith(".txt"))
                {
                    Console.WriteLine("PLEASE ENTER A .TXT FILE");
                    _Loop = true;
                }
            }

            while (_Loop == true);

            Console.WriteLine("Running...");

            string[] _Content = File.ReadAllLines(_Path);

            List<Nobel> _NobelList = new List<Nobel>();

            StringBuilder log = new StringBuilder("");

            foreach (String _Line in _Content)
            {
                _Nobel = new Nobel();

                _Nobel.Category = MidUntil(_Line, 0, ';');
                _Nobel.Year = _Line.Substring(_Line.IndexOf(';') + 1);
                _NobelList.Add(_Nobel);                                
            }

            TimeSpan ts = DateTime.Now - date1;

            if (ts < new TimeSpan(0, 0, 1, 0, 0))
            {
                TimeSpan time = (ts - new TimeSpan(0, 0, 1, 0, 0));                
                Thread.Sleep(Convert.ToInt32(time.TotalMilliseconds * -1));
            }
                    
            foreach (Nobel _Item in _NobelList)
            {                
                await API(_Item,log);
                await Task.Delay(15000); 
            }

            File.WriteAllText(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\log.txt", log.ToString());

            Thread.Sleep(5000);

            Console.WriteLine("SUCESS");
            Environment.Exit(0);

        }

#region "Functions"
        public static string MidUntil(string _Text, int _InitialIndex, char _LimitChar)
        {
            string _Result = string.Empty;

            for (int i = _InitialIndex ; i < _Text.Length -1; i++)
            {
                if (_LimitChar == _Text[i])
                {
                    break;
                }
                _Result += _Text[i];
            }

            return _Result;
        }

        static async Task API(Nobel _Nobel, StringBuilder log)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(string.Format("https://api.nobelprize.org/v1/prize.json?category={0}&year={1}", _Nobel.Category, _Nobel.Year));

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                log.AppendLine(result);
            }
        }

#endregion
    }
}
