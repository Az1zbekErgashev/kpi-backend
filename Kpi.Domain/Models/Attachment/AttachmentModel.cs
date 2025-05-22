﻿namespace Kpi.Domain.Models.Attachment
{
    public class AttachmentModel
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }


        public virtual AttachmentModel MapFromEntity(Domain.Entities.Attachment.Attachment entity)
        {
            Id = entity.Id;
            Path = entity.Path;
            CreatedAt = entity.CreatedAt;
            UpdatedAt = entity.UpdatedAt;
            return this;
        }
    }
}
