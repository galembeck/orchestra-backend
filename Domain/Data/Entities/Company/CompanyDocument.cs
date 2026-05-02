using Domain.Data.Entities._Base;
using Domain.Data.Entities._Base.Extension;
using Domain.Enumerators;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Data.Entities;

[Table("TBCompanyDocument")]
public class CompanyDocument : BaseEntity, IBaseEntity<CompanyDocument>
{
    public string CompanyId { get; set; } = string.Empty;
    public CompanyDocumentType Type { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;

    public CompanyDocument WithoutRelations(CompanyDocument entity)
    {
        if (entity == null)
            return null!;

        var newEntity = new CompanyDocument
        {
            CompanyId = entity.CompanyId,
            Type = entity.Type,
            FileName = entity.FileName,
            FilePath = entity.FilePath,
            FileUrl = entity.FileUrl,
        };

        newEntity.InitializeInstance(entity);
        return newEntity;
    }
}
