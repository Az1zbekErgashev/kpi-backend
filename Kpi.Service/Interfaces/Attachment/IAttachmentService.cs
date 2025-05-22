using Kpi.Service.DTOs.Attachment;

namespace Kpi.Service.Interfaces.Attachment
{
    public interface IAttachmentService
    {
        ValueTask<Domain.Entities.Attachment.Attachment> UploadAsync(AttachmentForCreationDTO dto);
        ValueTask<Domain.Entities.Attachment.Attachment> UpdateAsync(int id, Stream stream);
        ValueTask<bool> ResizeImage(Domain.Entities.Attachment.Attachment? attahcment, int dimension);
    }
}
