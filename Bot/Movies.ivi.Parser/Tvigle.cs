using System.Collections;
using System.Text;
using Bot.Models.Data;
using Bot.Models.Models;
using HtmlAgilityPack;

namespace Movies.Parser
{
	public class Tvigle
	{
		public void Parser()
		{
            HtmlWeb web = new HtmlWeb();
            web.OverrideEncoding = Encoding.UTF8;
            for (int j = 10; j < 30; j++)
            {
                //HtmlDocument document = web.Load($"https://www.tvigle.ru/catalog/filmy/?page={j}&show=block&page={++j}&q=&show=block");
                HtmlDocument document = web.Load($"https://www.tvigle.ru/collection/paramount-movies/?page={j}&show=block&page={++j}&q=&show=block");
                ArrayList list = new ArrayList();
                ArrayList listPoster = new ArrayList();

                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[contains(@class, 'col')]//a[@href]"))
                {
                    list.Add(node.GetAttributeValue("href", null));
                }
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[contains(@class, 'poster')]//img[@src]"))
                {
                    listPoster.Add("https:" + node.GetAttributeValue("src", null));
                }

                ArrayList list1 = new ArrayList();
                foreach (var item in list)
                    list1.Add("https://www.tvigle.ru" + item);
                list = list1;

                using (ApplicationContext context = new ApplicationContext())
                {
                    List<Movie> movies = new List<Movie>();
                    for (int i = 0; i < 16; i++)
                    {
                        Movie movie = new Movie();
                        try
                        {
                            document = web.Load((string)list[i]);
                            Console.WriteLine(list[i]);

                            movie.Link = (string)list[i];
                            movie.LinkPoster = (string)listPoster[i];

                            string movH1 = string.Empty;
                            string movSpan = string.Empty;

                            foreach (HtmlNode rating in document.DocumentNode.SelectNodes("//div[contains(@class, 'video-rating good jsVideoRating')]//span"))
                            {
                                Console.WriteLine(rating.InnerText);
                                movie.Rating = rating.InnerText;
                            }

                            foreach (HtmlNode mH1 in document.DocumentNode.SelectNodes("//div[contains(@class, 'product__info-content')]//h1"))
                            {
                                Console.WriteLine(mH1.InnerText);
                                movH1 = mH1.InnerText;
                            }
                            foreach (HtmlNode mSpan in document.DocumentNode.SelectNodes("//div[contains(@class, 'product__info-content')]//span"))
                            {
                                movSpan = mSpan.InnerText;
                                break;
                            }
                            movie.Title = movH1.Replace(movSpan, "");
                            Console.WriteLine(movie.Title);

                            int count = 0;
                            foreach (HtmlNode link in document.DocumentNode.SelectNodes("//div[contains(@class, 'labels')]//span"))
                            {
                                Console.WriteLine(link.InnerText);
                                if (count == 1)
                                    movie.Genre = link.InnerText;
                                else if (count == 2)
                                    movie.Country = link.InnerText;
                                else if (count == 3)
                                    movie.Release = link.InnerText;
                                count++;
                            }

                            try
                            {
                                foreach (HtmlNode subtitle in document.DocumentNode.SelectNodes("//div[contains(@class, 'product__description')]//p"))
                                {
                                    Console.WriteLine(subtitle.InnerText.Replace("\n", ""));
                                    movie.Sutitle = subtitle.InnerText.Replace("\n", "");
                                }
                            }
                            catch
                            {
                                foreach (HtmlNode subtitle in document.DocumentNode.SelectNodes("//div[contains(@class, 'jsReadMore')]//div"))
                                {
                                    Console.WriteLine(subtitle.InnerText.Replace("\n", ""));
                                    movie.Sutitle = subtitle.InnerText.Replace("\n", "");
                                }
                            }
                            context.Movies.Add(movie);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine();
                        }
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}

