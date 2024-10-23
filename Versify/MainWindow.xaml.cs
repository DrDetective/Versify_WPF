using Genius;
using HtmlAgilityPack;
using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Versify
{
    public partial class MainWindow : Window
    {
        private string accessToken = "UryxQVRI0k9N1PAigqGnCCxpiiWI6aust_v9kRn-sS9lyPW1pdfrhqM3JxKQC83v";

        public MainWindow()
        {
            InitializeComponent();
            Search.KeyDown += SongSearchBox_KeyDown;
        }
        private async void SongSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string query = Search.Text;
                if (string.IsNullOrEmpty(query))
                {
                    MessageBox.Show("Please enter a song or artist name.");
                    return;
                }
                try
                {
                    var lyrics = await GetLyricsAsync(query);

                    if (!string.IsNullOrEmpty(lyrics))
                    {
                        LyricsTextBox.Text = lyrics;
                    }
                    else
                    {
                        LyricsTextBox.Text = "Lyrics not found.";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching lyrics: " + ex.Message);
                }
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
        }
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

                    // Clean up each lyrics container
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
                        foreach (var annotation in unwantedNodes)
                        {
                            annotation.Remove();
                        }
                    }
                    // Append cleaned lyrics from this container
                    string cleanedLyrics = lyricsDoc.DocumentNode.InnerText.Trim();
                    cleanedLyrics = WebUtility.HtmlDecode(cleanedLyrics);

                    // Insert a single newline after each verse/section
                    cleanedLyrics = Regex.Replace(cleanedLyrics, @"(\n{2,})", "\n"); // Replace multiple newlines with a single newline
                    cleanedLyrics = Regex.Replace(cleanedLyrics, @"(\[.*?\])", "\n$1"); // Add a new line before square brackets

                    // Add to full lyrics with a single new line between different containers for clarity
                    fullLyrics += cleanedLyrics + "\n"; // Add a single new line between different containers
                }
                // Trim any extra newlines at the start or end
                return fullLyrics.Trim();
            }
            catch (Exception ex) { MessageBox.Show("Error scraping lyrics: " + ex.Message); }
            return null;
        }

    }
}
