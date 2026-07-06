using Catalog.Products.Dtos;
using Catalog.Products.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Products.Mappings
{
    public static class ProductMapping
    {
        public static ProductDto ToDto(this Product product) =>
            new(product.Id, product.Name, product.Category, product.Description, product.ImageFile, product.Price);
    }
}
