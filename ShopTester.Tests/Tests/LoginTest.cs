namespace ShopTester.Tests;

using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;


[TestClass]
[TestCategory("Sequential")]
[DoNotParallelize]
public class LoginTest : PageTest
{
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IBrowserContext _browserContext;
    private IPage _page;

    [TestInitialize]
    public async Task Setup()
    {
        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            SlowMo = 1000 // Delay between actions
        });
        _browserContext = await _browser.NewContextAsync();
        _page = await _browserContext.NewPageAsync();
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        await _browserContext.CloseAsync();
        await _browser.CloseAsync();
        _playwright.Dispose();
    }

    public async Task GivenIAmOnTheHomePage(string url)
    {
    
        await _page.GotoAsync(url);
    }

    public async Task WhenIClickTheButton(string buttonName)
    {
        await _page.GetByRole(AriaRole.Link, new() { Name = $"{buttonName}" }).ClickAsync();
    }

    public async Task WhenIAddTheProductToCart(string productName, int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            await _page.Locator($"#button-add-{productName.ToLower()}").ClickAsync();
        }
    }

    public async Task GivenIAmLoggedIn(string email, string password)
    {
        await _page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();

        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Email:" }).FillAsync(email);
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password:" }).ClickAsync();
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password:" }).FillAsync(password);
        await _page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();

        await Expect(_page.GetByRole(AriaRole.Button, new() { Name = "Logout" })).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task TestToBuyProductsAndCheckProfile()
    {
        await GivenIAmOnTheHomePage("http://localhost:5000/");

        await GivenIAmLoggedIn("john@email.com", "john123");

        await WhenIClickTheButton("Shop");

        await WhenIAddTheProductToCart("Smartphone", 1);

        await WhenIAddTheProductToCart("monitor", 1);
        
        await Expect(_page.Locator("#nav-cart")).ToContainTextAsync("Cart (2)");

        await WhenIClickTheButton("Cart");

        // Targets the fist button named delete
        await _page.Locator("#button-delete").First.ClickAsync();

        await WhenIClickTheButton("Shop");
        
        await WhenIAddTheProductToCart("tablet", 1);
        
        await WhenIAddTheProductToCart("jeans", 1);

        await Expect(_page.Locator("#nav-cart")).ToContainTextAsync("Cart (3)");

        await WhenIClickTheButton("Cart");

        await _page.Locator("#checkout-button").WaitForAsync(new() { State = WaitForSelectorState.Visible });

        await _page.ClickAsync("#checkout-button");

        await Expect(_page.GetByRole(AriaRole.Main)).ToContainTextAsync("Order placed successfully! Thank you for your purchase.");

        await WhenIClickTheButton("Profile"); 

        await _page.Locator("#logout-button").WaitForAsync(new() { State = WaitForSelectorState.Visible });

        await _page.ClickAsync("#logout-button");
    }

    [TestMethod]
     public async Task AdminLogInAndCheckOrders()
     {
        await GivenIAmOnTheHomePage("http://localhost:5000/");

        await GivenIAmLoggedIn("admin@admin.com", "admin123");

        await WhenIClickTheButton("Shop");

        await _page.Locator("button", new() { HasTextString = "Electronics" }).ClickAsync();

        await _page.Locator("button", new() { HasTextString = "Books" }).ClickAsync();
        
        await _page.Locator("button", new() { HasTextString = "Clothing" }).ClickAsync();

        await _page.Locator("button", new() { HasTextString = "Food" }).ClickAsync();
        
        await _page.Locator("button", new() { HasTextString = "All" }).ClickAsync();

        await WhenIClickTheButton("Admin");

        await _page.Locator("button", new() { HasTextString = "Fetch Users" }).ClickAsync();

        await _page.Locator("button", new() { HasTextString = "Fetch Orders" }).ClickAsync();

        await _page.Locator("button", new() { HasTextString = "Fetch Products" }).ClickAsync();

        await WhenIClickTheButton("Profile"); 

        await _page.Locator("#logout-button").WaitForAsync(new() { State = WaitForSelectorState.Visible });

        await _page.ClickAsync("#logout-button");
     }
}
