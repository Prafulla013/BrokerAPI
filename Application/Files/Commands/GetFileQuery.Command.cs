using Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Files.Commands
{
        public class GetFileHandler : IRequestHandler<GetFileQuery, GetFileResponse>
        {
            private readonly IAzureFileService _azureFileService;
            public GetFileHandler(IAzureFileService azureFileService)
            {
                _azureFileService = azureFileService;
            }
            public async Task<GetFileResponse> Handle(GetFileQuery request, CancellationToken cancellationToken)
            {
                var fileBytes = await _azureFileService.GetFileAsync(request.FileUrl);
                var filename = Path.GetFileName(request.FileUrl);
                var extension = filename.Split('.')[1];
                var response = new GetFileResponse
                {
                    FileBytes = fileBytes,
                    ContentType = $"pdf/{extension}",
                    FileName = filename
                };
                return response;
            }
        }
        public class GetFileQuery : IRequest<GetFileResponse>
        {
            public string FileUrl { get; set; }
        }
        public class GetFileResponse
        {
            public byte[] FileBytes { get; set; }
            public string ContentType { get; set; }
            public string FileName { get; set; }
        }
    
}
