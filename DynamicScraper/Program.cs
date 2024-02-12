using System;
using System.Threading.Tasks;
using PuppeteerSharp;
using PuppeteerSharp.Dom;

class Program
{
    static async Task Main(string[] args)
    {
        await new BrowserFetcher().DownloadAsync();
        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });

        var page = await browser.NewPageAsync();
        await page.GoToAsync("https://reverb.com/marketplace?query=Ibanez%20Pia&make=ibanez&product_type=electric-guitars&show_only_sold=true");

        Console.WriteLine("Enter search term: ");
        var userInput = Console.ReadLine();

        var searchBox = await page.QuerySelectorAsync("input[class='site-search__controls__input']");
        await searchBox.TypeAsync(userInput);

        var searchButton = await page.QuerySelectorAsync("a[class='site-search__controls__submit']");
        await searchButton.ClickAsync();

        //var element = await page.QuerySelectorAllAsync<HtmlDivElement>("rc-pricing-block__price");
        //var innerText = await element.GetInnerTextAsync();

        await page.WaitForSelectorAsync(".rc-price-block__price");
         //Get price for each item



        Dictionary<string, string> namePricePairs = new Dictionary<string, string>();
        var element = await page.QuerySelectorAllAsync(".rc-price-block__price");
        var nameElement = await page.QuerySelectorAllAsync(".rc-listing-card__title");

        if(element.Length != nameElement.Length)
        {
            throw new Exception("Length of price and name elements do not match");
        }


       for(int i = 0; i < element.Length; i++)
        {
            var price = await element[i].EvaluateFunctionAsync<string>("e => e.innerText");
            var name = await nameElement[i].EvaluateFunctionAsync<string>("e => e.innerText");
            namePricePairs[name] = price;
        }

       foreach(var pair in namePricePairs)
        {
            Console.WriteLine($"{pair.Key} : {pair.Value}");
        }   

          








        // Close the browser
        await browser.CloseAsync();
    }
}
