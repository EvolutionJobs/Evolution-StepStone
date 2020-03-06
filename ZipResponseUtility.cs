namespace EvoApi.Services.StepStone
{
    using EvoApi.Services.StepStone.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>Utility methods for parsing HTTP downloaded Zip files.</summary>
    static class ZipResponseUtility
    {
        /// <summary>Regular expression to parse the filename format used in CV Zip archives.
        /// <para>StepStone docs say this will be "{candidate_forename} {candidate_surname} ({candidate_id}).{extension}".
        /// However, in practice it appears to be "{candidate_forename} {candidate_surname} ({candidate_id} - BRAND).{extension}".
        /// To work around this the regex looks for the number in the () and ignores the content around it. </para>
        /// This looks for 3 groups: "name", "id" and "ext".</summary>
        static readonly Regex cvFilename = new Regex(@"(?<name>.*)\((?<id>\d*).*\)\.(?<ext>.*$)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ECMAScript);

        /// <summary>Parse the Zip filename to get the ID, candidate name and file extension.</summary>
        /// <param name="file">The file name to parse.</param>
        /// <returns>Tuple of the name, ext and id.</returns>
        static (string name, string ext, long id) ParseZipFileName(string file)
        {
            string name = null, ext = null;
            long id = 0;

            var fileMatches = cvFilename.Matches(file).FirstOrDefault();
            if (fileMatches?.Success ?? false)
            {
                var nameMatch = fileMatches.Groups["name"];
                if (nameMatch.Success)
                    name = nameMatch.Value.Trim();

                var idMatch = fileMatches.Groups["id"];
                if (idMatch.Success &&
                    long.TryParse(idMatch.Value.Trim(), out long r))
                    id = r;

                var extMatch = fileMatches.Groups["ext"];
                if (extMatch.Success)
                    ext = extMatch.Value.Trim();
            }

            return (name, ext, id);
        }

        /// <summary>Parse a response of a Zip file holding candidate CVs.</summary>
        /// <param name="response">The response to parse.</param>
        /// <returns>Task that resolves with the buffered enumeration of files.</returns>
        public async static Task<IEnumerable<StepStoneCV>> ParseCV(HttpResponseMessage response)
        {
            // We can async parse the Zip files, but any open streams will be closed once the response message is disposed.
            // All the file streams are buffered to arrays, and all the arrays read before this method returns.
            var result = new List<StepStoneCV>();
            await foreach (var cv in ParseCVs(response))
                result.Add(cv);

            return result;
        }

        async static IAsyncEnumerable<StepStoneCV> ParseCVs(HttpResponseMessage response)
        {
            // Open the content as a stream, this will be closed when the response is disposed.
            using var strm = await response.Content.ReadAsStreamAsync();

            // Open the archive and read the files from it
            using var archive = new ZipArchive(strm, ZipArchiveMode.Read);
            foreach (var entry in archive.Entries)
            {
                // Parse the filename "{candidate_forename} {candidate_surname} ({candidate_id}).{extension}"
                string filename = entry.FullName;
                var (name, ext, id) = ParseZipFileName(filename);

                // Copy the entry to memory to get the bytes, the stream will be closed so we need to copy the bytes
                using var memoryStream = new MemoryStream();
                using var entryStream = entry.Open();
                await entryStream.CopyToAsync(memoryStream);
                byte[] fileBytes = memoryStream.ToArray();

                yield return new StepStoneCV
                {
                    Name = name,
                    Extension = ext,
                    ID = id,
                    Filename = filename,
                    File = fileBytes,
                    Mime = MimeForExtension(ext)
                };
            }
        }

        /// <summary>Get the MIME type for the files expected as CV formats.</summary>
        /// <param name="ext">The extension of the file.</param>
        /// <returns>The MIME type to use for the file.</returns>
        static string MimeForExtension(string ext) {
            if (string.Equals("doc", ext, StringComparison.OrdinalIgnoreCase))
                return "application/msword";

            if (string.Equals("docx", ext, StringComparison.OrdinalIgnoreCase))
                return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

            if (string.Equals("pdf", ext, StringComparison.OrdinalIgnoreCase))
                return "application/pdf";

            if (string.Equals("txt", ext, StringComparison.OrdinalIgnoreCase))
                return "text/plain; charset=UTF-8"; // Probably not ISO-8859-1

            if (string.Equals("rtf", ext, StringComparison.OrdinalIgnoreCase))
                return "application/rtf";

            if (string.Equals("md", ext, StringComparison.OrdinalIgnoreCase))
                return "text/markdown; charset=UTF-8"; // Probably not ISO-8859-1

            // Other file types can be held in the Zip but aren't expected as CVs, return them as blob.
            return "application/octet-stream";
        }
    }
}
