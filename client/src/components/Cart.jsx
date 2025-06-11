import { useCart } from '../context/CartContext';
import { useNavigate } from 'react-router-dom';
import { useState } from 'react'; // Add this import

export default function Cart() {
    const { cart, removeFromCart, updateQuantity, getTotal, checkout, user } = useCart();
    const navigate = useNavigate();
    const [confirmationMessage, setConfirmationMessage] = useState('');

    const handleCheckout = async () => {
        if (!user) {
            navigate('/login');
            return;
        }

        try {
            await checkout();
            setConfirmationMessage('Order placed successfully! Thank you for your purchase.');
        } catch (error) {
            setConfirmationMessage('Failed to place order: ' + error.message);
        }
    };

    return (
        <div className="cart">
            <h2>Shopping Cart</h2>
            {confirmationMessage && (
                <div style={{
                    padding: '10px',
                    margin: '10px 0',
                    color: 'black',
                    backgroundColor: confirmationMessage.includes('Failed') ? '#ffebee' : '#e8f5e9',
                    border: '1px solid',
                    borderColor: confirmationMessage.includes('Failed') ? '#ef5350' : '#66bb6a',
                    borderRadius: '4px'
                }}>
                    {confirmationMessage}
                </div>
            )}
            {cart.length === 0 ? (
                <p>Your cart is empty</p>
            ) : (
                <>
                    {cart.map((item) => (
                        <div id={item.name} key={item.id} className="cart-item" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', margin: '10px' }}>
                            <img
                                src={item.image_url}
                                alt={item.name}
                                style={{ width: '50px', height: '50px' }}
                            />
                            <h3 style={{ flex: '1', margin: '0 10px' }}>{item.name}</h3>
                            <p style={{ width: '100px', textAlign: 'right' }}>${item.price}</p>
                            <div className="quantity" style={{ display: 'flex', alignItems: 'center' }}>
                                <button id='button-reduce' style={{ marginLeft: '20px' }} onClick={() => updateQuantity(item.id, item.quantity - 1)}>
                                    -
                                </button>
                                <span style={{ margin: '0 10px' }}>{item.quantity}</span>
                                <button id='button-add' onClick={() => updateQuantity(item.id, item.quantity + 1)}>
                                    +
                                </button>
                            </div>
                            <p style={{ width: '100px', textAlign: 'right' }}>Total: ${item.price * item.quantity}</p>
                            <button id='button-delete' style={{ marginLeft: '12px', backgroundColor: 'darkred' }} onClick={() => removeFromCart(item.id)}>Remove</button>
                        </div>
                    ))}
                    <div className="cart-total" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginTop: '20px' }}>
                        <h3>Total: ${getTotal()}</h3>
                        <button
                            id='checkout-button'
                            onClick={handleCheckout}
                            disabled={!user}
                            style={{
                                padding: '10px 20px',
                                backgroundColor: user ? '#4CAF50' : '#cccccc',
                                color: 'white',
                                border: 'none',
                                borderRadius: '5px',
                                cursor: user ? 'pointer' : 'not-allowed'
                            }}
                        >
                            {user ? 'Checkout' : 'Login to Checkout'}
                        </button>
                    </div>
                </>
            )}
        </div>
    );
}