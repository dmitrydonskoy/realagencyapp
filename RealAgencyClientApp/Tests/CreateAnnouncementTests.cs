using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using Xunit;

namespace RealAgencyClientApp.Tests
{
    public class CreateAnnouncementTests
    {
        private readonly IWebDriver _driver;

        public CreateAnnouncementTests()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [Fact]
        public void CreateAnnouncement_ShouldSucceed_WhenDataIsValid()
        {
            // Arrange
            _driver.Navigate().GoToUrl("https://localhost:7206/Announcement/Create");

            // Act
            // Заполнение формы
            SelectDropdownValue("Type", "Продажа");
            _driver.FindElement(By.Id("Description")).SendKeys("Прекрасная квартира в центре города");

            // Информация о недвижимости
            SelectDropdownValue("RealEstate.Type", "Квартира");
            _driver.FindElement(By.Id("RealEstate.Address")).SendKeys("Улица Ленина, 10");
            _driver.FindElement(By.Id("RealEstate.Rooms")).SendKeys("3");
            _driver.FindElement(By.Id("RealEstate.Square")).SendKeys("120");
            _driver.FindElement(By.Id("RealEstate.Floor")).SendKeys("5");
            _driver.FindElement(By.Id("RealEstate.Bathroom")).SendKeys("1");
            _driver.FindElement(By.Id("RealEstate.Repair")).SendKeys("Евро");
            _driver.FindElement(By.Id("RealEstate.Furniture")).SendKeys("Есть");
            SelectDropdownValue("RealEstate.TransactionType", "Продажа");
            _driver.FindElement(By.Id("RealEstate.Price")).SendKeys("12000000");
            _driver.FindElement(By.Id("RealEstate.Description")).SendKeys("Современная квартира с видом на парк.");

            // Загрузка фото
            var photoInput = _driver.FindElement(By.Name("photo"));
            photoInput.SendKeys("C:\\Users\\User\\Pictures\\example.jpg");

            // Отправка формы
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // Assert
            var successMessage = _driver.FindElement(By.ClassName("alert-success")).Text;
            Assert.Contains("Announcement created successfully!", successMessage);
        }

        private void SelectDropdownValue(string elementId, string value)
        {
            var dropdown = _driver.FindElement(By.Id(elementId));
            var options = dropdown.FindElements(By.TagName("option"));
            foreach (var option in options)
            {
                if (option.Text == value)
                {
                    option.Click();
                    break;
                }
            }
        }

        public void Dispose()
        {
            _driver.Quit();
        }
    }
}
