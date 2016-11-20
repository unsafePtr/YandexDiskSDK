using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using YandexDiskSDK.RequestModels;

namespace YandexDiskSDK.Extensions.RequestParameters
{
    public static class RequestParametersExtensions
    {
        public static KeyValuePair<string, string> FieldsParameter(this BaseRequestModel baseRequestModel, List<string> fields)
        {
            if (IsNullableObject(fields))
                return NullKeyValuePair();

            string fieldsString = String.Empty;
            for (int i = 0; i < fields.Count; i++)
            {
                if (i == fields.Count - 1)
                    fieldsString += fields[i];
                else
                    fieldsString += fields[i] + ",";
            }

            return new KeyValuePair<string, string>("fields", fieldsString);
        }

        public static KeyValuePair<string, string> PathParameter(this BaseRequestModel baseRequestModel, string path)
        {
            if (IsNullableObject(path))
                return NullKeyValuePair();

            return new KeyValuePair<string, string>("path", WebUtility.UrlEncode(path));
        }

        public static KeyValuePair<string, string> MediaTypeParameter(this BaseRequestModel baseRequestModel, List<MediaType> mediaTypesList)
        {
            if (IsNullableObject(mediaTypesList))
                return NullKeyValuePair();

            string mediaTypes = String.Empty;
            for (int i = 0; i < mediaTypesList.Count; i++)
            {
                if (i == mediaTypesList.Count - 1)
                    mediaTypes += mediaTypesList[i].ToString().ToLower();
                else
                    mediaTypes += mediaTypesList[i].ToString().ToLower() + ",";
            }

            return new KeyValuePair<string, string>("media_type", mediaTypes);
        }

        public static KeyValuePair<string, string> SortParameter(this BaseRequestModel baseRequestModel, SortBy? sortBy, SortDirection direction)
        {
            if (IsNullableObject(sortBy))
                return NullKeyValuePair();

            string sort = sortBy.ToString();

            if (direction == SortDirection.Descending)
                sort = "-" + sort;

            return new KeyValuePair<string, string>("sort", sort);
        }

        private static KeyValuePair<string, string> NullKeyValuePair()
        {
            return new KeyValuePair<string, string>(null, null);
        }

        private static bool IsNullableObject(object obj)
        {
            return obj == null;
        }
    }
}
