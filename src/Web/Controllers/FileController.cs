using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using AutoMapper;
using FileStorage.Contract.Commands;
using Infrastructure.Constants;
using Infrastructure.Extensions;
using Infrastructure.Messaging;
using MemberCore.Contract.Interfaces;
using Web.Models;
using YearlyPlanning.Contract.Commands.JobAssignCommands;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.ReadModel;

namespace Web.Controllers
{
    [RoutePrefix("api/files")]
    public class FileController : ApiController
    {
        private readonly IMessageBus messageBus;
        private readonly IJobAssignProvider jobAssignProvider;
        private readonly IJobService jobService;
        private readonly IMemberService memberService;

        public FileController(IMessageBus messageBus, IJobAssignProvider jobAssignProvider, IJobService jobService, IMemberService memberService)
        {
            this.messageBus = messageBus;
            this.jobAssignProvider = jobAssignProvider;
            this.jobService = jobService;
            this.memberService = memberService;
        }

        [HttpPost, Route("upload")]
        public async Task<HttpResponseMessage> Upload()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return new HttpResponseMessage(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);
            var contents = provider.Contents;
            var fileContent = contents.FirstOrDefault(x => x.Headers.ContentDisposition.FileName.IsNotNullOrEmpty());
            var jobId = await GetParameter(contents, "jobId");
            var departmentId = (await GetParameter(contents, "departmentId")).ToNullableGuid();
            var isOpertionalTask = await GetParameter(contents, "isOpertionalTask");
            var assignId = (await GetParameter(contents, "assignId")).ToNullableGuid();
            var isLocalChangedStr = await GetParameter(contents, "isLocalChanged");
            var uploaderId = Guid.Parse(await GetParameter(contents, "uploaderId"));

            if (fileContent == null || jobId.IsNullOrEmpty())
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            bool isLocalChanged = string.IsNullOrEmpty(isOpertionalTask) && bool.Parse(isLocalChangedStr);
            Guid jobAssignId = await CreateOrGetJobAssign(assignId, departmentId, jobId, isLocalChanged);

            var fileId = Guid.NewGuid();
            var buffer = await fileContent.ReadAsByteArrayAsync();
            var originalName = fileContent.Headers.ContentDisposition.FileName.Trim('"');
            var absolutePathPart = HostingEnvironment.MapPath(Constants.Upload.UploadPath);

            await messageBus.Publish(new UploadForTaskInDepartment(fileId, buffer, originalName, absolutePathPart, jobAssignId, uploaderId));

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpPost, Route("uploadFileWhenChangeJobStatus")]
        public async Task<HttpResponseMessage> UploadFileWhenChangeJobStatus()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return new HttpResponseMessage(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);
            var contents = provider.Contents;
            var fileContent = contents.FirstOrDefault(x => x.Headers.ContentDisposition.FileName.IsNotNullOrEmpty());

            if (fileContent == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            var fileId = Guid.NewGuid();
            var buffer = await fileContent.ReadAsByteArrayAsync();
            var originalName = fileContent.Headers.ContentDisposition.FileName.Trim('"');
            var absolutePathPart = HostingEnvironment.MapPath(Constants.Upload.DayAssignUploadPath);

            var dayAssignId = (await GetParameter(contents, "dayAssignId")).ToGuid();

            IMemberModel currentUser = memberService.GetCurrentUser();
            await messageBus.Publish(new DayAssignUploadFileCommand(fileId, buffer, originalName, absolutePathPart, dayAssignId, currentUser.MemberId));

            return Request.CreateResponse(HttpStatusCode.OK, new { fileId });
        }

        [HttpPost, Route("updateUploadList")]
        public async Task UpdateUploadList(UpdatedUploadListModel model)
        {
            foreach (var file in model.ChangedDescriptionFileList)
            {
                await messageBus.Publish(new ChangeDescription(file.FileId, file.Description));
            }

            foreach (var id in model.MarkedForDeletionFileIdList)
            {
                await messageBus.Publish(new Delete(id));
            }
        }

        [HttpPost, Route("deleteUploadedFile")]
        public async Task DeleteUploadedFile(Guid fileId)
        {
            await messageBus.Publish(new DeleteDataInDayAssign(fileId));
        }


        [HttpPost, Route("delete")]
        public Task Delete(Guid fileId)
        {
            return messageBus.Publish(new Delete(fileId));
        }

        [HttpGet, Route("download")]
        public HttpResponseMessage Download(string fileName, string originalFileName)
        {
            var result = Request.CreateResponse(HttpStatusCode.NotFound);
            var filePath = HostingEnvironment.MapPath("~" + fileName);
            if (!File.Exists(filePath))
            {
                return result;
            }

            result = Request.CreateResponse(HttpStatusCode.OK);
            result.Content = new StreamContent(new FileStream(filePath, FileMode.Open, FileAccess.Read));
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(fileName));
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = originalFileName
            };

            return result;
        }

        private async Task<Guid> CreateOrGetJobAssign(Guid? assignId, Guid? departmentId, string jobId, bool isLocalChanged)
        {
            var jobAssign = assignId.HasValue ? jobService.GetJobAssignById(assignId.Value) : null;

            if (jobAssign == null || jobAssign.IsGlobal && isLocalChanged)
            {
                if (!departmentId.HasValue)
                {
                    throw new ArgumentException();
                }

                var newAssignId = Guid.NewGuid();

                var globalAssign = jobAssignProvider.GetGlobalAssignByJobId(jobId);
                var command = Mapper.Map(globalAssign, new CreateJobAssignFromJobAssignCommand(newAssignId, new List<Guid> { departmentId.Value }));
                command.JobIdList = new List<string> { jobId };
                await messageBus.Publish(command);

                await messageBus.Publish(new ChangeJobIdAndJobAssignIdInDayAssignCommand(jobId, departmentId.Value, newAssignId));

                return newAssignId;
            }

            return jobAssign.Id;
        }

        private static async Task<string> GetParameter(IEnumerable<HttpContent> contents, string parameter)
        {
            var content = contents.FirstOrDefault(x => string.Equals(x.Headers.ContentDisposition.Name, $"\"{parameter}\"", StringComparison.InvariantCultureIgnoreCase));
            if (content == null)
            {
                return null;
            }

            var result = await content.ReadAsStringAsync();
            if (string.Equals(result, "undefined", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            return result;
        }
    }
}