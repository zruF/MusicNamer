using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MusicNamer
{
    public class MusicNamer
    {
        public MusicNamer()
        {

        }

        public async Task Process(string musicPath)
        {
            var scraper = new Scraper();

            var artistPaths = new DirectoryInfo(musicPath).EnumerateDirectories();

            foreach (var artistPath in artistPaths)
            {
                var artist = artistPath.Name;

                var albums = (await scraper.GetAlbumsByArtist(artist)).ToList();

                var albumPaths = new DirectoryInfo(artistPath.FullName)
                  .EnumerateDirectories("*", SearchOption.AllDirectories)
                  .Where(x => albums.Any(y => x.Name.ToLower().Contains(y.ToLower())) && x.GetFiles().Any());

                // If Album has the same name as the artist
                if (albums.Contains(artist))
                {
                    var selfNamedAlbum = albumPaths.First(x => x.Name.ToLower().Contains(artist.ToLower()) && !albums.Any(y => y.ToLower() != artist.ToLower() && x.Name.ToLower().Contains(y.ToLower())));

                    if (selfNamedAlbum.FullName.ToLower() != (artistPath.FullName + @"\" + artist).ToLower())
                    {
                        try
                        {
                            Directory.Move(selfNamedAlbum.FullName, artistPath.FullName + @"\" + artist);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        LogConverts(artist, selfNamedAlbum.Name, artist);
                    }

                    albums.Remove(artist);
                }

                foreach (var albumPath in albumPaths)
                {
                    if (albums.Any(x => albumPath.Name.ToLower().Contains(x.ToLower())))
                    {
                        var albumName = albums.First(x => albumPath.Name.ToLower().Contains(x.ToLower()));

                        if (albumPath.FullName.ToLower() != (artistPath.FullName + @"\" + albumName).ToLower())
                        {
                            try
                            {
                                Directory.Move(albumPath.FullName, artistPath.FullName + @"\" + albumName);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                            LogConverts(artist, albumPath.Name, albumName);
                        }
                    }
                }

                Console.WriteLine("Deleting empty folders for: " + artistPath.Name);

                DeleteEmptyFolders(artistPath.FullName);
            }

            Console.WriteLine("Fertig");
        }

        public void LogConverts(string artist, string from, string to)
        {
            Console.WriteLine($"Artist: {artist}. From ~ {from} ~ to - {to} -");
        }

        public void DeleteEmptyFolders(string artistPath)
        {
            foreach (var folder in Directory.GetDirectories(artistPath))
            {
                DeleteEmptyFolders(folder);

                if (Directory.GetFileSystemEntries(folder).Length == 0)
                {
                    Directory.Delete(folder, false);

                    Console.WriteLine("Deleted empty folder: " + folder);
                }
            }
        }
    }
}