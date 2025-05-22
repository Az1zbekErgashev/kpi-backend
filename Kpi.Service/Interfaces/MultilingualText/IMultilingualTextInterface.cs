using Kpi.Domain.Enum;
using Kpi.Domain.Models.MultilingualText;
using Kpi.Domain.Models.PagedResult;
using Kpi.Service.DTOs.MultilingualText;
using Microsoft.AspNetCore.Http;


namespace Kpi.Service.Interfaces.MultilingualText
{
    public interface IMultilingualTextInterface
    {
        ValueTask<bool> CreateFromJson(IFormFile formFile, SupportLanguage language);
        ValueTask<Dictionary<string, string>> GetDictonary(SupportLanguage language);
        ValueTask<PagedResult<UIContentExtendedModel>> GetTranslations(UIContentGetAllAndSearchDTO dto);
        ValueTask<bool> UpdateAsync(MultilingualTextForCreateDTO dto);
        ValueTask<bool> CreateAsync(MultilingualTextForCreateDTO dto);
        ValueTask<string> DeleteOrRecoverAsync(string key);
        ValueTask<bool> UpdateAllTranslationReplaceSpace();
    }
}
