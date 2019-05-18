using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SkillAlice
{
    class Request
    {
        public delegate void OnRequestComplete(string data); // Request Complete Callback
        public delegate void OnRequestError(string code); // Request Error Callback

        internal static async Task<bool> Get(string url, FormUrlEncodedContent data, OnRequestComplete complete, OnRequestError error) 
        {
            try
            { // Creating client and send request
                // Set client vars
                var myHttpClient = new HttpClient(); // Create HTTP Client
                var response = await myHttpClient.PostAsync(url, data); // HTTP Response
                var json = await response.Content.ReadAsStringAsync(); // JSON Data
                try
                { 
                    complete(json); // Show Complete
                    return true;
                }
                catch (Exception ex)
                { // Failed to decode data
                    error(ex.Message); // Show Error
                    return false;
                }
            }
            catch (Exception ex)
            { // Failed to send request
                error(ex.Message); // Show Error
                return false;
            }
        }


    }
}
