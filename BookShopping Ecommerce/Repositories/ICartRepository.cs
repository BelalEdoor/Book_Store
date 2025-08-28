namespace BookShopping_Ecommerce.Repositories
{
    public interface ICartRepository
    {
        Task<int> AddItem(int bookId, int qty);                 // ➕ إضافة أو زيادة
        Task<int> RemoveItem(int bookId, int qty = 1);          // ➖ إنقاص كمية (ولو صفر ينحذف)
        Task<int> DeleteItem(int bookId);                       // ❌ حذف المنتج بالكامل
        Task<ShoppingCart> GetUserCart();                       // 🛒 كارت المستخدم الحالي
        Task<int> GetCartItemCount(string userId = "");         // 🔢 عدد المنتجات
        Task<ShoppingCart> GetCart(string userId);              // 🛒 كارت حسب userId
        Task<bool> DoCheckout();
    }
}
