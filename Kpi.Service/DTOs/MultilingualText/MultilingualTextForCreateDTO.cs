﻿using Kpi.Domain.Enum;


namespace Kpi.Service.DTOs.MultilingualText
{
    public class MultilingualTextForCreateDTO
    {
        public required string Key { get; set; }
        public required string TextKo { get; set; }
        public required string TextEn { get; set; }
        public required SupportLanguage SupportLanguage { get; set; }
    }
}
