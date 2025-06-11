const Footer = () => {

    return (
        <footer style={footerStyle}>
            <p>© 2025 Shoptester Kurs: Testning - ( UI Exempel ). Quote : All your base are belong to us.</p>
        </footer>
    )
}

const footerStyle = {
    backgroundColor: '#333',
    color: 'white',
    textAlign: 'center',
    padding: '10px 10px',
    position: 'absolut',
    bottom: 0,
    width: '100%',
    borderRadius: '10px',
    marginTop: '20px'
};

export default Footer;