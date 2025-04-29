using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyFinder.Services
{
    public class MonkeyService
    {

        private readonly List<Monkey> _monkeys = new();
        private readonly HttpClient _httpClient;

        public MonkeyService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://montemagno.com")
            };
        }

        public async Task<IEnumerable<Monkey>> GetMonkeysAsync()
        {
            if (_monkeys.Count > 0)
            {
                return _monkeys;
            }

            var response = await _httpClient.GetAsync("monkeys.json");
            if (response.IsSuccessStatusCode)
            {
                var monkeys = await response.Content.ReadFromJsonAsync<List<Monkey>>() ?? [];

                var monkey = monkeys.FirstOrDefault(monkey => monkey.Name == "Baboon");
                if (monkey != null)
                {
                    monkey.Latitude = 51.4617815954;
                    monkey.Longitude = 0.0276079773903;
                }

                _monkeys.AddRange(monkeys);
            }

            return _monkeys;
        }
    }
}
