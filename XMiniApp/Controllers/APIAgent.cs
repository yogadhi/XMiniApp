using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XMiniApp.DependencyServices;
using Newtonsoft.Json;
using XMiniApp.Models;

namespace XMiniApp.Controllers
{
    public class APIAgent
    {
        private static HttpClient client;
        public enum APIEnum
        {
            GET = 1,
            POST = 2,
            DELETE = 3,
            PUT = 4
        }

        public static string baseAddress()
        {
            return "http://119.8.184.140";
        }

        static APIAgent()
        {

        }

        public static async Task<string> AccessAPI(APIEnum enums, double timeOutMinute, string functionURL, string accessToken, string jsonParam)
        {
            string resJson = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                client = new HttpClient();
                client.BaseAddress = new Uri(baseAddress());
                client.DefaultRequestHeaders.Accept.Clear();
                if (!string.IsNullOrEmpty(accessToken))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromMinutes(timeOutMinute);

                if (enums == APIEnum.POST)
                {
                    var content = new StringContent(jsonParam, Encoding.UTF8, "application/json");
                    response = await client.PostAsync(functionURL, content);
                }
                else if (enums == APIEnum.GET)
                {
                    response = await client.GetAsync(functionURL);
                }

                if (response.IsSuccessStatusCode)
                {
                    resJson = await response.Content.ReadAsStringAsync();
                }

            }
            catch (TaskCanceledException taskeX)
            {
                string taskMsg = taskeX.Message.ToString();
            }
            catch (WebException webEx)
            {
                string webExMsg = webEx.Message.ToString();
            }
            catch (Exception ex)
            {
                string exMsg = ex.Message.ToString();
            }
            return resJson;
        }

        public static async Task<string> GenerateToken()
        {
            string res = string.Empty;

            try
            {
                List<string> formatList = new List<string>();

                var userID = Guid.NewGuid().ToString();
                var deviceID = DependencyService.Get<IDeviceLinkedService>().GetDeviceID();
                var registerDate = new DateTime(2020, 1, 1);
                var expiredDate = registerDate.AddYears(5);

                formatList.Add("Quote123!");
                formatList.Add(userID);
                formatList.Add(deviceID);
                formatList.Add(registerDate.ToString("yyyy-MM-dd"));
                formatList.Add(expiredDate.ToString("yyyy-MM-dd"));

                var objEnc = new GoEncryptReq
                {
                    PlainText = string.Join("|", formatList),
                    Key = "UjUyWVZGQldQNVFJUzJOSkJPT0FZWUlMTU5EM0FUWkc="
                };

                var reqJson = JsonConvert.SerializeObject(objEnc);

                var objEncAPI = await APIAgent.AccessAPI(APIAgent.APIEnum.POST, 3, "MiniAPI/GoEncrypt", "", reqJson);
                if (!string.IsNullOrEmpty(objEncAPI))
                {
                    var resEncrypt = JsonConvert.DeserializeObject<GoEncryptRes>(objEncAPI);
                    if (resEncrypt != null)
                    {
                        if (!string.IsNullOrEmpty(resEncrypt.EncryptedText))
                        {
                            res = resEncrypt.EncryptedText;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string exMsg = ex.Message.ToString();
            }
            return res;
        }
    }
}
