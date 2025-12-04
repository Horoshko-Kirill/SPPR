document.addEventListener("DOMContentLoaded", () => {

    let cart = {
        items: [],
        totalPrice: 0,
        totalCount: 0
    };

    function updateCartDisplay() {
        const cartLink = document.querySelector('a[asp-controller="Cart"]');
        if (cartLink) {
            cartLink.innerHTML = `${cart.totalPrice.toFixed(2)} руб <i class="bi bi-cart"></i> (${cart.totalCount})`;
        }
    }

    document.querySelectorAll('.add-to-cart').forEach(btn => {
        btn.addEventListener('click', () => {
            const id = parseInt(btn.dataset.id);
            const price = parseFloat(btn.dataset.price);

            let item = cart.items.find(i => i.id === id);
            if (!item) {
                cart.items.push({ id: id, quantity: 1, price: price });
            } else {
                item.quantity++;
            }

            cart.totalCount = cart.items.reduce((sum, i) => sum + i.quantity, 0);
            cart.totalPrice = cart.items.reduce((sum, i) => sum + i.quantity * i.price, 0);

            updateCartDisplay();
        });
    });

    updateCartDisplay();
});
