using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.PublicApi.CatalogBrandEndpoints;
using Microsoft.eShopWeb.PublicApi.CatalogItemEndpoints;
using Microsoft.eShopWeb.PublicApi.CatalogTypeEndpoints;

namespace Microsoft.eShopWeb.PublicApi;

public static class MappingProfile
{
    public static CatalogItemDto ToDto(this CatalogItem item) => new()
    {
        Id = item.Id,
        CatalogBrandId = item.CatalogBrandId,
        CatalogTypeId = item.CatalogTypeId,
        Description = item.Description,
        Name = item.Name,
        PictureUri = item.PictureUri,
        Price = item.Price
    };

    public static CatalogTypeDto ToDto(this CatalogType item) => new()
    {
        Id = item.Id,
        Name = item.Type
    };

    public static CatalogBrandDto ToDto(this CatalogBrand item) => new()
    {
        Id = item.Id,
        Name = item.Brand
    };
}
