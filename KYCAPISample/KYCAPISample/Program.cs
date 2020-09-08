using System;
using System.IO;
using System.Text.Json;

namespace KYCAPISample
{
  public class Program
  {
    private static KYCReviewer reviewer = null;
    private static KYCUser user = null;
    private static ConsoleColor init = ConsoleColor.White;

    /// <summary>
    /// Print out input menu in the screen 
    /// </summary>
    public static void PrintOptions()
    {
      Console.WriteLine(
        "Select your options below" + Environment.NewLine +
        "1. List" + Environment.NewLine +
        "2. Download" + Environment.NewLine + 
        "3. Approve" + Environment.NewLine +
        "4. Delete" + Environment.NewLine +
        "5. Exit" + Environment.NewLine +
        "Enter your input below");
    }

    /// <summary>
    /// Validate the input text for Download, and Approve options
    /// for these two options, the user will need to specify both
    /// the id of the pack and the flag for blockchain notarization
    /// </summary>
    /// <param name="input">input text to validate</param>
    /// <returns>true if the user input is valid</returns>
    public static bool ValidateInput(string input)
    {
      var parts = input.Split(" ");
      if (parts.Length != 2)
        return false;
      if (!bool.TryParse(parts[1], out _))
        return false;
      return true;
    }

    /// <summary>
    /// User interaction for each option
    /// Provide input/and output
    /// </summary>
    /// <param name="option">between 1 - 5</param>
    public static void Action(int option)
    {
      switch (option)
      {
        case 1:
          Console.WriteLine("Action - List");
          var result = reviewer.List().Result;
          if(result.Item1)
          {
            Console.WriteLine(result.Item2);
          }
          else
          {
            Console.WriteLine("action Failed");
          }
          Console.WriteLine("Action - END");
          break;
        case 2:
          Console.WriteLine("Action - Download");
          Console.WriteLine("Please specify the id of the pack, and whether you want to notarise the download or not.");
          Console.WriteLine("(Format: [ID_OF_PACK] [true/false]");
          var input = Console.ReadLine();
          while(!ValidateInput(input) && !string.IsNullOrEmpty(input))
          {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid input, try again");
            Console.ForegroundColor = init;
            input = Console.ReadLine();
          }
          if(string.IsNullOrEmpty(input))
          {
            Console.WriteLine("Download option cancelled, back to main menu");
          }
          else
          {
            var parts = input.Split(" ");
            result = reviewer.Download(parts[0], bool.Parse(parts[1])).Result;
            if(result.Item1)
            {
              // now we will try to save the pack locally (in zip format)
              var path = $"{System.Environment.CurrentDirectory}/{parts[0]}.zip";
              var jsonOption = new JsonSerializerOptions()
              {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
              };
              var json = JsonSerializer.Deserialize<DownloadPack>(result.Item2, jsonOption);
              var decodedKyc = Convert.FromBase64String(json.Pack.Data);
              File.WriteAllBytes(path, decodedKyc);
              Console.WriteLine($"File saved at: {path}");
            }
            else
            {
              Console.WriteLine("Failed to download the kyc pack");
            }
          }
          Console.WriteLine("Action - END");
          break;
        case 3:
          Console.WriteLine("Action - Approve");
          Console.WriteLine("Please specify the id of the pack, and whether you want to notarise the approve or not.");
          Console.WriteLine("(Format: [ID_OF_PACK] [true/false]");
          input = Console.ReadLine();
          while (!ValidateInput(input) && !string.IsNullOrEmpty(input))
          {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid input, try again");
            Console.ForegroundColor = init;
            input = Console.ReadLine();
          }
          if (string.IsNullOrEmpty(input))
          {
            Console.WriteLine("Approve option cancelled, back to main menu");
          }
          else
          {
            var parts = input.Split(" ");
            result = reviewer.Approve(parts[0], bool.Parse(parts[1])).Result;
            if(result.Item1)
            {
              Console.WriteLine(result.Item2);
            }
            else
            {
              Console.WriteLine("Approve failed");
            }
            Console.WriteLine();
          }
          Console.WriteLine("Action - END");
          break;
        case 4:
          Console.WriteLine("Action - Delete");
          Console.WriteLine("Please specify the id of the pack to delete");
          Console.WriteLine("(Format: [ID_OF_PACK]");
          input = Console.ReadLine();
          if (string.IsNullOrEmpty(input))
          {
            Console.WriteLine("Delete option cancelled, back to main menu");
          }
          else
          {
            Console.WriteLine(reviewer.Delete(input).Result);
          }
          Console.WriteLine("Action - END");
          break;
      }
    }

    public static void Main()
    {
      Console.WriteLine("KYC API Example");
      reviewer = new KYCReviewer();
      user = new KYCUser();
      Console.WriteLine("Please input the content for the test pack, then this pack will be uploaded to the cold storage");
      var testContent = Console.ReadLine();
      var uploadResult = user.UploadKycPack(testContent).Result;
      Console.WriteLine($"Upload result: {uploadResult.Item1}, {uploadResult.Item2}");
      var loggedIn = reviewer.RequestAccessToken().Result;
      if(loggedIn)
      {
        Console.WriteLine("Reviewer logged in");
      }
      else
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Reviewer log-in failed, try again!");
        Console.ForegroundColor = init;
        return;
      }

      PrintOptions();
      var isValidInput = Int32.TryParse(Console.ReadLine(), out var option);
      while (!(isValidInput && option == 5))
      {
        if (isValidInput)
        {
          Action(option);
          PrintOptions();
          isValidInput = Int32.TryParse(Console.ReadLine(), out option);
        }
        else
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine("Invalid input (Option range: 1 - 5), try again below");
          Console.ForegroundColor = init;
          PrintOptions();
          isValidInput = Int32.TryParse(Console.ReadLine(), out option);
        }
      }
    }
  }
}
