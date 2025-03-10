using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IProductService
    {
        Task<ResponseDto?> GetProductAsync(int productId);
        Task<ResponseDto?> GetAllProductsAsync();
        Task<ResponseDto?> CreateProductAsync(ProductDto productDto);
        Task<ResponseDto?> EditProductAsync(ProductDto productDto);
        Task<ResponseDto?> DeleteProductAsync(int id);
        Task<ResponseDto?> UpdateProductsAsync(ProductDto productDto);


    }
}
