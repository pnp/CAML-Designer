using System.Net;
using System.Text;

namespace CamlDesigner2013.Helpers
{
    public static class OnlineChecker
    {
        ///
        /// Checks the file exists or not. This is a very crude way of checking if the link is valid or not
        ///
        /// The URL of the remote file.
        /// True : If the file exits, False if file not exists
        public static bool RemoteFileExists(string url, StringBuilder sb)
        {
            bool validLink = false;
            try
            {
                sb.AppendLine(string.Format("Connect_Click Method, RemoteFileExists?, the url that is being tested:{0} ", url));
                //Creating the HttpWebRequest
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //Setting the Request method HEAD, you can also use GET too.
                request.Method = "HEAD";
                //Getting the Web Response.
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //Returns TRUE if the Status code == 200
                sb.AppendLine(string.Format("Connect_Click Method, RemoteFileExists?, the response of the statuscode is {0} , the default ok code is = {1}",response.StatusCode.ToString(),HttpStatusCode.OK.ToString()));
                validLink = (response.StatusCode == HttpStatusCode.OK);
            }
            catch (WebException ex)
            {
                sb.AppendLine(string.Format("Connect_Click Method, RemoteFileExists?, the response of the statuscode is {0} , the default ok code is = {1}, if 401 or 403 than this is ok because Auth header isn't passed yet.", ex.Message, HttpStatusCode.OK.ToString()));
                // site is live
                if (ex.Message.Contains("401"))
                {
                    //unauthorized exception, site is valid
                    validLink = true;
                }
                else if (ex.Message.Contains("403"))
                {
                    // error response from an Office 365 site
                    validLink = true;
                }
            }
            catch
            {
                //Any exception will returns false.
            }

            return validLink;

        }
    }
}
