using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;

namespace ErtugrulGokayDumanHesVenturesCaseStudy.Services.PttScrapingServices
{
    public class PttScrapingService : IPttScrapingService, IDisposable
    {
        private readonly IWebDriver _driver;
        private const string PTT_URL = "https://gonderitakip.ptt.gov.tr/";
        private readonly ILogger<PttScrapingService> _logger;

        public PttScrapingService(ILogger<PttScrapingService> logger, ChromeDriver driver)
        {
            _logger = logger;
            var options = new ChromeOptions();
            options.AddArguments("--headless"); // Headless mod için
            options.AddArguments("--no-sandbox");
            options.AddArguments("--disable-dev-shm-usage");

            _driver = new ChromeDriver(options);
        }

        public async Task<string> GetTrackingStatus(string trackingNumber)
        {
            try
            {
                _driver.Navigate().GoToUrl(PTT_URL);

                // Barkod input alanını bul
                var barcodeInput = _driver.FindElement(By.Id("searchForm"));
                barcodeInput.Clear();
                barcodeInput.SendKeys(trackingNumber);

                // Sorgula butonunu bul ve tıkla
                var searchButton = _driver.FindElement(By.Id("searchButton"));
                searchButton.Click();

                // Sonucun yüklenmesini bekle
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10)); var statusElement = wait.Until(driver => driver.FindElement(By.CssSelector(".gonderi-durumu")));

                return statusElement.Text.Trim();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scraping PTT status for tracking number {TrackingNumber}", trackingNumber);
                return "Error: Could not fetch status";
            }
        }

        public void Dispose()
        {
            _driver?.Quit();
            _driver?.Dispose();
        }
    }
}
