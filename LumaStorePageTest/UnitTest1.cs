using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using static System.Net.WebRequestMethods;

namespace LumaStorePageTest
{
    public class Tests
    {
        String lumaUrl = "https://magento.softwaretestingboard.com/";
        IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            // Initialize the WebDriver instance and maximize the browser window
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
        }

        /// <summary>
        /// Tests that an item with all options selected can be added to the cart.
        /// </summary>
        [Test]
        public void ValidateItemAndOptionsAddedToCart()
        {
            // Navigate to the Luma Store page and wait for the page to load.
            OpenLumaStore();

            // Select
            SelectRadiantTeeOptions();

            // Wait for a brief delay to allow the cart to update if necessary
            WebDriverWait cartCounterWait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            IWebElement cartCounter = cartCounterWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//span[@class='counter-number']")));

            // Verify that the cart counter has incremented after the 'Add to Cart' button is clicked
            Assert.That(GetCartItemCount(), Is.EqualTo("1"));

            // Display the mini-cart popup
            cartCounter.Click();

            // Expand the item details section of the mini-cart popup
            IWebElement detailsExpander = driver.FindElement(By.XPath("//*[@id=\"mini-cart\"]/li/div/div/div[1]/span"));
            detailsExpander.Click();

            // Locate the fields displaying the selected options for the item added
            IWebElement cartItemSize = driver.FindElement(By.XPath("//*[@id=\"mini-cart\"]/li/div/div/div[1]/div/dl/dd[1]/span"));
            IWebElement cartItemColor = driver.FindElement(By.XPath("//*[@id=\"mini-cart\"]/li/div/div/div[1]/div/dl/dd[2]/span"));

            // Verify that the cart displays the matching options selected
            Assert.That(cartItemSize.Text, Is.EqualTo("XL"));
            Assert.That(cartItemColor.Text, Is.EqualTo("Purple"));
        }

        /// <summary>
        /// Tests that the cart counter increments correctly when an item is added.
        /// </summary>
        [Test]
        public void ValidateCartCounterIncrementsAccurately()
        {
            OpenLumaStore();
            SelectRadiantTeeOptions();

            Assert.That(GetCartItemCount(), Is.EqualTo("1"));

            //int totalExpectedItems = 1 + AddExtraItemsToCart();

            // Add a second item to the cart to test cart counter accuracy
            SelectRadiantTeeOptions();

            Assert.That(GetCartItemCount(), Is.EqualTo("2"));
        }

        /// <summary>
        /// Tests that the correct success message is displayed when adding an item to the cart.
        /// </summary>
        [Test]
        public void ValidateSuccessfulAddMessageDisplays()
        {
            String successfulAddMessage = "You added Radiant Tee to your shopping cart.";
            String successfulAddMessageLocator = "//div[@data-bind='html: $parent.prepareMessageForHtml(message.text)']";

            OpenLumaStore();
            SelectRadiantTeeOptions();

            WebDriverWait addSuccessWait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            IWebElement addSuccessMessage = addSuccessWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath($"{successfulAddMessageLocator}")));

            Assert.That(addSuccessMessage.Text, Is.EqualTo(successfulAddMessage));
        }

        /// <summary>
        /// Tests that the correct warning will be displayed when an item is added to the cart with insufficient options selected.
        /// </summary>
        [Test]
        public void ValidateFailedAddMessageDisplays()
        {
            OpenLumaStore();
            AddArgusTankWithoutOptions();
        }

        /// <summary>
        /// Disposes of the WebDriver instance created to run the tests.
        /// </summary>
        [TearDown]
        public void CleanUp()
        {
            driver.Dispose();
        }

        /// <summary>
        /// Navigates to the Luma store page and waits for the page to load.
        /// </summary>
        public void OpenLumaStore()
        {
            // Navigate to the Luma Store page and wait for the page to load.
            driver.Url = lumaUrl;

            // Wait until the 'Home Page' banner is displayed at the top of the page
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(2000));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//span[@class='base']")));

            // Update progress on Console for debugging reference
            Console.WriteLine("Luma Store page successfully opened.");
        }

        /// <summary>
        /// Selects all the necessary options for the Radiant Tee and adds the item to the cart.
        /// </summary>
        public void SelectRadiantTeeOptions()
        {
            // Locate and create objects for each of the Radiant Tee buttons to be used
            IWebElement radiantTeeExtraLargeButton = driver.FindElement(By.XPath("//div[@class='swatch-opt-1556']//div[@id='option-label-size-143-item-170']"));
            IWebElement radiantTeePurpleButton = driver.FindElement(By.Id("option-label-color-93-item-57"));
            IWebElement radiantTeeAddToCartButton = driver.FindElement(By.XPath("//li[1]//div[1]//div[1]//div[4]//div[1]//div[1]//form[1]//button[1]//span[1]"));

            // Click the Radiant Tee option buttons
            radiantTeeExtraLargeButton.Click();
            radiantTeePurpleButton.Click();
            radiantTeeAddToCartButton.Click();

            // Update progress on Console for debugging reference
            Console.WriteLine("Options selected and Radiant Tee added to cart.");
        }

        /// <summary>
        /// Clicks the close button on the mini-cart popup, closing the mini-cart popup.
        /// </summary>
        public void CloseMiniCartPopup()
        {
            IWebElement closeButton = driver.FindElement(By.Id("btn-minicart-close"));
            closeButton.Click();
        }

        /// <summary>
        /// Attempts to add the Argus Tank item to the cart with insufficient options selected.
        /// </summary>
        public void AddArgusTankWithoutOptions()
        {            
            String missingOptionsMessage = "You need to choose options for your item.";
            WebDriverWait pageLoadWait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            // Locate and create objects for each of the Argus Tank buttons to be used (no color button used to trigger warning)
            IWebElement argusTankSmallButton = driver.FindElement(By.XPath("//div[@class='swatch-opt-694']//div[@id='option-label-size-143-item-167']"));
            IWebElement argusTankAddToCartButton = driver.FindElement(By.XPath("//*[@id=\"maincontent\"]/div[3]/div/div[2]/div[5]/div/div/ol/li[3]/div/div/div[3]/div/div[1]/form/button"));
            
            // Click the Argus Tank size button and Add to Cart button, skipping a color selection
            argusTankSmallButton.Click();
            argusTankAddToCartButton.Click();

            // Wait for the warning field to load, then compare the warning text to the expected warning
            IWebElement addToCartWarning = pageLoadWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//div[@data-bind='html: $parent.prepareMessageForHtml(message.text)']")));
            Assert.That(addToCartWarning.Text, Is.EqualTo(missingOptionsMessage));

            // Update progress on Console for debugging reference
            Console.WriteLine("Warning displayed due to insufficient options selected on Argus Tank");
        }

        /// <summary>
        /// Gets the number of items currently added to the cart as a String.
        /// </summary>
        /// <returns>A String reflecting the number of items in the cart.</returns>
        public String GetCartItemCount()
        {
            // Wait for a brief delay to allow the cart to update if necessary
            WebDriverWait cartCounterWait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
            IWebElement cartCounter = cartCounterWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//span[@class='counter-number']")));

            // Update progress on Console for debugging reference
            Console.WriteLine("Cart Item Count retrieved: " + cartCounter.Text);

            // Parse the cart counter text and return the value as an Integer
            return cartCounter.Text;
        }
    }
}