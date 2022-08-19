using System.Collections;
using System.Text;
using Bot.Models.Data;
using Bot.Models.Models;
using HtmlAgilityPack;

namespace ParserMovies
{
    public class IvI
    {
        public void Parser()
        {
            HtmlWeb web = new HtmlWeb();
            web.OverrideEncoding = Encoding.UTF8;

            for (int f = 1; f <= 10; f++)
            {
                HtmlDocument document = web.Load($"https://www.ivi.ru/collections/best-movies/page{f}");
                ArrayList list = new ArrayList();
                ArrayList listPoster = new ArrayList();

                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[contains(@class, 'pageSection__container')]//a[@href]"))
                {
                    Console.WriteLine(node.GetAttributeValue("href", null));

                    list.Add("https://www.ivi.ru" + node.GetAttributeValue("href", null));
                }
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[contains(@class, 'pageSection__container')]//img[@src]"))
                {
                    Console.WriteLine(node.GetAttributeValue("src", null));

                    listPoster.Add(node.GetAttributeValue("src", null));
                }
                Console.WriteLine(listPoster.Count);
                Console.WriteLine(list.Count);

                //https://www.ivi.ru/watch/210332
                using (ApplicationContext context = new ApplicationContext())
                {
                    List<Movie> movies = new List<Movie>();
                    for (int i = 0; i < 30; i++)
                    {
                        Movie movie = new Movie();
                        try
                        {

                            document = web.Load((string)list[i]);
                            Console.WriteLine(list[i]);

                            movie.Link = (string)list[i];
                            movie.LinkPoster = (string)listPoster[i];


                            foreach (HtmlNode link in document.DocumentNode.SelectNodes("//div[contains(@class, 'watchTitle contentCard__watchTitle')]//h1"))
                            {
                                Console.WriteLine(link.InnerText);
                                movie.Title = link.InnerText;
                            }

                            Console.WriteLine(new string('-', 20));
                            List<string> common = new List<string>();
                            foreach (HtmlNode link in document.DocumentNode.SelectNodes("//div[contains(@class, 'watchParams__item')]//a"))
                            {
                                Console.WriteLine(link.InnerText);
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

                            Console.WriteLine(new string('-', 20));
                            foreach (HtmlNode link in document.DocumentNode.SelectNodes("//div[contains(@class, 'clause__text-inner')]//p"))
                            {
                                Console.WriteLine(link.InnerText);
                                movie.Sutitle = link.InnerText;
                                break;
                            }
                            foreach (HtmlNode link in document.DocumentNode.SelectNodes("//div[contains(@class, 'ratingMobile__container')]//div"))
                            {
                                Console.WriteLine(link.InnerText);
                                movie.Rating = link.InnerText;
                                break;
                            }
                            context.Movies.Add(movie);
                        }
                        catch (Exception) { }
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}

