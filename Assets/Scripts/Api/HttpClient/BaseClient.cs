using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using UnityEngine;

using Zenject;

public class BaseClient
{
    public string BaseRoute = "http://localhost:5000/";

    [Inject]
    protected readonly HttpClient httpClient;

    protected async Task<T> GetAsync<T>(string route)
    {
        try
        {
            string result = await this.httpClient.GetStringAsync(route);
            return this.ReturnResult<T>(result);
        }
        catch (Exception ex) when (!(ex is ErrorException))
        {
            Debug.Log(ex.Message);
            return default;
        }
    }

    protected async Task<T> PostAsync<T>(string jsonObject, string route)
    {
        try
        {
            HttpContent content = new StringContent(
                            jsonObject,
                            Encoding.UTF8,
                            "application/json"
                        );

            HttpResponseMessage response = await this.httpClient.PostAsync(route, content);
            string result = await response.Content.ReadAsStringAsync();
            return this.ReturnResult<T>(result);
        }
        catch (Exception ex) when (!(ex is ErrorException))
        {
            Debug.Log(ex.Message);
            return default;
        }
    }

    protected async Task<T> PutAsync<T>(string jsonObject, string route)
    {
        try
        {
            HttpContent content = new StringContent(
                            jsonObject,
                            Encoding.UTF8,
                            "application/json"
                        );

            HttpResponseMessage response = await this.httpClient.PutAsync(route, content);
            string result = await response.Content.ReadAsStringAsync();
            return this.ReturnResult<T>(result);
        }
        catch (Exception ex) when (!(ex is ErrorException))
        {
            Debug.Log(ex.Message);
            return default;
        }
    }

    private T ReturnResult<T>(string result)
    {
        if (result != null)
        {
            try
            {
                Dictionary<string, string> errors = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
                if (errors == null || errors.Count == 0)
                {

                }
                else
                {
                    if (errors != null)
                    {
                        Debug.Log(errors);
                        throw new ErrorException(errors);
                    }
                }
            }
            catch (Exception ex) when (!(ex is ErrorException))
            {
                T objectResult = JsonConvert.DeserializeObject<T>(result);
                return objectResult;
            }
        }

        return default;
    }
}