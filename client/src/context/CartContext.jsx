import { createContext, useContext, useState } from 'react';
import { useUser } from './UserContext';

const CartContext = createContext();

export function CartProvider({ children }) {
    const [cart, setCart] = useState([]);
    const { user } = useUser();

    const createOrder = async (item) => {
        if (!user) {
            throw new Error('Must be logged in to create order');
        }

        try {
            const response = await fetch('/api/orders', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include',
                body: JSON.stringify({
                    customerId: user.id,
                    productId: item.id,
                    quantity: item.quantity
                })
            });

            if (!response.ok) {
                throw new Error('Failed to create order');
            }

            return await response.json();
        } catch (error) {
            console.error('Order creation failed:', error);
            throw error;
        }
    };

    const checkout = async () => {
        if (!user) {
            throw new Error('Must be logged in to checkout');
        }

        try {
            // Create orders for each cart item
            const orders = await Promise.all(
                cart.map(item => createOrder(item))
            );

            // Clear the cart after successful checkout
            clearCart();
            //console.log('Orders:', orders);
            return orders;
        } catch (error) {
            console.error('Checkout failed:', error);
            throw error;
        }
    };

    const addToCart = (product) => {
        setCart(currentCart => {
            const existingItem = currentCart.find(item => item.id === product.id);

            if (existingItem) {
                return currentCart.map(item =>
                    item.id === product.id
                        ? { ...item, quantity: item.quantity + 1 }
                        : item
                );
            }

            return [...currentCart, { ...product, quantity: 1 }];
        });
    };

    const removeFromCart = (productId) => {
        setCart(currentCart => currentCart.filter(item => item.id !== productId));
    };

    const updateQuantity = (productId, quantity) => {
        if (quantity < 1) {
            removeFromCart(productId);
            return;
        }

        setCart(currentCart =>
            currentCart.map(item =>
                item.id === productId ? { ...item, quantity } : item
            )
        );
    };

    const clearCart = () => {
        setCart([]);
    };

    const getTotal = () => {
        return cart.reduce((total, item) => total + (item.price * item.quantity), 0);
    };

    return (
        <CartContext.Provider value={{
            cart,
            addToCart,
            removeFromCart,
            updateQuantity,
            clearCart,
            getTotal,
            checkout,
            user
        }}>
            {children}
        </CartContext.Provider>
    );
}

export function useCart() {
    return useContext(CartContext);
}