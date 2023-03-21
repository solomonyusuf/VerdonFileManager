using System;
using System.IO;
using System.Net.Http.Headers;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerdonFileManager.Data;
using VerdonFileManager.Models;

namespace VerdonFileManager.Services
{
    [ApiController]
    [SecurityPermission(SecurityAction.Demand, ControlThread = true)]
    public class UploadService : ControllerBase
    {
        public readonly ApplicationDbContext _db;
        public readonly IConfiguration _config;
        public UploadService(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }



        [Route("Blocked")]
        [HttpPost, DisableRequestSizeLimit]
        [SecurityPermission(SecurityAction.Demand, ControlThread = true)]
        public async Task<string> ProfileUpload(IBrowserFile seed)
        {
            try
            {
                    var file = seed.OpenReadStream(250000000);
                    var folderName = Path.Combine("wwwroot", "UserImages");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    FileIOPermission permission = new FileIOPermission(FileIOPermissionAccess.AllAccess, pathToSave);
                    permission.Demand();
                    if (file.Length > 0)
                    {
                        var fileName = seed.Name;
                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName);

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        return dbPath;
                    }
                    else
                    {
                        return null;
                    }

            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return null;
            }
        }


        [Route("Blocked")]
        [HttpPost, DisableRequestSizeLimit]
        [SecurityPermission(SecurityAction.Demand, ControlThread = true)]
        public async Task<string> InternalUpload(IBrowserFile seed,string appToken)
        {
            try
            {
                if (appToken != null && await _db.Folder.AnyAsync(x => x.UploadToken.Equals(appToken)))
                {
                    var file = seed.OpenReadStream(250000000);
                    var folderName = Path.Combine("wwwroot", "StaticFiles", appToken);
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    FileIOPermission permission = new FileIOPermission(FileIOPermissionAccess.AllAccess, pathToSave);
                    permission.Demand();
                    if (file.Length > 0)
                    {
                        var fileName = seed.Name;
                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName);

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        return dbPath;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return null;
            }
        }

        [AllowAnonymous]
        [Route("verdon.filemanager.api/{appToken}")]
        [HttpPost, DisableRequestSizeLimit]
        [SecurityPermission(SecurityAction.Demand, ControlThread = true)]
        public async Task<IActionResult> ExternalUpload([FromRoute]string appToken, [FromForm]IFormFile _file)
        {
            try
            {
                if (appToken != null && await _db.Folder.AnyAsync(x => x.UploadToken.Equals(appToken)))
                {
                    var folder = await _db.Folder.Where(x => x.UploadToken.Equals(appToken)).SingleAsync();
                    var file = _file;
                    var folderName = Path.Combine("wwwroot", "StaticFiles", appToken);
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                   
                    if (file.Length > 0)
                    {
                        var fileName = file.FileName.Replace(" ", "_");
                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName);

                        var fileData = new FileData()
                        {
                            FolderId = folder.FolderId,
                            UserId = folder.UserId,
                            Name = fileName,
                            Path = dbPath,
                            Extension = file.ContentType,
                            UploadToken = appToken,
                            Size = file.Length
                         };

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                            await _db.FileData.AddAsync(fileData);
                            await _db.SaveChangesAsync();
                        }
  
                        var host = Request.Host;
                        var link = $"https://{host}/wwwroot/StaticFiles/{appToken}/{fileName}";

                        return Ok(new { link });
                    }
                    else
                    {
                        return BadRequest();
                    }

                }
                else
                {
                    return NotFound();
                }
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }



        [AllowAnonymous]
        [Route("verdon.filemanager.api/delete/{appToken}/{fileId}")]
        [HttpPost, DisableRequestSizeLimit]
        [SecurityPermission(SecurityAction.Demand, ControlThread = true)]
        public async Task<IActionResult> DeleteUpload([FromRoute] string appToken, [FromRoute] Guid fileId)
        {
            try
            {
                if (appToken != null && await _db.Folder.AnyAsync(x => x.UploadToken.Equals(appToken)))
                {

                   
                    var file = await _db.FileData.FindAsync(fileId);
                    var folderName = Path.Combine("wwwroot", "StaticFiles", appToken);
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                        var fileName = file.Name;
                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName);

                        using (var stream = new FileStream(fullPath, FileMode.Truncate))
                        {
                          
                             _db.FileData.Remove(file);
                            await _db.SaveChangesAsync();
                        }

                       
                        var link = $"file deleted sucessfully";

                        return Ok(new { link });
                 

                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }


        [Route("Blocked")]
        [SecurityPermission(SecurityAction.Demand, ControlThread = true)]
        public string CreateFolder(string appToken)
        {
            try
            {
                var folderName = Path.Combine("wwwroot", "StaticFiles", appToken);
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var dir = Directory.CreateDirectory(pathToSave);
                DirectorySecurity dirSec = dir.GetAccessControl();
                dirSec.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.Write, AccessControlType.Allow));
                dirSec.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.ReadAndExecute, AccessControlType.Allow));
                dirSec.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.CreateFiles, AccessControlType.Allow));
                dir.SetAccessControl(dirSec);

                return pathToSave;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "NULL";
            }
        }



    }

 }


       