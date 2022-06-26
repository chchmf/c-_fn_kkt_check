using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Programm;
class Program
{
    public class JsAns
{
    public string status { get; set; }
    public string check_status { get; set; }
    public string check_result { get; set; }
}
    static async Task Main(string[] args)
    {
        Console.WriteLine("1 - проверяем ФН\n2 - проверяем ККТ");
        string choice = Console.ReadLine();
        Console.WriteLine("Введи код модели: ");
        string model = Console.ReadLine();
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        using var client = new HttpClient();
        List<string> vec = Read_file();

        var tasks = new List<Task<string>>();

        foreach (var sn in vec){
            tasks.Add(get_json(sn, model, choice, client));
        }
        Task.WaitAll(tasks.ToArray());
        stopWatch.Stop();
        TimeSpan ts = stopWatch.Elapsed;
        Console.WriteLine($"Закончил за: {stopWatch.Elapsed}. Закрой меня...");
    }
    static async Task<string> get_json(string sn, string model, string choice, HttpClient client){
        string url;
        if (choice == "1"){
            url = $"https://kkt-online.nalog.ru/lkip.html?query=/fn/model/check&factory_number={sn}&model_code={model}";
        }
        else if(choice == "2"){
            url = $"https://kkt-online.nalog.ru/lkip.html?query=/kkt/model/check&factory_number={sn}&model_code={model}";
        }
        else{
            return null;
        }
        var result = await client.GetAsync(url);
        var content = await result.Content.ReadAsStringAsync();
        var js = JsonConvert.DeserializeObject<JsAns>(content);
        Console.WriteLine($"{sn}\t{js.check_result}");
        return content;
    }
    static List<string> Read_file(){
        List<string> vec = new List<string>();
        string path = "text.txt";
        if (!File.Exists(path))
        {
            // Create the file.
            using (FileStream fs = File.Create(path))
            {
                Byte[] info =
                    new UTF8Encoding(true).GetBytes("Вставь сюда серийные номера! Каждый серийник с новой строки!!!");
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }
        }

        using (StreamReader sr = File.OpenText(path))
        {
            string s = "";
            while ((s = sr.ReadLine()) != null)
            {
                vec.Add(s);
            }
        }
        return vec;
    }
}