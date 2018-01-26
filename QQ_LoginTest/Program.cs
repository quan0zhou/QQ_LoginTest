using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Chrome;
using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace QQ_LoginTest
{
    class Program
    {
    
        static void Main(string[] args)
          {
       
             ChromeDriver driver = new ChromeDriver();

            //(Iframe中拖动元素无效的问题)https://stackoverflow.com/questions/46271789/drag-and-drop-doesnt-work-inside-iframe-with-selenium-webdriver-with-java

            driver.Navigate().GoToUrl("https://qzone.qq.com/");
            driver.Manage().Window.Maximize();//窗口最大化，便于脚本执行

            //设置超时等待(隐式等待)时间设置10秒
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10000);

            IWebDriver login = driver.SwitchTo().Frame(driver.FindElementById("login_frame"));
            login.FindElement(By.Id("switcher_plogin")).Click();
            login.FindElement(By.Id("u")).SendKeys("63365318");
            login.FindElement(By.Id("p")).SendKeys("lu35721450");
            if (login.FindElement(By.Id("err_m")).Text.Trim() == "请输入正确的帐号！")
            {
                Console.WriteLine("账号错误");
            }
            else
            {
                //登陆QQ
                login.FindElement(By.Id("login_button")).Click();

            }
    
            //获取到验证区域
            IWebDriver validate = login.SwitchTo().Frame(login.FindElement(By.TagName("iframe")));

            Thread.Sleep(2000);
            var slideBkg = validate.FindElement(By.Id("slideBkg"));//获取滑动验证图片
            Console.WriteLine(slideBkg.Size.Width);
            Console.WriteLine(slideBkg.Size.Height);

            //例如（带阴影）地址：https://ssl.captcha.qq.com/cap_union_new_getcapbysig?aid=549000912&asig=&captype=&protocol=https&clientype=2&disturblevel=&apptype=2&curenv=inner&ua=TW96aWxsYS81LjAgKFdpbmRvd3MgTlQgMTAuMDsgV09XNjQpIEFwcGxlV2ViS2l0LzUzNy4zNiAoS0hUTUwsIGxpa2UgR2Vja28pIENocm9tZS82NC4wLjMyODIuMjUgU2FmYXJpLzUzNy4zNg==&sess=GLJWtGBjtiY9IU65Duozl2CTJgVIeeI5j0T-aRhSyUHy1n_Hw4tI5hTPv8L4cVel2MHC-Tcr60K7Bn8i61wc7QIGZtkTtRHvxSNrCZPKeIix_yPRM6DVRgPABbeCtxomRu2iAIPw4catxqVGExbTMPutJWm4_prJkGe4jpALYW6-EO24eqD8PMCUTDSycH72eAEDAhmSKK8*&theme=&noBorder=noborder&fb=1&showtype=embed&uid=222222222&cap_cd=ZJBJeRGBPVMXgLfm8Q6NEMfhQUuBD25K8Z1dButH6TX_U0f-DWE9CQ**&lang=2052&rnd=828269&rand=0.22818125792486477&vsig=c01HtB-jbxc5jTAG-7hEWHT2NEe0uXQbqceH5xovPH4TrfUU7sofwExBehTib403f-aSAWGThq9_HPwhXZxO9m-d7EqoOMdVTaCkje30e5bjEqUM5wVvIr8VEGFKFIvu7HOxTJaz4R0cb9Hnb-7Sigh7INiSdr7uWSdjcpmUJgxrdAxsFz2xPSPLA**&img_index=1
            string newUrl = slideBkg.GetAttribute("src");
            //原图地址：https://ssl.captcha.qq.com/cap_union_new_getcapbysig?aid=549000912&asig=&captype=&protocol=https&clientype=2&disturblevel=&apptype=2&curenv=inner&ua=TW96aWxsYS81LjAgKFdpbmRvd3MgTlQgMTAuMDsgV09XNjQpIEFwcGxlV2ViS2l0LzUzNy4zNiAoS0hUTUwsIGxpa2UgR2Vja28pIENocm9tZS82NC4wLjMyODIuMjUgU2FmYXJpLzUzNy4zNg==&sess=GLJWtGBjtiY9IU65Duozl2CTJgVIeeI5j0T-aRhSyUHy1n_Hw4tI5hTPv8L4cVel2MHC-Tcr60K7Bn8i61wc7QIGZtkTtRHvxSNrCZPKeIix_yPRM6DVRgPABbeCtxomRu2iAIPw4catxqVGExbTMPutJWm4_prJkGe4jpALYW6-EO24eqD8PMCUTDSycH72eAEDAhmSKK8*&theme=&noBorder=noborder&fb=1&showtype=embed&uid=222222222&cap_cd=ZJBJeRGBPVMXgLfm8Q6NEMfhQUuBD25K8Z1dButH6TX_U0f-DWE9CQ**&lang=2052&rnd=828269&rand=0.22818125792486477&vsig=c01HtB-jbxc5jTAG-7hEWHT2NEe0uXQbqceH5xovPH4TrfUU7sofwExBehTib403f-aSAWGThq9_HPwhXZxO9m-d7EqoOMdVTaCkje30e5bjEqUM5wVvIr8VEGFKFIvu7HOxTJaz4R0cb9Hnb-7Sigh7INiSdr7uWSdjcpmUJgxrdAxsFz2xPSPLA**&img_index=0
            string oldUrl = newUrl.Substring(0, newUrl.Length - 1) + "0";
            //根据地址获取到图像
            Bitmap oldBmp = (Bitmap)LoginHelper.GetImg(oldUrl);
            Bitmap newBmp = (Bitmap)LoginHelper.GetImg(newUrl);

            driver.GetScreenshot().SaveAsFile("原图.png");
            oldBmp.Save("原始验证图.png");
            newBmp.Save("阴影验证图.png");

            int left = LoginHelper.GetArgb(oldBmp, newBmp);//得到阴影到图片左边界的像素
            int leftShift = (int)(left * ((double)slideBkg.Size.Width / (double)newBmp.Width) - 18);//得到真实的偏移值


            //validate.SwitchTo();

            var slideBlock = validate.FindElement(By.Id("slideBlock"));//获取到滑块
            Console.WriteLine($"开始位置：{slideBlock.Location.X}");
            Actions actions = new Actions(driver);
            //actions.ClickAndHold(slideBlock).Build().Perform();
            //actions.DragAndDropToOffset(slideBlock, 50, 0).Perform();
            //actions.Click(tcaptcha_drag_button).Perform();
            //actions.ClickAndHold(tcaptcha_drag_button).Release().Perform();
            actions.DragAndDropToOffset(slideBlock, leftShift, 0).Build().Perform();//单击并在指定的元素上按下鼠标按钮,然后移动到指定位置
            //Thread.Sleep(2000);
            //Console.WriteLine($"移动后位置：{slideBlock.Location.X}");
            //actions.DragAndDropToOffset(slideBlock,leftShift, 0).Perform();//移动滑块到阴影处
            //Thread.Sleep(2000);
            driver.GetScreenshot().SaveAsFile("移动后图片.png");





        }






    }
}
