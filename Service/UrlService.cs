using Model.FileContext;
using Model.ResponseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class UrlService
    {
        public UrlService()
        {
            
        }
        // get url file absolute paths whcih are inside of a directory NOT RECURSIVE
        public ServiceResponse<List<string>> GetUrlAbsolutePathsInsideDir(string rootFolder)
        {
            if (!Directory.Exists(rootFolder))
            {
                return ServiceResponse<List<string>>.Fail("Directory does not exist", true);
            }
            List<string> lnkAbsolutePaths = Directory.GetFiles(rootFolder, "*.url", SearchOption.TopDirectoryOnly).ToList();
            return ServiceResponse<List<string>>.Ok(lnkAbsolutePaths);
        }

        public ServiceResponse<List<string>> GetLnkAbsolutePathsUnderDesktop() => GetUrlAbsolutePathsInsideDir(Environment.ExpandEnvironmentVariables("%DESKTOP%"));


        public ServiceResponse<UrlMetaData> GetUrlMetaData(string urlAbsolutePath)
        {
            try
            {
                if (!File.Exists(urlAbsolutePath))
                {
                    return ServiceResponse<UrlMetaData>.Fail("File does not exist", true);
                }
                UrlMetaData urlMetaData = new UrlMetaData();
                string[] lines = File.ReadAllLines(urlAbsolutePath);
                foreach (string line in lines)
                {
                    if (line.StartsWith("URL=", StringComparison.OrdinalIgnoreCase))
                    {
                        string urlPart = line.Substring(4).Trim();
                        urlMetaData.Url = urlPart;
                    }
                    if (line.StartsWith("IconFile=", StringComparison.OrdinalIgnoreCase))
                    {
                        string iconFilePart = line.Substring(9).Trim();
                        urlMetaData.IconFilePath = iconFilePart;
                    }
                }

                return ServiceResponse<UrlMetaData>.Ok(urlMetaData);
            }catch(Exception ex)
            {
                return ServiceResponse<UrlMetaData>.Fail($"Exception: {ex.Message}");
            }

        }

        public ServiceResponse<string>  ChangeUrlOfUrlFile(string urlAbsolutePath, string newUrl)
        {
            try
            {
                if (!File.Exists(urlAbsolutePath))
                {
                    return ServiceResponse<string>.Fail("File does not exist", true);
                }
                string[] lines = File.ReadAllLines(urlAbsolutePath);
                bool urlFound = false;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("URL=", StringComparison.OrdinalIgnoreCase))
                    {
                        lines[i] = "URL=" + newUrl;
                        urlFound = true;
                        break;
                    }
                }
                if (!urlFound)
                {
                    return ServiceResponse<string>.Fail("URL= line not found in the file", true);
                }
                File.WriteAllLines(urlAbsolutePath, lines);
                return ServiceResponse<string>.OkMessage("URL updated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResponse<string>.Fail($"Exception: {ex.Message}");
            }
        }

        public ServiceResponse<string> ChangeIconFileOfUrlFile(string urlAbsolutePath, string newIconFilePath)
        {
            try
            {
                if (!File.Exists(urlAbsolutePath))
                {
                    return ServiceResponse<string>.Fail("File does not exist", true);
                }

                string[] lines = File.ReadAllLines(urlAbsolutePath);
                bool iconFileFound = false;

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("IconFile=", StringComparison.OrdinalIgnoreCase))
                    {
                        lines[i] = "IconFile=" + newIconFilePath;
                        iconFileFound = true;
                        break;
                    }
                }

                if (!iconFileFound)
                {
                    var list = lines.ToList();
                    list.Add("IconFile=" + newIconFilePath);
                    lines = list.ToArray();
                }

                File.WriteAllLines(urlAbsolutePath, lines);
                return ServiceResponse<string>.OkMessage("Icon file path updated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResponse<string>.Fail($"Exception: {ex.Message}");
            }
        }
    }
}
