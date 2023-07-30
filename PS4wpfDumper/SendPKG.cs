using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PS4wpfDumper;
using static PS4wpfDumper.MainWindow;



namespace PS4wpfDumper
{
    class SendPKG
    {
        public static async Task CheckAppExistsAsync()
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent($@"{CUSA}", Encoding.UTF8, "application/json");
                var response = await client.PostAsync($@"http://{ip}:12800/api/is_exists", content);
                var responseString = await response.Content.ReadAsStringAsync();
            }

        }

        public static async Task InstallMainPackage()
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent("{\"type\":\"direct\",\"packages\":[\"http://<local ip>:<local port>/UP1004-CUSA03041_00-REDEMPTION000002.pkg\"]}", Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://<PS4 IP>:12800/api/install", content);
                var responseString = await response.Content.ReadAsStringAsync();
            }
        }

        public static async Task UninstallGame()
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent("{\"title_id\":\"CUSA02299\"}", Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://<PS4 IP>:12800/api/uninstall_game", content);
                var responseString = await response.Content.ReadAsStringAsync();
            }
        }

        public static async Task PauseTask()
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent("{\"task_id\":123}", Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://<PS4 IP>:12800/api/pause_task", content);
                var responseString = await response.Content.ReadAsStringAsync();
            }
        }


    }
}
