namespace ShopTester.Tests;  
  
using System.Text.RegularExpressions;  
using Microsoft.Playwright;  
using Microsoft.Playwright.MSTest;  
  
  
[TestClass]  
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
            Headless = false, //Do you want to see it or no?  
            SlowMo = 1000 // Add a delay between actions  
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
        // Go to the specified URL  
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
        // Click the login link in nav.  
        await _page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();  
  
        // Fill out the login form and submit.  
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Email:" }).FillAsync(email);  
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password:" }).ClickAsync();  
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password:" }).FillAsync(password);  
        await _page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();  
  
        // Expect the page to have a welcome message and a logout button.  
        await Expect(_page.GetByRole(AriaRole.Button, new() { Name = "Logout" })).ToBeVisibleAsync();  
    }  
  
    [TestMethod]  
    public async Task TestToBuyProducts()  
    {  
        await GivenIAmOnTheHomePage("http://localhost:5000/");  
  
        await GivenIAmLoggedIn("john@email.com", "john123");  
  
        await WhenIClickTheButton("Shop");  
  
        await WhenIAddTheProductToCart("the-great-gatsby", 7);  
  
        await Expect(_page.Locator("#nav-cart")).ToContainTextAsync("Cart (7)");  
  
        await WhenIClickTheButton("Cart");  
    }  
}  
  
    // [TestMethod]  
    // public async Task Login()    // {    //     // Go to main page    //     await _page.GotoAsync("http://localhost:5000/");  
    //     // Click the login link in nav.    //     await _page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();  
    //     // Fill out the login form and submit.    //     await _page.GetByRole(AriaRole.Textbox, new() { Name = "Email:" }).FillAsync("admin@admin.com");    //     await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password:" }).ClickAsync();    //     await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password:" }).FillAsync("admin123");    //     await _page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();  
    //     // Expect the page to have a welcome message and a logout button.    //     await Expect(_page.Locator("#logged-in-user")).ToContainTextAsync("Welcome, Admin");    //     await Expect(_page.GetByRole(AriaRole.Button, new() { Name = "Logout" })).ToBeVisibleAsync();  
    //     // Click the shop link in nav.    //     await _page.GetByRole(AriaRole.Link, new() { Name = "Shop" }).ClickAsync();  
    //     // Expect page to have a button with the text 'Electronics'.    //     await Expect(_page.GetByRole(AriaRole.Button, new() { Name = "Electronics" })).ToBeVisibleAsync();  
    //     // Click the add to cart button for a laptop 2 times.    //     await _page.Locator("#button-add-laptop").ClickAsync();    //     await _page.Locator("#button-add-laptop").ClickAsync();  
    //     // Expect the cart link in nav to have the text 'Cart (2)'.    //     await Expect(_page.Locator("#nav-cart")).ToContainTextAsync("Cart (2)");  
    //     // Click the cart link in nav.    //     await _page.GetByRole(AriaRole.Link, new() { Name = "Cart (2)" }).ClickAsync();  
    //     // Expect the cart page to have the text 'Laptop'.    //     await Expect(_page.Locator("#Laptop")).ToContainTextAsync("Laptop");  
    //     // Click the checkout button.    //     await _page.GetByRole(AriaRole.Button, new() { Name = "Checkout" }).ClickAsync();  
    //     // Expect the page to show order placed successfully message.    //     await Expect(_page.GetByRole(AriaRole.Main)).ToContainTextAsync("Order placed successfully! Thank you for your purchase.");  
    //     // Click the profile link in nav.    //     await _page.GetByRole(AriaRole.Link, new() { Name = "Profile" }).ClickAsync();  
    //     // Expect the profile page to have the text 'Order Product: Laptop'.    //     await Expect(_page.GetByRole(AriaRole.Main)).ToContainTextAsync("Order Product: Laptop");    // }  
    // [TestMethod]    // public async Task CheckFilterProducts()    // {    //     // Go to main page    //     await _page.GotoAsync("http://localhost:5000/");  
    //     // Find the link button to shop  
    //     // await _page.GetByRole(AriaRole.Link, new() { Name = "Shop"}).ClickAsync();    //     await _page.Locator("#nav-shop").ClickAsync();  
    //     await Expect(_page.GetByRole(AriaRole.Button, new() { Name = "Electronics" })).ToBeVisibleAsync();  
    //     await _page.GetByRole(AriaRole.Button, new() { Name = "Electronics" }).ClickAsync();  
    //     await Expect(_page.GetByRole(AriaRole.Main)).ToContainTextAsync("Electronics");            //     // the site should not show books  
    //     var books = _page.GetByRole(AriaRole.Heading, new() { Name = "The Great Gatsby" });    //     // Assert that the books element is not visible    //     Assert.IsFalse(await books.IsVisibleAsync(), "Books should not be visible on the Electronics page.");  
    //     await Expect(_page.GetByRole(AriaRole.Heading, new() { Name = "The Great Gatsby" })).ToBeHiddenAsync();  
    //     await _page.GetByRole(AriaRole.Button, new() { Name = "Books" }).ClickAsync();  
    //     await Expect(_page.GetByRole(AriaRole.Main)).ToContainTextAsync("Books");    //     // the site should not show electronics    //     var electronics = _page.GetByRole(AriaRole.Heading, new() { Name = "Laptop" });    //     // Assert that the electronics element is not visible    //     Assert.IsFalse(await electronics.IsVisibleAsync(), "Electronics should not be visible on the Books page.");    //     await Expect(_page.GetByRole(AriaRole.Heading, new() { Name = "Laptop" })).ToBeHiddenAsync();  
    //     var book = _page.GetByRole(AriaRole.Heading, new() { Name = "The Great Gatsby" });    //     // Assert that the book element is visible    //     Assert.IsTrue(await book.IsVisibleAsync(), "Books should be visible on the Books page.");    //     await Expect(_page.GetByRole(AriaRole.Heading, new() { Name = "The Great Gatsby" })).ToBeVisibleAsync();  
    //     await _page.Locator("#button-add-the-great-gatsby").ClickAsync();  
    //     await Expect(_page.Locator("#nav-cart")).ToContainTextAsync("Cart (1)");    //     await _page.GetByRole(AriaRole.Link, new() { Name = "Cart (1)" }).ClickAsync();    //     await Expect(_page.GetByRole(AriaRole.Main)).ToContainTextAsync("The Great Gatsby");  
    // }
