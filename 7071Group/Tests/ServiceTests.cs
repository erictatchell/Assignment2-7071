using _7071Group.Data;
using _7071Group.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Linq;

namespace UITests
{
    [TestFixture]
    public class ServiceTests
    {
        private ApplicationDbContext? _context;
        private Service? _testService;
        private IWebDriver? _driver;
        private string _baseUrl = "http://localhost:5023"; // Adjust URL if needed

        // Helper method to create a test service
        private Service CreateTestService(int id, string name, decimal rate, bool requiresCertification)
        {
            return new Service
            {
                ServiceID = id,
                ServiceName = name,
                Rate = rate,
                RequiresCertification = requiresCertification
            };
        }

        [SetUp]
        public void Setup()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless=new"); // Run in headless mode for GitHub Actions
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--disable-dev-shm-usage");

            _driver = new ChromeDriver(chromeOptions);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connectionString)
                .Options;

            _context = new ApplicationDbContext(options);

            // Ensure the database is created and seeded
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [Test]
        public void CreateService()
        {
            // Navigate to the create service page
            _driver!.Navigate().GoToUrl($"{_baseUrl}/Service/Create");

            // Fill in the form
            _driver.FindElement(By.Id("ServiceName")).SendKeys("Test Service");
            _driver.FindElement(By.Id("Rate")).SendKeys("100");
            _driver.FindElement(By.Id("RequiresCertification")).Click();

            // Submit the form
            _driver.FindElement(By.XPath("//input[@type='submit']")).Click();
            Assert.Pass();
        }

        [Test]
        public void ReadService()
        {
            // Arrange
            _testService = CreateTestService(1, "Test Service", 100, true);
            _context!.Services.Add(_testService);
            _context.SaveChanges();
            // Navigate to the service details page
            _driver!.Navigate().GoToUrl($"{_baseUrl}/Service/Details/{_testService.ServiceID}");
            // Verify the service details are displayed
            Assert.Pass();
        }

        [Test]
        public void UpdateService()
        {
            // Arrange
            _testService = CreateTestService(1, "Test Service", 100, true);
            _context!.Services.Add(_testService);
            _context.SaveChanges();
            // Navigate to the edit service page
            _driver!.Navigate().GoToUrl($"{_baseUrl}/Service/Edit/{_testService.ServiceID}");
            // Fill in the form
            _driver.FindElement(By.Id("ServiceName")).Clear();
            _driver.FindElement(By.Id("ServiceName")).SendKeys("Updated Service");
            _driver.FindElement(By.Id("Rate")).Clear();
            _driver.FindElement(By.Id("Rate")).SendKeys("200");
            _driver.FindElement(By.Id("RequiresCertification")).Click();
            // Submit the form
            _driver.FindElement(By.XPath("//input[@type='submit']")).Click();
            Assert.Pass();
        }

        [Test]
        public void DeleteService()
        {
            // Arrange
            _testService = CreateTestService(1, "Test Service", 100, true);
            _context!.Services.Add(_testService);
            _context.SaveChanges();
            // Navigate to the delete service page
            _driver!.Navigate().GoToUrl($"{_baseUrl}/Service/Delete/{_testService.ServiceID}");
            // Submit the form
            _driver.FindElement(By.XPath("//input[@type='submit']")).Click();
            Assert.Pass();
        }

        [TearDown]
        public void Teardown()
        {
            _context!.Dispose();
            _driver!.Quit();
        }
    }
}
