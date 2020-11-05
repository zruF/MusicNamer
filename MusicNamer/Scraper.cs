using AngleSharp.Html.Parser;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MusicNamer
{
    public class Scraper
    {
        public Scraper()
        {

        }

        public async Task<IEnumerable<string>> GetAlbumsByArtist(string artist)
        {
            var website = $"https://www.last.fm/music/{artist}/+albums?order=most_popular";

            var content = await GetWebSiteContent(website);

            var parser = new HtmlParser();

            var document = parser.ParseDocument(content);

            var albumSection = document.All.First(x => x.Id != null && x.Id.Equals("artist-albums-section"));

            var albums = albumSection.GetElementsByClassName("link-block-target").Select(x => x.InnerHtml);

            return albums;
        }

        public async Task<Stream> GetWebSiteContent(string url)
        {
            var httpClient = new HttpClient();

            var request = await httpClient.GetAsync(url);

            var content = await request.Content.ReadAsStreamAsync();

            return content;
        }
    }
}