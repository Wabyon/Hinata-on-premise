using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Bloqs.Http.Net;
using Hinata.Web.Mvc;

namespace Hinata.Controllers
{
    [Authorize]
    [RoutePrefix("images")]
    public class ImageController : WindowsAuthenticationContoller
    {
        private readonly string[] _permittedExtensions = {"png", "gif", "jpg", "jpeg", "bmp"};
        private const int MaxLength = 2097152; // 2MB

        [HttpPost]
        [Route("upload")]
        public async Task<ActionResult> Upload()
        {
            var file = Request.Files[0];

            if (file == null || file.ContentLength < 0) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (file.ContentLength > MaxLength) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (string.IsNullOrWhiteSpace(file.FileName)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            ext = ext.Replace(".", "").ToLower();
            if (_permittedExtensions.All(x => x != ext)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var data = new byte[file.ContentLength];
            await file.InputStream.ReadAsync(data, 0, file.ContentLength);

            var account = new Account(new Credentials(GlobalSettings.BloqsAccountName, GlobalSettings.BloqsAccessKey), GlobalSettings.BloqsBaseAddress);
            var client = account.CreateClient();
            var container = await client.GetContainerReferenceAsync(LogonUser.Name);
            container.IsPublic = true;
            await container.CreateIfNotExistsAsync();

            var fileInfo = new FileInfo(file.FileName);
            var originalName = fileInfo.Name;
            var extension = fileInfo.Extension;

            var uniquFileName = string.Format("{0}{1}",Guid.NewGuid().ToString("N"), extension);
            var blob = await container.GetBlobReferenceAsync(uniquFileName);

            blob.Properties.CacheControl = "max-age=2592000";
            blob.Properties.ContentDisposition = "inline";
            blob.Properties.ContentType = file.ContentType;

            blob.Metadata["OriginalName"] = originalName;
            blob.Metadata["LogonName"] = LogonUser.LogonName;

            await blob.UploadFromByteArrayAsync(data, 0, data.Length);

            var baseAddress = GlobalSettings.BloqsBaseAddress.EndsWith("/")
                ? GlobalSettings.BloqsBaseAddress
                : GlobalSettings.BloqsBaseAddress + "/";
            var imageUrl = string.Format("{0}{1}/{2}/{3}", baseAddress, GlobalSettings.BloqsAccountName, container.Name, uniquFileName);
            return Json(new { OriginalFileName = originalName, Url = imageUrl });
        }

        [Obsolete]
        [HttpGet]
        [Route("{authorName}/{name}")]
        public async Task<ActionResult> Get(string authorName, string name)
        {
            var account = new Account(new Credentials(GlobalSettings.BloqsAccountName, GlobalSettings.BloqsAccessKey), GlobalSettings.BloqsBaseAddress);
            var client = account.CreateClient();
            var container = await client.GetContainerReferenceAsync(authorName);

            try
            {
                var blob = await container.GetBlobReferenceAsync(name);
                using (var ms = new MemoryStream())
                {
                    await blob.DownloadToStreamAsync(ms);
                    var data = ms.ToArray();
                    return File(data, blob.Properties.ContentType);
                }
            }
            catch
            {
                return HttpNotFound();
            }
        }
    }
}
