using BookShopping_Ecommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookShopping_Ecommerce.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartRepository(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor,
               UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            return _userManager.GetUserId(principal);
        }

        // ➕ إضافة منتج أو زيادة الكمية
        public async Task<int> AddItem(int bookId, int qty)
        {
            string userId = GetUserId();
            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("user not logged-in");

                var cart = await GetCart(userId);
                if (cart is null)
                {
                    cart = new ShoppingCart { UserId = userId };
                    _db.ShoppingCarts.Add(cart);
                    await _db.SaveChangesAsync();
                }

                var cartItem = await _db.CartDetails
                    .FirstOrDefaultAsync(a => a.shoppingcartid == cart.Id && a.BookId == bookId);

                if (cartItem is not null)
                {
                    cartItem.Quantity += qty;
                }
                else
                {
                    cartItem = new CartDetails
                    {
                        BookId = bookId,
                        shoppingcartid = cart.Id,
                        Quantity = qty
                    };
                    _db.CartDetails.Add(cartItem);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
            }

            return await GetCartItemCount(userId);
        }

        // ➖ إنقاص كمية (ولو وصلت صفر ينحذف المنتج)
        public async Task<int> RemoveItem(int bookId, int qty = 1)
        {
            string userId = GetUserId();

            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("user not logged-in");

                var cart = await GetCart(userId);
                if (cart is null)
                    throw new Exception("invalid cart");

                var cartItem = await _db.CartDetails
                    .FirstOrDefaultAsync(a => a.shoppingcartid == cart.Id && a.BookId == bookId);

                if (cartItem is null)
                    throw new Exception("no items in cart");

                if (cartItem.Quantity <= qty)
                    _db.CartDetails.Remove(cartItem);
                else
                    cartItem.Quantity -= qty;

                await _db.SaveChangesAsync();
            }
            catch
            {
                // logging
            }

            return await GetCartItemCount(userId);
        }

        // ❌ حذف المنتج بالكامل من الكارت
        public async Task<int> DeleteItem(int bookId)
        {
            string userId = GetUserId();

            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("user not logged-in");

                var cart = await GetCart(userId);
                if (cart is null)
                    throw new Exception("invalid cart");

                var cartItem = await _db.CartDetails
                    .FirstOrDefaultAsync(a => a.shoppingcartid == cart.Id && a.BookId == bookId);

                if (cartItem is not null)
                {
                    _db.CartDetails.Remove(cartItem);
                    await _db.SaveChangesAsync();
                }
            }
            catch
            {
                // logging
            }

            return await GetCartItemCount(userId);
        }

        // 🛒 استرجاع كارت المستخدم الحالي
        public async Task<ShoppingCart> GetUserCart()
        {
            var userId = GetUserId();
            if (userId == null)
                throw new InvalidOperationException("Invalid userid");

            return await _db.ShoppingCarts
                .Include(a => a.CartDetails)
                .ThenInclude(a => a.Book)
                .ThenInclude(a => a.Genre)
                .FirstOrDefaultAsync(a => a.UserId == userId);
        }

        // 🛒 استرجاع كارت حسب userId
        public async Task<ShoppingCart> GetCart(string userId)
        {
            return await _db.ShoppingCarts
                .Include(c => c.CartDetails)
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        // 🔢 عدد المنتجات في الكارت
        public async Task<int> GetCartItemCount(string userId = "")
        {
            if (string.IsNullOrEmpty(userId))
                userId = GetUserId();

            var data = await (from cart in _db.ShoppingCarts
                              join cartDetail in _db.CartDetails
                                  on cart.Id equals cartDetail.shoppingcartid
                              where cart.UserId == userId
                              select cartDetail.Quantity).ToListAsync();

            return data.Sum();
        }
        public async Task<bool> DoCheckout()
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                // logic
                // move data from cartDetail to order and order detail then we will remove cart detail
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("User is not logged-in");
                var cart = await GetCart(userId);
                if (cart is null)
                    throw new InvalidOperationException("Invalid cart");
                var cartDetail = _db.CartDetails
                                   .Where(a => a.shoppingcartid == cart.Id).ToList();
                if (cartDetail.Count == 0)
                    throw new InvalidOperationException("Cart is empty");
                var pendingRecord = _db.OrderStatuses.FirstOrDefault(s => s.Name == "Pending");
                if (pendingRecord is null)
                    throw new InvalidOperationException("Order status does not have Pending status");
                var order = new Order
                {
                    UserId = userId,
                    CreateDate = DateTime.UtcNow,
                    OrderStatusId = 1,
                };
                _db.Orders.Add(order);
                _db.SaveChanges();
                foreach (var item in cartDetail)
                {
                    var orderDetail = new OrderDetails
                    {
                        BookId = item.BookId,
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };
                    _db.OrderDetails.Add(orderDetail);
                }
                _db.SaveChanges();

                _db.CartDetails.RemoveRange(cartDetail);
                _db.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
