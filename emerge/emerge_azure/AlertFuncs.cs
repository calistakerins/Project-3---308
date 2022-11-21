﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using emerge_azure.Models;
using Newtonsoft.Json;
using System.IO;
using MongoDB.Driver;
using System;
using MongoDB.Bson;

public static class AlertFuncs
{
    public static readonly string connectionString = "mongodb+srv://emerge:project3@cluster0.ztzvtkd.mongodb.net/?retryWrites=true&w=majority";

    [FunctionName("get_newsalert")]
    public static async Task<IActionResult> GetNewsAlerts(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "newsalert")] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("Called get_newsalert with GET request");
        MongoClient dbClient = new MongoClient(connectionString);
        var database = dbClient.GetDatabase("alerts");
        var collection = database.GetCollection<BsonDocument>("alertInfo");
        var documents = collection.AsQueryable();
        foreach (BsonDocument alert in documents)
        {
            Console.Write(alert["Title"].AsString);
        }

        return new OkObjectResult(NewsAlertFeed.newsalerts);
    }

    //[FunctionName("get_newsalert_byID")]
    //public static async Task<IActionResult> GetNewsAlertsById(
    //        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "fooditem/{id}")] HttpRequest req,
    //        ILogger log, string id)
    //{
    //    log.LogInformation("Called get_newsalert_byID with GET request");

    //    var newsItem = NewsAlertFeed.newsalerts.FirstOrDefault(f => f.Id.Equals(id));
    //    if (newsItem == null)
    //    {
    //        return new NotFoundResult();
    //    }

    //    return new OkObjectResult(newsItem);
    //}

    [FunctionName("add_newsalert")]
    public static async Task<IActionResult> AddNewsAlert(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "newsalert")] HttpRequest req,
            ILogger log)
    {
        log.LogInformation("Called add_newsalert with POST request");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        var newsalert = JsonConvert.DeserializeObject<NewsAlert>(requestBody);

        NewsAlertFeed.newsalerts.Add(newsalert);

        return new OkObjectResult(newsalert);
    }
}