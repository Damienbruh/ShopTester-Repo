import { useEffect, useState } from "react";
import { NavLink, Outlet } from "react-router";
import { useNavigate } from "react-router-dom";
import Footer from "../components/Footer"
import { useUser } from '../context/UserContext';
import { useCart } from '../context/CartContext';

function Layout() {
    const [isLoginDialogOpen, setIsLoginDialogOpen] = useState(false);
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [username, setUsername] = useState("");
    const [isRegisterDialogOpen, setIsRegisterDialogOpen] = useState(false);
    const [confirmationMessage, setConfirmationMessage] = useState('');
    const { user, login, logout } = useUser();
    const { cart } = useCart();
    const navigate = useNavigate();

    const toggleLogoutButton = () => {
        const logoutButton = document.getElementById("logout-button");
        if (user) {
            if (user.id >= 1) {
                logoutButton.style.display = "block";
            } else {
                logoutButton.style.display = "none";
            }
        } else {
            logoutButton.style.display = "none";
        }
    }

    useEffect(() => {
        toggleLogoutButton();
    });

    const handleRegisterClick = () => {
        setIsRegisterDialogOpen(true);
    }

    const handleRegisterDialogClose = () => {
        setIsRegisterDialogOpen(false);
    }

    const handleLoginClick = () => {
        setIsLoginDialogOpen(true);
    };

    const handleLoginDialogClose = () => {
        setIsLoginDialogOpen(false);
    };

    const handleLogoutClick = async () => {
        await fetch("/api/login", {
            method: "DELETE",
            credentials: "include"
        });
        logout();
        navigate("/");
    };

    const handleRegisterSubmit = async (e) => {
        e.preventDefault();
        if (password !== confirmPassword) {
            alert("Passwords do not match!");
            return;
        }
        let roleId = 2;
        const response = await fetch("/api/users", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ email, username, password, roleId }),
        });
        if (response.ok) {
            setIsRegisterDialogOpen(false);
            setConfirmationMessage('Registration successfull!');
            setTimeout(() => setConfirmationMessage(''), 5000);
            //setTimeout(() => { window.location.reload(); }, 5000);

            setConfirmPassword("");
            setEmail("");
            setPassword("");
            setUsername("");
        } else {
            alert("Registration failed!");
            console.error("Registration failed!");
        }

    }


    const handleLoginSubmit = async (event) => {
        event.preventDefault();
        const response = await fetch("/api/login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ email, password }),
            credentials: "include"
        });
        if (response.ok) {
            const data = await response.json();
            login(data);
            setConfirmationMessage('Login successfull!');
            setTimeout(() => setConfirmationMessage(''), 5000);
            //console.log("Login successful!", data);

        } else {
            alert("Login failed!");
            console.error("Login failed!");
        }
        setEmail("");
        setPassword("");
        setIsLoginDialogOpen(false);
    };
    const getTotalItems = () => {
        return cart.reduce((total, item) => total + item.quantity, 0);
    };

    return <>
        <header>
            <nav className="navbar">
                {user ? <NavLink to={`/profile`}>Profile</NavLink> : null}
                <NavLink id="nav-home" to={"/"}>Home</NavLink>
                <NavLink id="nav-shop" to={"/shop"}>Shop</NavLink>
                <NavLink id="nav-cart" to={"/cart"}>Cart ({getTotalItems()})</NavLink>
                {user && user.role == "admin" ? <NavLink to={"/admin"}>Admin</NavLink> : null}
                {user ? (
                    <span id="logged-in-user">Welcome, {user.username.charAt(0).toUpperCase() + user.username.slice(1)}</span>
                ) : (
                    <button id="login-button" onClick={handleLoginClick}>Login</button>
                )}<button id="logout-button" onClick={handleLogoutClick}>Logout</button>
                {!user ? (<button id="register-button" onClick={handleRegisterClick}>Register</button>) : null}
            </nav>
        </header>
        <main>
            <Outlet />
        </main>
        <Footer />
        {isLoginDialogOpen && (
            <div className="dialog-overlay">
                <div className="dialog">
                    <h2>Login</h2>
                    <form id="login-form" onSubmit={handleLoginSubmit}>
                        <label>
                            Email:
                            <input
                                type="email"
                                name="email"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                required
                            />
                        </label>
                        <label>
                            Password:
                            <input
                                type="password"
                                name="password"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                required
                            />
                        </label>
                        <div className="button-group">
                            <button type="button" onClick={handleLoginDialogClose}>Close</button>
                            <button type="submit">Submit</button>
                        </div>
                    </form>
                </div>
            </div>
        )}
        {isRegisterDialogOpen && (
            <div className="dialog-overlay">
                <div className="dialog">
                    <h2>Login</h2>
                    <form id="register-form" onSubmit={handleRegisterSubmit}>
                        <label>
                            Email:
                            <input
                                type="email"
                                name="email"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                required
                            />
                        </label>
                        <label>
                            Username:
                            <input
                                type="text"
                                name="username"
                                value={username}
                                onChange={(e) => setUsername(e.target.value)}
                                required
                            />
                        </label>
                        <label>
                            Password:
                            <input
                                type="password"
                                name="password"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                required
                            />
                        </label>
                        <label>
                            Confirm Password:
                            <input
                                type="password"
                                name="confirmPassword"
                                value={confirmPassword}
                                onChange={(e) => setConfirmPassword(e.target.value)}
                                required
                            />
                        </label>
                        <div className="button-group">
                            <button type="button" onClick={handleRegisterDialogClose}>Close</button>
                            <button type="submit">Submit</button>
                        </div>
                    </form>
                </div>
            </div>
        )}
        {confirmationMessage && (
            <div id='confirmation' style={{
                padding: '10px',
                margin: '10px 0',
                backgroundColor: confirmationMessage.includes('Error') || confirmationMessage.includes('Failed')
                    ? '#ffebee'
                    : '#e8f5e9',
                color: confirmationMessage.includes('Error') || confirmationMessage.includes('Failed')
                    ? '#c62828'
                    : '#2e7d32',
                border: '1px solid',
                borderColor: confirmationMessage.includes('Error') || confirmationMessage.includes('Failed')
                    ? '#ef5350'
                    : '#66bb6a',
                borderRadius: '4px',
                position: 'fixed',
                top: '20px',
                right: '20px',
                zIndex: 1000
            }}>
                {confirmationMessage}
            </div>
        )}
    </>
}

export default Layout