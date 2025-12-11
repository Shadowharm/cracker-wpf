using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Cracker
{
    public class LogValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";

            string jsonValue = value.ToString();
            if (string.IsNullOrWhiteSpace(jsonValue)) return "";
            
            string trimmed = jsonValue.Trim();
            if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
            {
                try
                {
                    StringBuilder result = new StringBuilder();
                    jsonValue = trimmed.Substring(1, trimmed.Length - 2).Trim();
                    
                    if (string.IsNullOrWhiteSpace(jsonValue))
                        return "";
                    
                    var objects = SplitJsonObjects(jsonValue);
                    
                    foreach (var obj in objects)
                    {
                        string objClean = obj.Trim().TrimStart('{').TrimEnd('}');
                        string field = ExtractJsonValue(objClean, "field");
                        string oldVal = ExtractJsonValue(objClean, "old");
                        string newVal = ExtractJsonValue(objClean, "new");
                        
                        if (!string.IsNullOrEmpty(field))
                        {
                            result.AppendLine($"{field}: {oldVal} → {newVal}");
                        }
                    }
                    
                    return result.ToString().TrimEnd();
                }
                catch (Exception ex)
                {
                    return jsonValue;
                }
            }
            
            return jsonValue;
        }

        private List<string> SplitJsonObjects(string json)
        {
            var result = new List<string>();
            int depth = 0;
            int start = 0;
            
            for (int i = 0; i < json.Length; i++)
            {
                if (json[i] == '{') depth++;
                else if (json[i] == '}') depth--;
                
                if (depth == 0 && (json[i] == '}' || json[i] == ','))
                {
                    if (i > start)
                    {
                        string obj = json.Substring(start, i - start).Trim();
                        if (obj.StartsWith("{")) obj = obj.Substring(1);
                        if (obj.EndsWith("}")) obj = obj.Substring(0, obj.Length - 1);
                        if (!string.IsNullOrWhiteSpace(obj))
                            result.Add("{" + obj + "}");
                    }
                    start = i + 1;
                }
            }
            
            if (start < json.Length)
            {
                string obj = json.Substring(start).Trim();
                if (obj.StartsWith("{")) obj = obj.Substring(1);
                if (obj.EndsWith("}")) obj = obj.Substring(0, obj.Length - 1);
                if (!string.IsNullOrWhiteSpace(obj))
                    result.Add("{" + obj + "}");
            }
            
            return result;
        }

        private string ExtractJsonValue(string json, string key)
        {
            string searchKey = $"\"{key}\":\"";
            int startIndex = json.IndexOf(searchKey);
            if (startIndex == -1) return "";
            
            startIndex += searchKey.Length;
            int endIndex = json.IndexOf("\"", startIndex);
            if (endIndex == -1) return "";
            
            string value = json.Substring(startIndex, endIndex - startIndex);
            value = value.Replace("\\\"", "\"")
                        .Replace("\\\\", "\\")
                        .Replace("\\n", "\n")
                        .Replace("\\r", "\r")
                        .Replace("\\t", "\t");
            
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

