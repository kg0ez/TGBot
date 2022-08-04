using System.Collections;
using System.Text;
using Bot.Models.Data;
using Bot.Models.Models;
using HtmlAgilityPack;

HtmlWeb web = new HtmlWeb();
web.OverrideEncoding = Encoding.UTF8;
HtmlDocument document = web.Load("https://www.ivi.ru/collections/best-movies/page2");
ArrayList listMovies = new ArrayList();
ArrayList listPoster = new ArrayList();

foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[contains(@class, 'pageSection__container')]//a[@href]"))
{
    listMovies.Add("https://www.ivi.ru" + node.GetAttributeValue("href", null));
}

foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[contains(@class, 'pageSection__container')]//img[@src]"))
{
    listPoster.Add(node.GetAttributeValue("src", null));
}

Console.WriteLine(listPoster.Count);
Console.WriteLine(listMovies.Count);

using (ApplicationContext context = new ApplicationContext())
{
    List<Movie> movies = new List<Movie>();
    for (int i = 0; i <= 29; i++)
    {
        Movie movie = new Movie();
        try
        {
            document = web.Load((string)listMovies[i]);

            movie.Link = (string)listMovies[i];
            movie.LinkPoster = (string)listPoster[i];

            foreach (HtmlNode link in document.DocumentNode.SelectNodes("//div[contains(@class, 'watchTitle contentCard__watchTitle')]//h1"))
            {
                movie.Title = link.InnerText;
            }

            List<string> common = new List<string>();
            foreach (HtmlNode link in document.DocumentNode.SelectNodes("//div[contains(@class, 'watchParams__item')]//a"))
            {
                common.Add(link.InnerText);
            }
            string genre = string.Empty;
            for (int j = 0; j < common.Count / 2; j++)
            {
                if (j == 0)
                    movie.Release = common[j];
                if (j == 3)
                    movie.Country = common[j];
                if (j > 3)
                    genre += common[j] + " ";
            }
            movie.Genre = genre.TrimEnd();

            foreach (HtmlNode link in document.DocumentNode.SelectNodes("//div[contains(@class, 'clause__text-inner')]//p"))
            {
                movie.Sutitle = link.InnerText;
                break;
            }
            context.Movies.Add(movie);
        }
        catch (Exception) { }
    }
    context.SaveChanges();
}

