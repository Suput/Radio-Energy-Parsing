using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParseMusicEnergy_v0
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                string url = "";
                do
                    url = await Start2();
                while (url == "");

                using (var mf = new MediaFoundationReader(url))
                using (var wo = new WaveOutEvent())
                {
                    wo.Init(mf);
                    wo.Volume = 0.35f;
                    wo.Play();
                    while (wo.PlaybackState == PlaybackState.Playing)
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n\n\n" + ex.Message + "\n\n\n");
                Console.WriteLine("Now, I'm trying more.");
                await Task.Delay(2000);
            }

            await Main(args);
        }

        static async Task<string> Start2()
        {
            try
            {
                string response = await new HttpClient().GetStringAsync("https://www.energyfm.ru/");
                Console.WriteLine("(1/5) -- Energy html getted");
                string url = Regex.Match(response, "data-playlist=\"([^\"]+)\"").Groups[1].Value;
                Console.WriteLine("(2/5) -- Audio tag found");
                response = await new HttpClient().GetStringAsync(url);
                Console.WriteLine("(3/5) -- Playlists getted");
                RootObject root = JsonConvert.DeserializeObject<RootObject>(response);
                Console.WriteLine("(4/5) -- Playlists converted");
                url = (root.playlist.Where(p => p.file[9] == '7').SingleOrDefault()).file;
                Console.WriteLine("(5/5) -- ic7 url found (exaclty the music)");

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

    public class Playlist
    {
        public string comment { get; set; }
        public string file { get; set; }
    }

    public class RootObject
    {
        public List<Playlist> playlist { get; set; }
    }
}

//string response = await new HttpClient().GetStringAsync("https://www.energyfm.ru/");
//var energy = new HtmlDocument();
//energy.LoadHtml(response.Trim());
//var str2 = energy.DocumentNode.Descendants("audio");
//var x = energy.DocumentNode.SelectNodes("//audio[@data-playlist]").Select(n => n.Attributes["data-playlist"].Value).SingleOrDefault();

//"http://ic7.101.ru:8000/a99?userid=0&setst=satb6cgskuu3emni9713hcfsvm" --- 9 symbol == 7