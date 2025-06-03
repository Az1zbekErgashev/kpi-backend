using Dapper;
using Kpi.Domain.Enum;
using Kpi.Domain.Models.MultilingualText;
using Kpi.Domain.Models.PagedResult;
using Kpi.Service.DTOs.MultilingualText;
using Kpi.Service.Exception;
using Kpi.Service.Interfaces.IRepositories;
using Kpi.Service.Interfaces.MultilingualText;
using Kpi.Service.StringExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Linq.Expressions;
using System.Text.Json;


namespace Kpi.Service.Service.MultilingualText
{
    public class MultilingualTextService : IMultilingualTextInterface
    {
        private readonly IGenericRepository<Domain.Entities.MultilingualText.MultilingualText> multilingualRepository;
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MultilingualTextService(
            IGenericRepository<Domain.Entities.MultilingualText.MultilingualText> multilingualRepository,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            this.multilingualRepository = multilingualRepository;
            this.configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async ValueTask<bool> CreateFromJson(IFormFile formFile, SupportLanguage language)
        {
            if (formFile == null || formFile.Length == 0)
                return false;

            try
            {
                using var stream = formFile.OpenReadStream();
                using var reader = new StreamReader(stream);
                string jsonContent = await reader.ReadToEndAsync();

                var data = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);

                if (data == null)
                    return false;

                foreach (var item in data)
                {
                    var existText = await multilingualRepository.GetAsync(x => (x.Key != null && x.SupportLanguage != null) && x.Key.ToLower() == item.Key.ToLower() && x.SupportLanguage == language);

                    if (existText is null)
                    {
                        if (language == SupportLanguage.Ko)
                        {
                            var newText = new Domain.Entities.MultilingualText.MultilingualText
                            {
                                Key = item.Key,
                                Text = item.Value,
                                SupportLanguage = language,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };

                            await multilingualRepository.CreateAsync(newText);
                        }

                        if (language == SupportLanguage.En)
                        {
                            var newText = new Domain.Entities.MultilingualText.MultilingualText
                            {
                                Key = item.Key,
                                Text = item.Value,
                                SupportLanguage = language,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };

                            await multilingualRepository.CreateAsync(newText);
                        }

                    }
                    else
                    {
                        if (language == SupportLanguage.Ko)
                        {
                            existText.Text = item.Value;
                            existText.UpdatedAt = DateTime.UtcNow;
                        }

                        if (language == SupportLanguage.En)
                        {
                            existText.Text = item.Value;
                            existText.UpdatedAt = DateTime.UtcNow;
                        }

                        multilingualRepository.UpdateAsync(existText);
                    }
                }

                await multilingualRepository.SaveChangesAsync();
                return true;
            }
            catch
            {
                Console.WriteLine($"Ошибка при обработке JSON");
                return false;
            }
        }

        public async ValueTask<Dictionary<string, string>> GetDictonary(SupportLanguage language)
        {
            var connectionString = configuration.GetConnectionString("PostgresConnection");

            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            string query = "SELECT \"Key\", \"Text\" FROM \"MultilingualText\" WHERE \"SupportLanguage\" = @Language AND \"IsDeleted\" = 0;";

            var result = (await connection.QueryAsync<Domain.Entities.MultilingualText.MultilingualText>(query, new { Language = language })).ToList();

            return language == SupportLanguage.Ko
                ? result.ToDictionary(x => x.Key, x => x.Text) :
                language == SupportLanguage.En
                ? result.ToDictionary(x => x.Key, x => x.Text)
                : language == SupportLanguage.Uz
                ? result.ToDictionary(x => x.Key, x => x.Text)
                : result.ToDictionary(x => x.Key, x => x.Text);
        }

