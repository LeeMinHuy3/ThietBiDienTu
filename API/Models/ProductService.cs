using Microsoft.EntityFrameworkCore;
using System;

namespace API.Models
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> UpdateProductQuantity(int productId, int quantityChange)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.Quantity += quantityChange; // Cập nhật số lượng sản phẩm
                await _context.SaveChangesAsync();
                return true; // Trả về true nếu thành công
            }
            return false; // Trả về false nếu không tìm thấy sản phẩm
        }
    }
}
