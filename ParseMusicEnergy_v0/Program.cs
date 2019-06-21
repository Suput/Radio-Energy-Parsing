using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ParseMusicEnergy_v0
{
    class Program
    {
        static string url = "";
        static int count = 0;

        static async Task Main(string[] args)
        {
            while (true)
            {
                await Start();
                PlayMusic();

                // I don't know why they have many urls
                // Just increase and try another
                if (count == 2)
                    count = 0;
                else
                    count++;
            }
        }

        static async Task Start()
        {
            try
            {
                do
                    url = await GetUrl();
                while (url == "");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n\n\n" + ex.Message + "\n\n\n");
                Console.WriteLine("Now, I'm trying more.");
                await Task.Delay(2000);
            }
        }

        static void PlayMusic()
        {
            try
            {
                using (var mf = new MediaFoundationReader(url))
                using (var wo = new WaveOutEvent())
                {
                    wo.Init(mf);
                    wo.Volume = 0.35f;
                    wo.Play();
                    while (wo.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("\n\n\n" + ex.Message + "\n\n\n");
                Console.WriteLine("Now, I'm trying more.");
                return;
            }
            
        }

        static async Task<string> GetUrl()
        {
            try
            {
                var client = new HttpClient();

                string response = await client.GetStringAsync("https://www.energyfm.ru/");
                Console.WriteLine("(1/5) -- Energy html getted");
                string url = Regex.Match(response, "data-playlist=\"([^\"]+)\"").Groups[1].Value;
                Console.WriteLine("(2/5) -- Audio tag found");
                response = await client.GetStringAsync(url);
                Console.WriteLine("(3/5) -- Playlists getted");
                var music = JsonConvert.DeserializeObject<Music>(response);
                Console.WriteLine("(4/5) -- Playlists converted");

                url = music.playlist[count].file;
                Console.WriteLine("(5/5) -- Music url found");

                return url;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n\n\n" + ex.Message + "\n\n\n");
                Console.WriteLine("Now, I'm trying more.");
                await Task.Delay(2000);
                return "";
            }
        }
    }

    public class MusicPath
    {
        public string comment { get; set; }
        public string file { get; set; }
    }

    public class Music
    {
        public List<MusicPath> playlist { get; set; }
    }

}

//string response = await new HttpClient().GetStringAsync("https://www.energyfm.ru/");
//var energy = new HtmlDocument();
//energy.LoadHtml(response.Trim());
//var str2 = energy.DocumentNode.Descendants("audio");
//var x = energy.DocumentNode.SelectNodes("//audio[@data-playlist]").Select(n => n.Attributes["data-playlist"].Value).SingleOrDefault();

//"http://ic7.101.ru:8000/a99?userid=0&setst=satb6cgskuu3emni9713hcfsvm" --- 9 symbol == 7