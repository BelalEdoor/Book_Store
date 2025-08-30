using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookShopping_Ecommerce.Repositories;

namespace BookShopping_Ecommerce.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepo;
        public CartController(ICartRepository cartRepo)
        {
            _cartRepo = cartRepo;
        }

        // ➕ إضافة منتج أو زيادة كمية
        public async Task<IActionResult> AddItem(int bookId, int qty = 1, int redirect = 0)
        {
            var cartCount = await _cartRepo.AddItem(bookId, qty);

            if (redirect == 0) // ajax call
                return Ok(cartCount);

            return RedirectToAction("GetUserCart");
        }


        // ⬆️ زيادة
        public async Task<IActionResult> Increase(int bookId)
        {
            await _cartRepo.AddItem(bookId, 1);
            return RedirectToAction("GetUserCart");
        }

        // ⬇️ إنقاص
        public async Task<IActionResult> Decrease(int bookId)
        {
            await _cartRepo.RemoveItem(bookId, 1);
            return RedirectToAction("GetUserCart");
        }

        // ❌ حذف كامل للمنتج
        public async Task<IActionResult> Delete(int bookId)
        {
            await _cartRepo.DeleteItem(bookId); // ⬅️ استدعاء الميثود الجديد
            return RedirectToAction("GetUserCart");
        }

        // 🛒 عرض الكارت
        public async Task<IActionResult> GetUserCart()
        {
            var cart = await _cartRepo.GetUserCart();
            return View(cart);
        }

        // 🔢 إرجاع عدد المنتجات
        public async Task<IActionResult> GetTotalItemInCart()
        {
            int cartItem = await _cartRepo.GetCartItemCount();
            return Ok(cartItem);
        }
        public IActionResult DoCheckout()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DoCheckout(CheckoutModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            bool isCheckout = await _cartRepo.DoCheckout(model);
            if (!isCheckout)
                return RedirectToAction(nameof(OrderFailure));
            return RedirectToAction(nameof(OrderSucces));

        }
        public IActionResult OrderSucces()
        {
            return View();
        }
        public IActionResult OrderFailure()
        {
            return View();
        }


    }
}
