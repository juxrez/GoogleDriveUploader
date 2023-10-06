using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Util.Store;
using System.Text.Json;
using System.Text.Json.Serialization;
using Google.Apis.Auth.OAuth2.Flows;
using GoogleFile = Google.Apis.Drive.v3.Data.File;
using System.Net;

namespace GoogleDriveUploader.Services;

public class GoogleDriveService
{
    private static DriveService _driveService;

    public static DriveService DriveService => _driveService;

    public GoogleDriveService()
    {
        //string credentialsJson = ReadCredentials();
        //using var stream = File.OpenRead(@"C:\credentials.json");

        //var credentials = GoogleCredential
        //    .(stream) 
        //    .CreateScoped(DriveService.ScopeConstants.Drive);

        var credentials = GetCredentials().Result;

        _driveService = new DriveService(new Google.Apis.Services.BaseClientService.Initializer()
        {
            HttpClientInitializer = credentials,
            ApplicationName = "Google Drive Uploader"
        });

    }


    private async Task<UserCredential> GetCredentials()
    {
        await using var stream = File.OpenRead(@"C:\credentials.json")
                                 ?? throw new FileNotFoundException();

        var credentials = await GoogleWebAuthorizationBroker.AuthorizeAsync((await
                GoogleClientSecrets.FromStreamAsync(stream)).Secrets,
            new[] { DriveService.ScopeConstants.Drive },
            "user", CancellationToken.None);

        return credentials;
    }

    private string Set()
    {

        IAuthorizationCodeFlow flow =
            new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = "PUT_CLIENT_ID_HERE",
                    ClientSecret = "PUT_CLIENT_SECRET_HERE"
                },
                Scopes = new[] { DriveService.Scope.Drive },
                DataStore = new FileDataStore("Drive.Api.Auth.Store")
            });
    }



    private static string ReadCredentials()
    {
        try
        {
             string stream = 
                File.ReadAllText(@"C:\credentials.json") 
                ?? throw new Exception("credentials missing");

             return stream;
             //return JsonSerializer.Deserialize<string>(stream)!;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed reading the credentials.json {ex.Message}");
        }
    }

    public async Task<(bool Success, string Message)> UploadFile(string filePath, string filename)
    {
        try
        {
            var fileMetadata = new GoogleFile()
            {
                Name = filename
            };
            
            using var stream = File.OpenRead(Path.Combine(filePath, filename));
            
            var request = _driveService.Files.Create(fileMetadata, stream, "application/octet-stream");
            request.Fields = "";
            
            var progress = await request.UploadAsync(CancellationToken.None);

            return progress.Status is Google.Apis.Upload.UploadStatus.Completed ? 
                (true, "Ok") 
                : (false, progress.Exception.Message);

        }
        catch (FileNotFoundException ex) 
        {
            return (false, ex.Message);
        }
        catch (Exception ex) 
        {

            throw;
        }
    }
}
