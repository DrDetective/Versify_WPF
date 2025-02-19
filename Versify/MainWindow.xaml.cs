using Genius;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Security.Policy;
using System.Diagnostics;

namespace Versify
{
    public partial class MainWindow : Window
    {
        private string accessToken = "UryxQVRI0k9N1PAigqGnCCxpiiWI6aust_v9kRn-sS9lyPW1pdfrhqM3JxKQC83v";
        public static string Full_lyrics;
        public MainWindow()
        {
            InitializeComponent();
            Search.KeyDown += SongSearchBox_KeyDown;
            Artist_Search.KeyDown += SongSearchBox_KeyDown;
        }
        private async void SongSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string songName = Search.Text;
                string artistName = Artist_Search.Text;
                try
                {
                    var lyrics = await GetLyricsAsync(songName + artistName);

                    if (!string.IsNullOrEmpty(lyrics)) { LyricsTBox.Text = lyrics; Full_lyrics = lyrics; Debug.Write(lyrics); }
                    else { LyricsTBox.Text = "Lyrics not found"; }
                }
                catch (Exception ex) { MessageBox.Show("Error fetching lyrics: " + ex.Message); }
            }
        }
        private async Task<string> GetLyricsAsync(string query)
        {
            var geniusClient = new GeniusClient(accessToken);
            var searchResults = await geniusClient.SearchClient.Search(query);
            if (searchResults != null && searchResults.Response.Hits.Count > 0)
            {
                var song = searchResults.Response.Hits[0].Result;
                string lyrics = await GetCleanLyricsFromUrl(song.Url);
                return lyrics;
            }
            return null;
        } //getting lyrics
        private async Task<string> GetCleanLyricsFromUrl(string url)
        {
            try
            {
                var web = new HtmlWeb();
                var document = await web.LoadFromWebAsync(url);
                var lyricsDivs = document.DocumentNode.SelectNodes("//div[@data-lyrics-container='true']");
                if (lyricsDivs == null || lyricsDivs.Count == 0)
                    return null;
                string fullLyrics = string.Empty;
                foreach (var lyricsDiv in lyricsDivs)
                {
                    string lyricsHtml = lyricsDiv.InnerHtml;
                    var lyricsDoc = new HtmlDocument();
                    lyricsDoc.LoadHtml(lyricsHtml);
                    foreach (var brNode in lyricsDoc.DocumentNode.SelectNodes("//br") ?? new HtmlNodeCollection(null))
                    {
                        brNode.ParentNode.ReplaceChild(lyricsDoc.CreateTextNode("\n"), brNode);
                    }
                    foreach (var pNode in lyricsDoc.DocumentNode.SelectNodes("//p") ?? new HtmlNodeCollection(null))
                    {
                        pNode.ParentNode.ReplaceChild(lyricsDoc.CreateTextNode("\n" + pNode.InnerText + "\n"), pNode);
                    }
                    var unwantedNodes = lyricsDoc.DocumentNode.SelectNodes("//a[@class='referent'] | //script | //style");
                    if (unwantedNodes != null)
                    {
                        foreach (var annotation in unwantedNodes) { annotation.Remove(); }
                    }
                    string cleanedLyrics = lyricsDoc.DocumentNode.InnerText.Trim();
                    cleanedLyrics = WebUtility.HtmlDecode(cleanedLyrics);
                    cleanedLyrics = Regex.Replace(cleanedLyrics, @"(\n{2,})", "\n");
                    cleanedLyrics = Regex.Replace(cleanedLyrics, @"(\[.*?\])", "\n$1");
                    fullLyrics += cleanedLyrics + "\n";
                }
                return fullLyrics.Trim();
            }
            catch (Exception ex) { MessageBox.Show("Error scraping lyrics: " + ex.Message); }
            return null;
        } //cleaning lyrics

        private void FullLyrics_Click(object sender, RoutedEventArgs e)
        {
            FullLyrics flyrics = new FullLyrics();
            flyrics.Show();
        }
    }
}
