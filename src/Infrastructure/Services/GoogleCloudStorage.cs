﻿using Application.Abstractions.Services;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Infrastructure.Services;

public class GoogleCloudStorage : ICloudStorage
{
    private readonly GoogleCredential googleCredential;
    private readonly StorageClient storageClient;
    private readonly string bucketName;

    public GoogleCloudStorage(IConfiguration configuration)
    {
        //string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //string currentDirectory = "D://Ky9//CaptonProject";
        string keyName = configuration.GetValue<string>("GoogleCredentialFile");
        //string keyAddress = Path.Combine(currentDirectory, keyName);
        //Console.WriteLine(">>>>>>>>>>", keyAddress);
        googleCredential = GoogleCredential.FromFile(keyName);
        storageClient = StorageClient.Create(googleCredential);
        bucketName = configuration.GetValue<string>("GoogleCloudStorageBucket");
    }

    public async Task<string> UploadFileAsync(IFormFile imageFile, string fileNameForStorage)
    {
        using (var memoryStream = new MemoryStream())
        {
            await imageFile.CopyToAsync(memoryStream);
            var dataObject = await storageClient.UploadObjectAsync(bucketName, fileNameForStorage, null, memoryStream);
            return dataObject.MediaLink;
        }
    }

    public async Task DeleteFileAsync(string fileNameForStorage)
    {
        await storageClient.DeleteObjectAsync(bucketName, fileNameForStorage);
    }

    public async Task<string> GetSignedUrlAsync(string fileNameForStorage)
    {
        if (string.IsNullOrWhiteSpace(fileNameForStorage))
        {
            fileNameForStorage = "no_image.jpg";
        }
        var sac = googleCredential.UnderlyingCredential as ServiceAccountCredential;
        var urlSigned = UrlSigner.FromServiceAccountCredential(sac);
        var signedUrl = await urlSigned.SignAsync(bucketName, fileNameForStorage, TimeSpan.FromMinutes(30));
        return signedUrl.ToString();
    }
}