        public async ValueTask<bool> CreateAsync(MultilingualTextForCreateDTO dto)
        {
            var existText = await multilingualRepository.GetAsync(x => x.Key.ToLower().Replace(" ", "") == dto.Key.ToLower().Replace(" ", ""));

            if (existText is not null) throw new KpiException(400, "key_already_exist");

            var newTextKo = new Domain.Entities.MultilingualText.MultilingualText
            {
                Key = dto.Key.Replace(" ", ""),
                Text = dto.TextKo,
                SupportLanguage = SupportLanguage.Ko,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var newTextEn = new Domain.Entities.MultilingualText.MultilingualText
            {
                Key = dto.Key.Replace(" ", ""),
                Text = dto.TextEn,
                SupportLanguage = SupportLanguage.En,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var newTextUz = new Domain.Entities.MultilingualText.MultilingualText
            {
                Key = dto.Key.Replace(" ", ""),
                Text = dto.TextUz,
                SupportLanguage = SupportLanguage.Uz,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var newTextRu = new Domain.Entities.MultilingualText.MultilingualText
            {
                Key = dto.Key.Replace(" ", ""),
                Text = dto.TextRu,
                SupportLanguage = SupportLanguage.Ru,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await multilingualRepository.CreateAsync(newTextKo);
            await multilingualRepository.CreateAsync(newTextEn);
            await multilingualRepository.CreateAsync(newTextRu);
            await multilingualRepository.CreateAsync(newTextUz);


            await multilingualRepository.SaveChangesAsync();
            return true;
        }

        public async ValueTask<string> DeleteOrRecoverAsync(string key)
        {
            var existText = await multilingualRepository.GetAll(x => x.Key.ToLower() == key.ToLower()).AsNoTracking().ToListAsync();

            bool isDelete = false;

            foreach (var item in existText)
            {
                if (item.IsDeleted == 0)
                {
                    item.IsDeleted = 1;
                    multilingualRepository.UpdateAsync(item);
                    isDelete = true;
                }
                else
                {
                    item.IsDeleted = 0;
                    multilingualRepository.UpdateAsync(item);
                }
            }

            await multilingualRepository.SaveChangesAsync();
            if (isDelete) return "deleted";
            else return "recovered";
        }

        public async ValueTask<bool> UpdateAsync(MultilingualTextForCreateDTO dto)
        {
            var existText = await multilingualRepository.GetAll(x => x.Key.ToLower().Replace(" ", "") == dto.Key.ToLower().Replace(" ", "")).ToListAsync();

            if (existText.Count == 0) throw new KpiException(400, "translation_not_found");

            foreach (var item in existText)
            {
                if (item.SupportLanguage == SupportLanguage.Ko)
                {
                    item.Text = dto.TextKo;
                    multilingualRepository.UpdateAsync(item);
                }
                else if (item.SupportLanguage == SupportLanguage.En)
                {
                    item.Text = dto.TextEn;
                    multilingualRepository.UpdateAsync(item);
                }
                else if (item.SupportLanguage == SupportLanguage.Ru)
                {
                    item.Text = dto.TextRu;
                    multilingualRepository.UpdateAsync(item);
                }
                else if (item.SupportLanguage == SupportLanguage.Uz)
                {
                    item.Text = dto.TextUz;
                    multilingualRepository.UpdateAsync(item);
                }
            }

            await multilingualRepository.SaveChangesAsync();
            return true;
        }

        public async ValueTask<PagedResult<UIContentExtendedModel>> GetTranslations(UIContentGetAllAndSearchDTO dto)
        {
            if (dto.PageIndex == 0)
            {
                dto.PageIndex = 1;
            }

            if (dto.PageSize == 0)
            {
                dto.PageSize = 20;
            }

            dto.Key = string.IsNullOrEmpty(dto.Key) ? "" : dto.Key;
            Expression<Func<Domain.Entities.MultilingualText.MultilingualText, bool>> frontendTextExpression = dto.IsDeleted.HasValue
                 ? f => !string.IsNullOrWhiteSpace(f.Key) && f.IsDeleted == (dto.IsDeleted.Value ? 1 : 0)
                 : f => !string.IsNullOrWhiteSpace(f.Key);

            var query = multilingualRepository.GetAll(frontendTextExpression)
                .Join(multilingualRepository.GetAll(),
                    x => x.Key,
                    y => y.Key,
                    (x, y) => new
                    {
                        x.Key,
                        y.Text,
                        y.SupportLanguage,
                        x.IsDeleted
                    })
                .OrderBy(x => x.Key)
                .AsNoTracking()
                .AsEnumerable()
                .GroupBy(x => new { x.Key, x.IsDeleted })
               .Where(x =>
                (!string.IsNullOrEmpty(x.Key.Key) &&
                 !string.IsNullOrWhiteSpace(x.Key.Key) &&
                 x.Key.Key.Contains(dto.Key))
                ||
                x.Any(y => (y.Text?.ToLower() ?? "").Contains(dto.Key?.ToLower()))
            );



            int totalCount = query.Count();
            int itemsPerPage = dto.PageSize;
            int totalPages = (totalCount / itemsPerPage) + (totalCount % itemsPerPage == 0 ? 0 : 1);

            if (dto.PageIndex > totalPages)
            {
                dto.PageIndex = totalPages;
            }


            var enumerableQuery = query.ToPagedList(dto);

            var uiContents = enumerableQuery
                .Select(group =>
                {
                    var translation = new UIContentExtendedModel
                    {
                        Key = group.Key.Key,
                        IsDeleted = group.Key.IsDeleted
                    };

                    foreach (var item in group)
                    {
                        switch (item.SupportLanguage)
                        {
                            case SupportLanguage.En:
                                translation.TextEn = item.Text;
                                break;
                            case SupportLanguage.Ko:
                                translation.TextKo = item.Text;
                                break;
                            case SupportLanguage.Ru:
                                translation.TextRu = item.Text;
                                break;
                            case SupportLanguage.Uz:
                                translation.TextUz = item.Text;
                                break;

                        }
                    }
                    return translation;
                })
                .ToList();


            var pagedResult = PagedResult<UIContentExtendedModel>.Create(uiContents,
                totalCount,
                itemsPerPage,
                uiContents.Count,
                dto.PageIndex,
                totalPages
                );

            return pagedResult;
        }


        public async ValueTask<bool> UpdateAllTranslationReplaceSpace()
        {
            var existText = await multilingualRepository.GetAll().ToListAsync();

            var uniqueItems = existText
                .GroupBy(x => x?.Key?.Replace(" ", ""))
                .Select(g => g.First())
                .ToList();

            foreach (var item in uniqueItems)
            {
                await multilingualRepository.DeleteAsync(item.Id);

            }


            var allText = await multilingualRepository.GetAll().ToListAsync();

            foreach (var item in allText)
            {
                item.Key = item?.Key?.Replace(" ", "");
                multilingualRepository.UpdateAsync(item);
            }

            return true;
        }
    }
}
