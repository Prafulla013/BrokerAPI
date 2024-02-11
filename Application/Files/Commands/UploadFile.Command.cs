using Application.Common.Interfaces;
using Common.Helpers;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Files.Commands
{
    public class UploadFileHandler : IRequestHandler<UploadFileCommand, UploadFileResponse>
    {
        private IAzureFileService _azureFileService;
        public UploadFileHandler(IAzureFileService azureFileService)
        {
            _azureFileService = azureFileService;
        }

        public async Task<UploadFileResponse> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            var extension = request.File.GetExtension();
            var bytes = await request.File.GetFileBytesAsync();
            var url = await _azureFileService.SaveFileAsync($"{Guid.NewGuid()}{extension}", bytes, "temp");

            var response = new UploadFileResponse
            {
                FileUrl = url
            };
            return response;
        }
    }

    public class UploadFileCommand : IRequest<UploadFileResponse>
    {
        public IFormFile File { get; set; }
    }

    public class UploadFileResponse
    {
        public string FileUrl { get; set; }
    }
}
