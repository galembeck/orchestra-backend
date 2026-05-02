using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Utils;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace API.Public.DTOs;

public class RegisterCompanyDTO
{
    // Owner
    public string OwnerName { get; set; } = string.Empty;
    public string OwnerEmail { get; set; } = string.Empty;
    public string OwnerCellphone { get; set; } = string.Empty;
    public string OwnerPassword { get; set; } = string.Empty;
    public bool AcceptTerms { get; set; }

    public string OwnerZipcode { get; set; } = string.Empty;
    public string OwnerAddress { get; set; } = string.Empty;
    public string OwnerNumber { get; set; } = string.Empty;
    public string? OwnerComplement { get; set; }
    public string OwnerNeighborhood { get; set; } = string.Empty;
    public string OwnerCity { get; set; } = string.Empty;
    public string OwnerState { get; set; } = string.Empty;

    // Company
    public Segment Segment { get; set; }
    public string Cnpj { get; set; } = string.Empty;
    public string SocialReason { get; set; } = string.Empty;
    public string FantasyName { get; set; } = string.Empty;

    public string Zipcode { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string? Complement { get; set; }
    public string Neighborhood { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;

    // Documents (multipart files)
    public IFormFile? CnpjDocument { get; set; }
    public IFormFile? AddressProof { get; set; }
    public IFormFile? OwnerIdentity { get; set; }
    public IFormFile? OperatingLicense { get; set; }

    public User ToOwnerModel() => new()
    {
        Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(OwnerName.ToLower().Trim()),
        Email = OwnerEmail.ToLower().Trim(),
        Cellphone = OwnerCellphone,
        Password = OwnerPassword.Trim(),
        ProfileType = ProfileType.CLIENT,
        AccountType = AccountType.COMPANY,
        AcceptedTerms = AcceptTerms,
        Zipcode = OwnerZipcode,
        Address = OwnerAddress,
        Number = OwnerNumber,
        Complement = OwnerComplement,
        Neighborhood = OwnerNeighborhood,
        City = OwnerCity,
        State = OwnerState,
    };

    public Company ToCompanyModel() => new()
    {
        Segment = Segment,
        Cnpj = StringUtil.Slugify(Cnpj.Trim()),
        SocialReason = SocialReason.Trim(),
        FantasyName = FantasyName.Trim(),
        Zipcode = Zipcode,
        Address = Address,
        Number = Number,
        Complement = Complement,
        Neighborhood = Neighborhood,
        City = City,
        State = State,
        ApprovalStatus = CompanyApprovalStatus.PENDING,
    };

    public IDictionary<CompanyDocumentType, IFormFile> ToDocumentsMap()
    {
        var map = new Dictionary<CompanyDocumentType, IFormFile>();
        if (CnpjDocument is not null) map[CompanyDocumentType.CNPJ_DOCUMENT] = CnpjDocument;
        if (AddressProof is not null) map[CompanyDocumentType.ADDRESS_PROOF] = AddressProof;
        if (OwnerIdentity is not null) map[CompanyDocumentType.OWNER_IDENTITY] = OwnerIdentity;
        if (OperatingLicense is not null) map[CompanyDocumentType.OPERATING_LICENSE] = OperatingLicense;
        return map;
    }
}
