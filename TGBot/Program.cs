using System;
using System.Collections;
using System.Text;
using HtmlAgilityPack;

namespace TGBot
{
    class Program
    {
        //https://www.ivi.ru/watch/210332
        static void Main(string[] args)
        {
            HtmlWeb web = new HtmlWeb();
            web.OverrideEncoding = Encoding.UTF8;
            HtmlDocument document = web.Load("https://www.ivi.ru/collections/best-movies");
            ArrayList list = new ArrayList();

            foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[contains(@class, 'pageSection__container')]//a[@href]"))
            {
                Console.WriteLine(node.GetAttributeValue("href", null));

                list.Add("https://www.ivi.ru"+node.GetAttributeValue("href", null));
            }
            Console.WriteLine(list.Count);

            foreach (string item in list)
            {
                document = web.Load(item);
                Console.WriteLine(item);
                foreach (HtmlNode link in document.DocumentNode.SelectNodes("//div[contains(@class, 'watchTitle contentCard__watchTitle')]//h1"))
                    Console.WriteLine(link.InnerText);
                Console.WriteLine(new string('-', 20));
                foreach (HtmlNode link in document.DocumentNode.SelectNodes("//div[contains(@class, 'watchParams__item')]//a"))
                    Console.WriteLine(link.InnerText);
                Console.WriteLine(new string('-', 20));
                foreach (HtmlNode link in document.DocumentNode.SelectNodes("//div[contains(@class, 'clause__text-inner')]//p"))
                    Console.WriteLine(link.InnerText);
                break;
            }
        }
    }
}

